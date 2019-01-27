using GGTServer.Helpers;
using GGTServer.Models;
using GGTServer.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.UI.Core;

namespace GGTServer.Services
{
    public static class CommunicationService
    {


        public static StreamSocketListener ListenerSocket { get; set; } = null;

        private static ConcurrentBag<StreamSocket> Clients = new ConcurrentBag<StreamSocket>();

        public static String Result { get; set; } = String.Empty;

        public static String Port { get; set; } = "63000";

        public static Int32 Idx { get; set; } = 1;

        public static async Task StartServer()
        {
            try
            {
                // StreamSocket을 만들고 서버동작을 위한 연결을 개설합니다.
                ListenerSocket = new StreamSocketListener();

                ListenerSocket.ConnectionReceived += ListenerSocket_ConnectionReceivedAsync;

                //  Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await ListenerSocket.BindServiceNameAsync(Port);
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                //this.clientListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }

        private static async void ListenerSocket_ConnectionReceivedAsync(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {

            //foreach (StreamSocket socket in Clients)
            //{
            //    if (socket.Information.LocalAddress.Equals(args.Socket))
            //    {
            //        Clients.
            //    }
            //}

            //if (Clients.)
            //{
            //    Clients.Add(args.Socket);
            //}

            String request = String.Empty;
            using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
            {
                request = await streamReader.ReadLineAsync();
            }


            foreach (StreamSocket socket in Clients)
            {
                //String request;
                //using (var streamReader = new StreamReader(socket.InputStream.AsStreamForRead()))
                //{
                //    request = await streamReader.ReadLineAsync();
                //}

                // Echo the request back as the response.
                using (Stream outputStream = socket.OutputStream.AsStreamForWrite())
                {
                    using (var streamWriter = new StreamWriter(outputStream))
                    {
                        await streamWriter.WriteLineAsync(request);
                        await streamWriter.FlushAsync();
                    }
                }
            }
        }

        public static void StopServer()
        {
            try
            {
                ListenerSocket.Dispose();
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                //this.clientListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }
    }
}
