using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace GGTClient.Services
{
    public static class CommunicationService
    {
        public static StreamSocket ClientSocket { get; set; } = null;

        public static HostName HostName { get; set; } = null;

        public static String Result { get; set; } = String.Empty;

        public static String Port { get; set; } = "63000";

        public static async Task StartClient()
        {
            try
            {
                // Create the StreamSocket and establish a connection to the echo server.
                ClientSocket = new StreamSocket();

                HostName = new HostName("localhost");

                await ClientSocket.ConnectAsync(HostName, Port);

                string request = "더미데이터";

                using (Stream outputStream = ClientSocket.OutputStream.AsStreamForWrite())
                {
                    using (var streamWriter = new StreamWriter(outputStream))
                    {
                        await streamWriter.WriteLineAsync(request);
                        await streamWriter.FlushAsync();
                    }
                }

                // Read data from the echo server.
                string response;
                using (Stream inputStream = ClientSocket.InputStream.AsStreamForRead())
                {
                    using (StreamReader streamReader = new StreamReader(inputStream))
                    {
                        response = await streamReader.ReadLineAsync();
                    }
                }

                Result = response;
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                //this.clientListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }

        public static void StopClient()
        {
            try
            {
                ClientSocket.Dispose();
            }
            catch (Exception ex)
            {
               SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                //this.clientListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }
    }
}
