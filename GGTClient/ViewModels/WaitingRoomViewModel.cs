﻿using GGTClient.Helpers;
using GGTClient.Models;
using GGTClient.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GGTClient.ViewModels
{
    public class WaitingRoomViewModel : Observable
    {
        public ObservableCollection<MessageInfo> MessageList = new ObservableCollection<MessageInfo>();

        private String _user_id = String.Empty;
        public String UserId
        {
            get { return _user_id; }
            set { Set(ref _user_id, value); }
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

        private String _message_box = String.Empty;
        public String MessageBox
        {
            get { return _message_box; }
            set { Set(ref _message_box, value); }
        }

        private ICommand _message_send;
        public ICommand MessageSend
        {
            get
            {
                if (_message_send == null)
                {
                    _message_send = new RelayCommand(
                        () =>
                        {
                            Singleton<CommunicationService>.Instance.RequestSendMessage(UserId, UserName, Message);
                        });
                }

                return _message_send;
            }
        }

        public WaitingRoomViewModel()
        {
            Singleton<CommunicationService>.Instance.Packet0005Received += CommunicationService_Packet0005Received;
        }

        private void CommunicationService_Packet0005Received(object sender, Events.Packet0005ReceivedEventArgs e)
        {
            if (e.SendFrom.Equals(UserName))
                MessageList.Add(new MessageInfo(e.RequestTime, e.SendFrom, e.Response.Message, true));
            else
                MessageList.Add(new MessageInfo(e.RequestTime, e.SendFrom, e.Response.Message, false));
        }


    }
}