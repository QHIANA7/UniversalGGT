using GGTClient.Events;
using GGTClient.Models;
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

        public IHubProxy GGTHubProxy = null;

        public ViewModels.MainViewModel MainViewModel_Instance { get; set; } = null;

        public event EventHandler<HubConnectionErrorFiredEventArgs> HubConnectionErrorFired = null;
        public event EventHandler<HubConnectionConnectedEventArgs> HubConnectionConnected = null;
        public event EventHandler<HubConnectionConnectingEventArgs> HubConnectionConnecting = null;
        public event EventHandler<HubConnectionDisconnectedEventArgs> HubConnectionDisconnected = null;
        public event EventHandler<IdCheckAppliedEventArgs> IdCheckApplied = null;

        public void StartClient()
        {
            Connection = new HubConnection("http://ggtsvr.azurewebsites.net/");
            GGTHubProxy = Connection.CreateHubProxy("GGTHub");
            Connection.Error += Connection_Error;
            Connection.StateChanged += Connection_StateChanged;
            GGTHubProxy.On<String>("ResponseLogin", ResponseLogin);
            Connection.Start();
        }

        private void Connection_StateChanged(StateChange obj)
        {
            switch (obj.NewState)
            {
                case ConnectionState.Connecting:
                    HubConnectionConnecting?.Invoke(this, new HubConnectionConnectingEventArgs(DateTime.Now, Connection));
                    break;
                case ConnectionState.Connected:
                    HubConnectionConnected?.Invoke(this, new HubConnectionConnectedEventArgs(DateTime.Now, Connection));
                    break;
                case ConnectionState.Reconnecting:
                    break;
                case ConnectionState.Disconnected:
                    HubConnectionDisconnected?.Invoke(this, new HubConnectionDisconnectedEventArgs(DateTime.Now, Connection));
                    break;
                default:
                    break;
            }
        }

        private void Connection_Error(Exception ex)
        {
            Connection.Stop();
            HubConnectionErrorFired?.Invoke(this, new HubConnectionErrorFiredEventArgs(DateTime.Now, ex, Connection));
        }


        public void Send(String msg)
        {
            string request = msg;

            GGTHubProxy.Invoke("Send", new object[] { "william", msg });
        }

        public void RequestLogin(String id, String pw)
        {
            GGTHubProxy.Invoke("RequestLogin", new object[] { id, pw });
        }

        public async void RequestIdCheck(String id)
        {
            Req0001 req = new Req0001() { Id = id, RequestTime = DateTime.Now };
            Res0001 res = await GGTHubProxy.Invoke<Res0001>("RequestIdCheck", req);

            IdCheckApplied?.Invoke(this, new IdCheckAppliedEventArgs(req, res));
        }

        public void ConnectionCheck()
        {
            GGTHubProxy.Invoke("ConnectionCheck", new object[] { "data" });
        }

        private async void ResponseLogin(String username)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MainViewModel_Instance.UserName = username;
            });
        }

        #endregion
    }
}
