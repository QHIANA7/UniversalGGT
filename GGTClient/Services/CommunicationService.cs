using GGTClient.Events;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.UI.Core;

namespace GGTClient.Services
{
    public class CommunicationService
    {
        #region StreamSocket 사용

        //public static StreamSocket ClientSocket { get; set; } = null;

        //public static HostName HostName { get; set; } = null;

        //public static String Result { get; set; } = String.Empty;

        //public static String Port { get; set; } = "63000";

        //public static List<String> Lists { get; set; } = new List<string>();

        //public static ViewModels.MainViewModel MainViewModel_Instance { get; set; } = null;

        //public static async void ComAction()
        //{
        //    try
        //    {
        //        while (true)
        //        {
        //            // Read data from the echo server.
        //            string response;
        //            using (Stream inputStream = ClientSocket.InputStream.AsStreamForRead())
        //            {
        //                using (StreamReader streamReader = new StreamReader(inputStream))
        //                {
        //                    response = await streamReader.ReadLineAsync();
        //                }
        //            }

        //            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //            {
        //                MainViewModel_Instance.UserPassword = response;
        //                //write your code
        //                //in OnPropertyChanged use PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
        //        //this.clientListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
        //    }
        //}

        //public static async Task StartClient(ViewModels.MainViewModel viewmodel)
        //{

        //    try
        //    {
        //        //야매니까 고려해보자.
        //        MainViewModel_Instance = viewmodel;


        //        // Create the StreamSocket and establish a connection to the echo server.
        //        ClientSocket = new StreamSocket();

        //        HostName = new HostName("192.168.0.55");

        //        await ClientSocket.ConnectAsync(HostName, Port);

        //        await Task.Run(() => ComAction());

        //    }
        //    catch (Exception ex)
        //    {
        //        SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
        //        //this.clientListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
        //    }


        //}

        //public static void StopClient()
        //{
        //    try
        //    {
        //        ClientSocket.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //       SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
        //        //this.clientListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
        //    }
        //}

        //public async static void Send(String msg)
        //{
        //    string request = msg;

        //    using (Stream outputStream = ClientSocket.OutputStream.AsStreamForWrite())
        //    {
        //        using (var streamWriter = new StreamWriter(outputStream))
        //        {
        //            await streamWriter.WriteLineAsync(request);
        //            await streamWriter.FlushAsync();
        //        }
        //    }
        //}

        #endregion

        #region SignalR 사용

        public HubConnection Connection { get; set; }

        public IHubProxy myHubProxy = null;

        public ViewModels.MainViewModel MainViewModel_Instance { get; set; } = null;

        public event EventHandler<HubConnectionErrorFiredEventArgs> HubConnectionErrorFiredInfo = null;

        public async void StartClient()
        {
            // init hub connection with url ...
            Connection = new HubConnection("http://ggtsvr.azurewebsites.net/");
            myHubProxy = Connection.CreateHubProxy("GGTHub");
            Connection.Error += Connection_Error;
            Connection.StateChanged += Connection_StateChanged;

            // attach event handler from server sent message.
            myHubProxy.On<string, string>("addMessage", _OnAddMessage);
            myHubProxy.On<string>("showMsg", _OnShowMsg);
            myHubProxy.On<String>("ResponseLogin", OnResponseLogin);
            myHubProxy.On<String>("OnConnectionCheckResponse", OnConnectionCheckResponse);

            // retry connection every 3 seconds ...
            while (Connection.State != ConnectionState.Connected)
            {
                try
                {
                    await Connection.Start();

                }
                catch (Exception)
                {
                    Console.WriteLine(@"
connection failed !
sleep 3 sec ...
try reconnect ...
                ");

                    Task.Delay(3000).Wait();
                }
            }
        }

        private void Connection_StateChanged(StateChange obj)
        {
            switch (obj.NewState)
            {
                case ConnectionState.Connecting:
                    break;
                case ConnectionState.Connected:
                    break;
                case ConnectionState.Reconnecting:
                    break;
                case ConnectionState.Disconnected:
                    break;
                default:
                    break;
            }
        }

        private void Connection_Error(Exception ex)
        {
            HubConnectionErrorFiredInfo?.Invoke(this, new HubConnectionErrorFiredEventArgs(DateTime.Now, ex, Connection));
        }

        private void _OnShowMsg(string msg)
        {
            //Console.WriteLine($"RECV showMsg : {msg}");
        }

        private async void _OnAddMessage(string name, string message)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MainViewModel_Instance.UserPassword = message;
            });
        }

        public void Send(String msg)
        {
            string request = msg;

            myHubProxy.Invoke("Send", new object[] { "william", msg });
        }

        public void RequestLogin(String id, String pw)
        {
            myHubProxy.Invoke("RequestLogin", new object[] { id, pw });
        }

        public void ConnectionCheck()
        {
            myHubProxy.Invoke("ConnectionCheck", new object[] { "data" });
        }

        private async void OnConnectionCheckResponse(String data)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MainViewModel_Instance.UserName = data;
                Views.MainPage.Current.Connected();
            });
        }

        private async void OnResponseLogin(String username)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MainViewModel_Instance.UserName = username;
            });
        }

        //public static async void ComAction()
        //{
        //    try
        //    {
        //        while (true)
        //        {
        //            // Read data from the echo server.
        //            string response;
        //            using (Stream inputStream = ClientSocket.InputStream.AsStreamForRead())
        //            {
        //                using (StreamReader streamReader = new StreamReader(inputStream))
        //                {
        //                    response = await streamReader.ReadLineAsync();
        //                }
        //            }

        //            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //            {
        //                MainViewModel_Instance.UserPassword = response;
        //                //write your code
        //                //in OnPropertyChanged use PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
        //        //this.clientListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
        //    }
        //}

        //        private static void _RunServerLoop(IHubProxy myHubProxy)
        //        {
        //            while (true)
        //            {
        //                Console.WriteLine(@"
        //commands
        //------------------------------------
        //Q : quit
        //S : Send(""william"", ""hello"")
        //T : StartTimer(10)
        //            ");

        //                var cmd = Console.ReadKey();

        //                if (cmd.Key == ConsoleKey.Q)
        //                {
        //                    break;
        //                }
        //                else if (cmd.Key == ConsoleKey.S)
        //                {
        //                    myHubProxy.Invoke("Send", new object[] { "william", "hello" });
        //                }
        //                else if (cmd.Key == ConsoleKey.T)
        //                {
        //                    myHubProxy.Invoke("StartTimer", new object[] { 10 });
        //                }
        //            }
        //        }

        #endregion
    }
}
