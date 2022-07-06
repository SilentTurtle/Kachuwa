using Kachuwa.Identity.Extensions;
using Kachuwa.Web;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kachuwa.RTC.Hubs
{

    public class CallOffer
    {
        public RTCUser Caller { get; set; }
        public RTCUser Callee { get; set; }
    }


    public class UserCall
    {
        public List<RTCUser> Users { get; set; }
    }
    public interface IWebRTCConnectionHub
    {
        Task UpdateUsers(List<RTCUser> userList);
        Task CallAccepted(RTCUser acceptingUser);
        Task CallDeclined(RTCUser decliningUser, string reason);
        Task onCallerCanceled(RTCUser cancelingUser, string reason);
        Task IncomingCall(RTCUser callingUser);
        Task ReceiveSignal(RTCUser signalingUser, string signal);
        Task CallEnded(RTCUser signalingUser, string signal);
        Task onUserChange(RtcUserStatus status);
        Task onDestroyCall(string status);
    }

    public class WebRTCHub : Hub<IWebRTCConnectionHub>
    {

        private readonly List<UserCall> _UserCalls;
        private readonly List<CallOffer> _CallOffers;
        public readonly IRTCConnectionManager ConnectionManager;
        public WebRTCHub(IRTCConnectionManager connectionManager, List<UserCall> userCalls, List<CallOffer> callOffers)
        {
            ConnectionManager = connectionManager;
            _UserCalls = userCalls;
            _CallOffers = callOffers;
        }
        #region Default
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            var rtcUser = new RTCUser();
            var context = ContextResolver.Context;
            string ua = context.Request.Headers["User-Agent"].ToString();
            rtcUser.UserDevice = ua;
            rtcUser.ConnectionIds.Add(connectionId);
            rtcUser.IdentityUserId = context.User.Identity.GetIdentityUserId();
            rtcUser.SessionId = context.Session.Id;
            rtcUser.GroupName = "";
            rtcUser.HubNames.Add("Voice");
            //rtcUser.IsFromMobile = false;
            rtcUser.IsFromWeb = true;
            rtcUser.UserRoles = string.Join(',', context.User.Identity.GetRoles());
            rtcUser.ConnectionId = connectionId;
            rtcUser.UserName = context.User.Identity.Name;
            rtcUser.FullName = context.User.Identity.GetFullName();
            rtcUser.Picture = context.User.Identity.GetPicture();
            ConnectionManager.AddUser(rtcUser);

            try
            {
                var status = await ConnectionManager.GetOnlineUserStatus();
                await Clients.All.onUserChange(status);
                await Clients.All.UpdateUsers(await ConnectionManager.GetUsersByHub("Voice"));


            }
            catch (Exception e)
            {

            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ConnectionManager.RemoveUser(Context.ConnectionId);
            try
            {
                var status = await ConnectionManager.GetOnlineUserStatus();
                await Clients.All.onUserChange(status);
                await Clients.All.UpdateUsers(await ConnectionManager.GetUsersByHub("Voice"));

            }
            catch (Exception e)
            {

            }

            await base.OnDisconnectedAsync(exception);
        }
        #endregion
        //public async Task Join(string username)
        //{
        //    // Add the new user
        //    _Users.Add(new User
        //    {
        //        Username = username,
        //        ConnectionId = Context.ConnectionId
        //    });

        //    // Send down the new list to all clients
        //    await SendUserListUpdate();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    // Hang up any calls the user is in
        //    await HangUp(); // Gets the user from "Context" which is available in the whole hub

        //    // Remove the user
        //    _Users.RemoveAll(u => u.ConnectionId == Context.ConnectionId);

        //    // Send down the new user list to all clients
        //    await SendUserListUpdate();

        //    await base.OnDisconnectedAsync(exception);
        //}

        public async Task CallUser(RTCUser targetConnectionId)
        {
            var callingUser = await ConnectionManager.GetCurrentUser(Context.ConnectionId);
            // var callingUser = _Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
            // var targetUser = _Users.SingleOrDefault(u => u.ConnectionId == targetConnectionId.ConnectionId);
            var targetUser = await ConnectionManager.GetUser(targetConnectionId.IdentityUserId, targetConnectionId.ConnectionId);
            // Make sure the person we are trying to call is still here
            if (targetUser == null)
            {
                // If not, let the caller know
                await Clients.Caller.CallDeclined(targetConnectionId, "The user you called has left.");
                return;
            }

            // And that they aren't already in a call
            if (GetUserCall(targetUser.ConnectionId) != null)
            {
                await Clients.Caller.CallDeclined(targetConnectionId, string.Format("{0} is already in a call.", targetUser.UserName));
                return;
            }

            // They are here, so tell them someone wants to talk
            if (targetUser.IdentityUserId > 0)
            {
                foreach (var conId in targetUser.ConnectionIds)
                {
                    await Clients.Client(conId).IncomingCall(callingUser);
                }

            }
            else
            {
                await Clients.Client(targetConnectionId.ConnectionId).IncomingCall(callingUser);
            }


            // Create an offer
            _CallOffers.Add(new CallOffer
            {
                Caller = callingUser,
                Callee = targetUser
            });
        }

        public async Task AnswerCall(bool acceptCall, RTCUser targetConnectionId)
        {
            var callingUser = await ConnectionManager.GetCurrentUser(Context.ConnectionId);
            // var callingUser = _Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
            // var targetUser = _Users.SingleOrDefault(u => u.ConnectionId == targetConnectionId.ConnectionId);
            var targetUser = await ConnectionManager.GetUser(targetConnectionId.IdentityUserId, targetConnectionId.ConnectionId);

            // This can only happen if the server-side came down and clients were cleared, while the user
            // still held their browser session.
            if (callingUser == null)
            {
                return;
            }

            // Make sure the original caller has not left the page yet
            if (targetUser == null)
            {
                await Clients.Caller.CallEnded(targetConnectionId, "The other user in your call has left.");
                return;
            }

            // Send a decline message if the callee said no
            if (acceptCall == false)
            {
                await Clients.Client(targetConnectionId.ConnectionId).CallDeclined(callingUser, string.Format("{0} did not accept your call.", callingUser.UserName));
                //destroying other own instances of incoming calls
                foreach (var conn in callingUser.ConnectionIds)
                {
                    await Clients.Client(conn).onDestroyCall($"call connectected on {callingUser.ConnectionId}");
                }
                return;
            }

            // Make sure there is still an active offer.  If there isn't, then the other use hung up before the Callee answered.
            var offerCount = _CallOffers.RemoveAll(c => c.Callee.ConnectionId == callingUser.ConnectionId
                                                  && c.Caller.ConnectionId == targetUser.ConnectionId);
            if (offerCount < 1)
            {
                await Clients.Caller.CallEnded(targetConnectionId, string.Format("{0} has already hung up.", targetUser.UserName));
                return;
            }

            // And finally... make sure the user hasn't accepted another call already
            if (GetUserCall(targetUser.ConnectionId) != null)
            {
                // And that they aren't already in a call
                await Clients.Caller.CallDeclined(targetConnectionId, string.Format("{0} chose to accept someone elses call instead of yours :(", targetUser.UserName));
                return;
            }

            // Remove all the other offers for the call initiator, in case they have multiple calls out
            _CallOffers.RemoveAll(c => c.Caller.ConnectionId == targetUser.ConnectionId);

            // Create a new call to match these folks up
            _UserCalls.Add(new UserCall
            {
                Users = new List<RTCUser> { callingUser, targetUser }
            });

            // Tell the original caller that the call was accepted
            await Clients.Client(targetConnectionId.ConnectionId).CallAccepted(callingUser);

            //destroying multiple instance of call
            foreach (var conn in callingUser.ConnectionIds)
            {
                if (conn != callingUser.ConnectionId)
                    await Clients.Client(conn).onDestroyCall($"call connectected on {callingUser.ConnectionId}");
            }

            // Update the user list, since thes two are now in a call
            // await SendUserListUpdate();
            await Clients.All.UpdateUsers(await ConnectionManager.GetUsersByHub("Voice"));
        }

        public async Task CancelCall(RTCUser targetConnectionId)
        {
            var callingUser = await ConnectionManager.GetCurrentUser(Context.ConnectionId);
            // var callingUser = _Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
            // var targetUser = _Users.SingleOrDefault(u => u.ConnectionId == targetConnectionId.ConnectionId);
            var targetUser = await ConnectionManager.GetUser(targetConnectionId.IdentityUserId, targetConnectionId.ConnectionId);

            // This can only happen if the server-side came down and clients were cleared, while the user
            // still held their browser session.
            if (callingUser == null)
            {
                return;
            }

            // Make sure the original caller has not left the page yet
            if (targetUser == null)
            {
                await Clients.Caller.CallEnded(targetConnectionId, "The other user in your call has left.");
                return;
            }

            foreach (var conn in targetConnectionId.ConnectionIds)
                await Clients.Client(conn).onCallerCanceled(callingUser, string.Format("{0} canceled call.", callingUser.UserName));
            return;



        }


        public async Task HangUp()
        {
            var callingUser = await ConnectionManager.GetCurrentUser(Context.ConnectionId);

            if (callingUser == null)
            {
                return;
            }

            var currentCall = GetUserCall(callingUser.ConnectionId);

            // Send a hang up message to each user in the call, if there is one
            if (currentCall != null)
            {
                foreach (var user in currentCall.Users.Where(u => u.ConnectionId != callingUser.ConnectionId))
                {
                    await Clients.Client(user.ConnectionId).CallEnded(callingUser, string.Format("{0} has hung up.", callingUser.UserName));
                }

                // Remove the call from the list if there is only one (or none) person left.  This should
                // always trigger now, but will be useful when we implement conferencing.
                currentCall.Users.RemoveAll(u => u.ConnectionId == callingUser.ConnectionId);
                if (currentCall.Users.Count < 2)
                {
                    _UserCalls.Remove(currentCall);
                }
            }

            // Remove all offers initiating from the caller
            _CallOffers.RemoveAll(c => c.Caller.ConnectionId == callingUser.ConnectionId);

            //await SendUserListUpdate();
            await Clients.All.UpdateUsers(await ConnectionManager.GetUsersByHub("Voice"));
        }

        // WebRTC Signal Handler
        public async Task SendSignal(string signal, string targetConnectionId)
        {
            try
            {


                var callingUser = await ConnectionManager.GetCurrentUser(Context.ConnectionId);
                // var callingUser = _Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
                // var targetUser = _Users.SingleOrDefault(u => u.ConnectionId == targetConnectionId.ConnectionId);
                var targetUser = await ConnectionManager.GetUser(0, targetConnectionId);

                // Make sure both users are valid
                if (callingUser == null || targetUser == null)
                {
                    return;
                }

                // Make sure that the person sending the signal is in a call
                var userCall = GetUserCall(callingUser.ConnectionId);

                // ...and that the target is the one they are in a call with
                if (userCall != null && userCall.Users.Exists(u => u.ConnectionId == targetUser.ConnectionId))
                {
                    //JObject sdpAnswer = new JObject {
                    //        { "type", "sdp" },
                    //        { "answer", sdp }
                    //    };

                    // These folks are in a call together, let's let em talk WebRTC
                    await Clients.Client(targetConnectionId).ReceiveSignal(callingUser, signal);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #region Private Helpers


        private UserCall GetUserCall(string connectionId)
        {
            var matchingCall =
                _UserCalls.SingleOrDefault(uc => uc.Users.SingleOrDefault(u => u.ConnectionId == connectionId) != null);
            return matchingCall;
        }

        #endregion
    }
}