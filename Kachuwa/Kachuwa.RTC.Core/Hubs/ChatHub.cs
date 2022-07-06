using Kachuwa.Data.Extension;
using Kachuwa.Extensions;
using Kachuwa.Identity.Extensions;
using Kachuwa.Web;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kachuwa.RTC.Hubs
{
    public class RoomViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class RoomContentViewModel : ChatContent
    {

    }
    public interface IClientChatHub
    {
        Task onError(string error);
        Task onChatRoomAdded(string room);
        Task onChatRoomUpdate(List<Room> rooms);
        Task onChatRoomRemoveUser(RTCUser user);
        Task onRoomDeleted(string room);
        Task onRoomUserUpdate(List<RTCUser> users);
        Task onNewContentRecieved(RoomContentViewModel content);
        Task onOlderContentRecieved(List<ChatContent> contents);
        Task onUserChange(RtcUserStatus status);

    }

    public class ChatHub : Hub<IClientChatHub>
    {
        private readonly static List<RoomViewModel> _Rooms = new List<RoomViewModel>();
        private readonly IChatService _chatService;
        public readonly IRTCConnectionManager ConnectionManager;
        public ChatHub(IRTCConnectionManager connectionManager, IChatService chatService)
        {
            _chatService = chatService;
            ConnectionManager = connectionManager;
        }

        #region Default
        public override Task OnConnectedAsync()
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
            rtcUser.HubNames.Add("Chat");
            //rtcUser.IsFromMobile = false;
            rtcUser.IsFromWeb = true;
            rtcUser.UserRoles = string.Join(',', context.User.Identity.GetRoles());
            rtcUser.ConnectionId = connectionId;
            ConnectionManager.AddUser(rtcUser);

            try
            {
                var status = ConnectionManager.GetOnlineUserStatus().Result;
                Clients.All.onUserChange(status);


            }
            catch (Exception e)
            {

            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectionManager.RemoveUser(Context.ConnectionId);
            try
            {
                var status = ConnectionManager.GetOnlineUserStatus().Result;
                Clients.All.onUserChange(status);

            }
            catch (Exception e)
            {

            }

            return base.OnDisconnectedAsync(exception);
        }
        #endregion
        public async Task SendToRoom(RoomContentViewModel content)
        {
            try
            {

                var model = content.To<ChatContent>();
                model.AutoFill();
                model.IsActive = true;
                await _chatService.ContentService.InsertAsync<long>(model);

                await Clients.Group(content.RoomName).onNewContentRecieved(content);

            }
            catch (Exception ex)
            {
                await Clients.Caller.onError(ex.Message);
            }
        }
        public async Task OldMessages(string roomName, int offset, int limit)
        {
            try
            {
                var data = await GetMessageHistory(roomName, offset, limit);
                await Clients.Caller.onOlderContentRecieved(data.ToList());
            }
            catch (Exception ex)
            {
                await Clients.Caller.onError("Couldn't create chat room: " + ex.Message);
            }
        }

        public async Task Join(string roomName)
        {
            try
            {
                if (string.IsNullOrEmpty(roomName))
                    return;
                var userId = ContextResolver.Context.User.Identity.GetIdentityUserId();
                var user = await this.ConnectionManager.GetCurrentUser(Context.ConnectionId);

                //currently user can have one room only TODO:: multiple rooms
                if (user.GroupName != roomName)
                { //user is different group/room 
                    // Remove user from others list
                    if (!string.IsNullOrEmpty(user.GroupName))
                        await Clients.OthersInGroup(user.GroupName).onChatRoomRemoveUser(user);

                    // Join to new chat room
                    await Leave(user.GroupName);

                }
                else
                {
                    //user is already in same group though new connections arrived and trying to join
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                if (userId > 0)
                    await this.ConnectionManager.UpdateGroupName(roomName, userId);
                else
                    await this.ConnectionManager.UpdateGroupName(roomName, Context.ConnectionId);
                var users = await this.ConnectionManager.GetUsersByGroup(roomName);
                // Tell others to update their list of users
                await Clients.OthersInGroup(roomName).onRoomUserUpdate(users);
            }
            catch (Exception ex)
            {
                await Clients.Caller.onError("You failed to join the chat room!" + ex.Message);
            }
        }

        public async Task Leave(string roomName)
        {
            var user = await this.ConnectionManager.GetCurrentUser(Context.ConnectionId);
            foreach (var connectionId in user.ConnectionIds)
                await Groups.RemoveFromGroupAsync(connectionId, roomName);
        }

        public async Task CreateRoom(string roomName)
        {
            try
            {
                if (!await _chatService.CheckRoomExists(roomName))
                {
                    var room = new Room();
                    room.Name = roomName;
                    room.AutoFill();
                    await _chatService.RoomService.InsertAsync<int>(room);
                    await Clients.All.onChatRoomAdded(roomName);
                }
                else
                {
                    await Clients.Caller.onError("Already Exist");
                }

            }
            catch (Exception ex)
            {
                await Clients.Caller.onError("Couldn't create chat room: " + ex.Message);
            }
        }

        public async Task DeleteRoom(string roomName)
        {
            try
            {
                var room = await _chatService.RoomService.GetAsync("Where Name=@Name and IsActive=@IsActive and IsDeleted=@IsDeleted", new { Name = roomName, IsActive = true, IsDeleted = false });
                await _chatService.RoomService.UpdateAsDeleted(room.RoomId);
                // Move users back to Lobby
                await Clients.Group(roomName).onRoomDeleted(string.Format("Room {0} has been deleted.\nYou are now moved to the Lobby!", roomName));

                // Tell all users to update their room list
                var rooms = await GetRooms();
                await Clients.All.onChatRoomUpdate(rooms.ToList());
            }
            catch (Exception)
            {
                await Clients.Caller.onError("Can't delete this chat room.");
            }
        }

        public async Task<IEnumerable<ChatContent>> GetMessageHistory(string roomName, int offset, int limit)
        {
            return await _chatService.ContentService.GetListPagedAsync(offset, limit, 1, "Where RoomName=@Name and IsActive=@IsActive and IsDeleted=@IsDeleted", "AddedOn desc", new { Name = roomName, IsActive = true, IsDeleted = false });
        }

        public async Task<IEnumerable<Room>> GetRooms()
        {

            return await _chatService.RoomService.GetListAsync("Where IsActive=@IsActive and IsDeleted=@IsDeleted", new { IsActive = true, IsDeleted = false });

        }

    }



}