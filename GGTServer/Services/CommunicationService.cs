using System;
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

        public static HostName HostName { get; set; } = null;

        public static String Result { get; set; } = String.Empty;

        public static String Port { get; set; } = "63000";

        public static async Task StartServer()
        {
            try
            {
                // Create the StreamSocket and establish a connection to the echo server.
                ListenerSocket = new StreamSocketListener();

                ListenerSocket.ConnectionReceived += ListenerSocket_ConnectionReceivedAsync;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await ListenerSocket.BindServiceNameAsync(Port);

            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                //this.clientListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }

        private static async void ListenerSocket_ConnectionReceivedAsync(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            String request;
            using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
            {
                request = await streamReader.ReadLineAsync();
            }

            // Echo the request back as the response.
            using (Stream outputStream = args.Socket.OutputStream.AsStreamForWrite())
            {
                using (var streamWriter = new StreamWriter(outputStream))
                {
                    await streamWriter.WriteLineAsync(request);
                    await streamWriter.FlushAsync();
                }
            }

            sender.Dispose();
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
