﻿using GGTServer.Helpers;
using GGTServer.Models;
using GGTServer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GGTServer.ViewModels
{
    public class MainViewModel : Observable
    {
        public ObservableCollection<ConnectedClientInfo> ConnectedClientList { get; set; } = new ObservableCollection<ConnectedClientInfo>();

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

        private ICommand _login;
        public ICommand Login
        {
            get
            {
                if (_login == null)
                {
                    _login = new RelayCommand(
                        async () =>
                        {
                            //user = new UserInfo(UserId, UserPassword);
                            await CommunicationService.StartServer();

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
                             CommunicationService.StopServer();
                         });
                }

                return _logout;
            }
        }

        public MainViewModel()
        {

        }
    }
}
