using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.UI.Core;

namespace GGTClient.Services
{
    public static class CommunicationService
    {
        public static StreamSocket ClientSocket { get; set; } = null;

        public static HostName HostName { get; set; } = null;

        public static String Result { get; set; } = String.Empty;

        public static String Port { get; set; } = "63000";

        public static List<String> Lists { get; set; } = new List<string>();

        public static ViewModels.MainViewModel MainViewModel_Instance { get; set; } = null;

        public static async void ComAction()
        {
            // Read data from the echo server.
            string response;
            using (Stream inputStream = ClientSocket.InputStream.AsStreamForRead())
            {
                using (StreamReader streamReader = new StreamReader(inputStream))
                {
                    response = await streamReader.ReadLineAsync();
                }
            }

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MainViewModel_Instance.UserPassword = response;
                //write your code
                //in OnPropertyChanged use PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            });

        }

        public static async Task StartClient(ViewModels.MainViewModel viewmodel)
        {
            try
            {
                //야매니까 고려해보자.
                MainViewModel_Instance = viewmodel;
           

                // Create the StreamSocket and establish a connection to the echo server.
                ClientSocket = new StreamSocket();

                HostName = new HostName("192.168.0.58");

                await ClientSocket.ConnectAsync(HostName, Port);

                await Task.Run(() => ComAction());

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

        public async static void Send(String msg)
        {
            string request = msg;

            using (Stream outputStream = ClientSocket.OutputStream.AsStreamForWrite())
            {
                using (var streamWriter = new StreamWriter(outputStream))
                {
                    await streamWriter.WriteLineAsync(request);
                    await streamWriter.FlushAsync();
                }
            }
        }
    }
}
