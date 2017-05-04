using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Chunks;

namespace Kachuwa.InsightConsole
{
    class Program
    {
        private static object consoleLock = new object();
        private const int sendChunkSize = 256;
        private const int receiveChunkSize = 256;
        private const bool verbose = true;
        private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(30000);
        static UTF8Encoding encoder = new UTF8Encoding();
        static void  Main(string[] args)
        {
            //Connect().Wait();
            //Console.WriteLine("Press any key to exit...");
            //Console.ReadKey();
           
                try
                {
                    Connect2().Wait();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                    Thread.Sleep(5000);
                    Console.ReadKey();
                }

            Console.ReadKey();
        }

        private static async Task Connect2()
        {
            var sendBuffer = new byte[1024];
            var sendSegment = new ArraySegment<byte>(sendBuffer);

            var receiveBuffer = new byte[1024];
            var receiveSegment = new ArraySegment<byte>(receiveBuffer);

            using (ClientWebSocket cws = await GetConnectedWebSocket(new Uri( "wss://kachuwaframwork.azurewebsites.net/insight"), 10000))
            {
                var ctsDefault = new CancellationTokenSource(10000);

                // The server will read buffers and aggregate it up to 64KB before echoing back a complete message.
                // But since this test uses a receive buffer that is small, we will get back partial message fragments
                // as we read them until we read the complete message payload.
                //for (int i = 0; i < 63; i++)
                //{
                //    await cws.SendAsync(sendSegment, WebSocketMessageType.Binary, false, ctsDefault.Token);
                //}
                //await cws.SendAsync(sendSegment, WebSocketMessageType.Binary, true, ctsDefault.Token);

               // WebSocketReceiveResult recvResult = await cws.ReceiveAsync(receiveSegment, ctsDefault.Token);
                //  Assert.Equal(false, recvResult.EndOfMessage);

                while (true)
                {
                    await Receive(cws, ctsDefault.Token);
                }
                //while (recvResult.EndOfMessage == false)
                //{
                //    recvResult = await cws.ReceiveAsync(receiveSegment, ctsDefault.Token);
                //}
                
               // await cws.CloseAsync(WebSocketCloseStatus.NormalClosure, "PartialMessageTest", ctsDefault.Token);
            }
        }

        public static async Task<ClientWebSocket> GetConnectedWebSocket(
          Uri server,
          int timeOutMilliseconds,
          //  ITestOutputHelper output,
          TimeSpan keepAliveInterval = default(TimeSpan))
        {
            const int MaxTries = 5;
            int betweenTryDelayMilliseconds = 1000;

            for (int i = 1; ; i++)
            {
                try
                {
                    var cws = new ClientWebSocket();
                    if (keepAliveInterval.TotalSeconds > 0)
                    {
                        cws.Options.KeepAliveInterval = keepAliveInterval;
                    }

                    using (var cts = new CancellationTokenSource(timeOutMilliseconds))
                    {
                       // output.WriteLine("GetConnectedWebSocket: ConnectAsync starting.");
                        Task taskConnect = cws.ConnectAsync(server, cts.Token);
                        //Assert.True(
                        //    (cws.State == WebSocketState.None) ||
                        //    (cws.State == WebSocketState.Connecting) ||
                        //    (cws.State == WebSocketState.Open) ||
                        //    (cws.State == WebSocketState.Aborted),
                        //    "State immediately after ConnectAsync incorrect: " + cws.State);
                        await taskConnect;
                        Console.WriteLine("GetConnectedWebSocket: ConnectAsync done.");
                        // Assert.Equal(WebSocketState.Open, cws.State);
                    }
                    return cws;
                }
                catch (WebSocketException exc)
                {
                    Console.WriteLine($"Retry after attempt #{i} failed with {exc}");
                    if (i == MaxTries)
                    {
                        throw;
                    }

                    await Task.Delay(betweenTryDelayMilliseconds);
                    betweenTryDelayMilliseconds *= 2;
                }
            }
        }

        private static bool InitWebSocketSupported()
        {
            ClientWebSocket cws = null;

            try
            {
                cws = new ClientWebSocket();
                return true;
            }
            catch (PlatformNotSupportedException)
            {
                return false;
            }
            finally
            {
                if (cws != null)
                {
                    cws.Dispose();
                }
            }
        }

        private async static Task Connect()
        {
            //using (var cws = new ClientWebSocket())
            //{
            //    await cws.ConnectAsync(new Uri("wss://kachuwaframwork.azurewebsites.net/insight"), CancellationToken.None);
            //    await Task.WhenAll(Receive(cws), Send(cws));
            //}
            ClientWebSocket webSocket = null;

            try
            {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri("wss://kachuwaframwork.azurewebsites.net/insight"), CancellationToken.None);
              
               // await Task.WhenAll(Receive(webSocket), Send(webSocket));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
            finally
            {
                if (webSocket != null)
                    webSocket.Dispose();
                Console.WriteLine();

                lock (consoleLock)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("WebSocket closed.");
                    Console.ResetColor();
                }
            }
        }
        private static async Task Send(ClientWebSocket webSocket)
        {

            //byte[] buffer = encoder.GetBytes("{\"op\":\"blocks_sub\"}"); //"{\"op\":\"unconfirmed_sub\"}");
            byte[] buffer = encoder.GetBytes("{\"op\":\"unconfirmed_sub\"}");
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            while (webSocket.State == WebSocketState.Open)
            {
                LogStatus(false, buffer, buffer.Length);
                await Task.Delay(delay);
            }
        }

        private static async Task Receive(ClientWebSocket webSocket, CancellationToken ctsDefault)
        {
            byte[] buffer = new byte[receiveChunkSize];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), ctsDefault);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    LogStatus(true, buffer, result.Count);
                }
            }
        }

        private static void LogStatus(bool receiving, byte[] buffer, int length)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = receiving ? ConsoleColor.Green : ConsoleColor.Gray;
                //Console.WriteLine("{0} ", receiving ? "Received" : "Sent");

                if (verbose)
                    Console.WriteLine(encoder.GetString(buffer));

                Console.ResetColor();
            }
        }
    }
    

      
    
}