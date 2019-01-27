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

        private String _message = String.Empty;
        public String Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        private ICommand _login;
        public ICommand Login
        {
            get
            {
                if (_login == null)
                {
                    _login = new RelayCommand(
                        async() =>
                        {
                            //user = new UserInfo(UserId, UserPassword);
                            await CommunicationService.StartClient(this);

                            UserPassword = CommunicationService.Result;
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
                            //user = new UserInfo(UserId, UserPassword);
                             CommunicationService.StopClient();
                        });
                }

                return _logout;
            }
        }

        private ICommand _msg_send;
        public ICommand MessageSend
        {
            get
            {
                if (_msg_send == null)
                {
                    _msg_send = new RelayCommand(
                         () =>
                         {
                             //user = new UserInfo(UserId, UserPassword);
                             CommunicationService.Send(Message);
                         });
                }

                return _msg_send;
            }
        }

        public MainViewModel()
        {

        }
    }
}
