using GGTClient.Events;
using GGTClient.Helpers;
using GGTClient.Models;
using GGTClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GGTClient.ViewModels
{
    public class MainViewModel : Observable
    {
        private String _user_id = String.Empty;
        public String UserId
        {
            get { return _user_id; }
            set { Set(ref _user_id, value); }
        }

        private String _user_pw = String.Empty;
        public String UserPassword
        {
            get { return _user_pw; }
            set { Set(ref _user_pw, value); }
        }

        private String _user_name = String.Empty;
        public String UserName
        {
            get { return _user_name; }
            set { Set(ref _user_name, value); }
        }

        private String _message = String.Empty;
        public String Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        private String _group_name = String.Empty;
        public String GroupName
        {
            get { return _group_name; }
            set { Set(ref _group_name, value); }
        }

        private Boolean _login_enable = true;
        public Boolean LoginEnable
        {
            get { return _login_enable; }
            set { Set(ref _login_enable, value); }
        }

        private Boolean _logout_enable = true;
        public Boolean LogoutEnable
        {
            get { return _logout_enable; }
            set { Set(ref _logout_enable, value); }
        }

        private ICommand _login;
        public ICommand Login
        {
            get
            {
                if (_login == null)
                {
                    _login = new RelayCommand(
                        () =>
                        {
                            Singleton<CommunicationService>.Instance.RequestLogin(UserId, UserPassword);
                            LoginEnable = false;
                        });
                }
            
                return _login;
            }
        }

        private ICommand _logout;
        public ICommand Logout
        {
            get
            {
                if (_logout == null)
                {
                    _logout = new RelayCommand(
                        () =>
                        {
                            Singleton<CommunicationService>.Instance.RequestLogout(UserId);
                            LogoutEnable = false;
                        });
                }
                return _logout;
            }
        }

        private ICommand _entrance;
        public ICommand Entrance
        {
            get
            {
                if (_entrance == null)
                {
                    _entrance = new RelayCommand(
                        () =>
                        {
                            Singleton<CommunicationService>.Instance.RequestLeaveGroup(UserId, GroupName);
                            Task.Delay(1000);
                            Singleton<CommunicationService>.Instance.RequestJoinGroup(UserId, CurrentLocation.WaitingRoom.ToString());
                            LogoutEnable = false;
                        });
                }

                return _entrance;
            }
        }

        private ICommand _connect;
        public ICommand Connect
        {
            get
            {
                if (_connect == null)
                {
                    _connect = new RelayCommand(
                        () =>
                        {
                            //user = new UserInfo(UserId, UserPassword);

                            Singleton<CommunicationService>.Instance.MainViewModel_Instance = this;
                            Singleton<CommunicationService>.Instance.StartClient();
                            //UserPassword = Singleton<CommunicationService>.Instance.Result;
                        });
                }

                return _connect;
            }
        }

        public MainViewModel()
        {
            Singleton<CommunicationService>.Instance.Packet0003Received += CommunicationService_Packet0003Received;
            Singleton<CommunicationService>.Instance.Packet0004Received += CommunicationService_Packet0004Received;
        }

        private void CommunicationService_Packet0003Received(object sender, Packet0003ReceivedEventArgs e)
        {
            UserName = e.UserName;
            if(e.IsLoginSuccess)
            {
                LoginEnable = false;
                LogoutEnable = true;
            }
            else
            {
                LoginEnable = true;
            }
        }

        private void CommunicationService_Packet0004Received(object sender, Packet0004ReceivedEventArgs e)
        {
            //UserName = String.Empty;
            if (e.IsLogoutSuccess)
            {
                LogoutEnable = false;
                LoginEnable = true;
                UserId = String.Empty;
                UserPassword = String.Empty;
            }
            else
            {
                LogoutEnable = true;
            }
        }
    }
}
