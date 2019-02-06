using GGTClient.Events;
using GGTClient.Helpers;
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
        public ObservableCollection<UserInfo> UserList = new ObservableCollection<UserInfo>();

        private String _user_id = String.Empty;
        public String UserID
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

        private String _location_name = CurrentLocation.WaitingRoom.ToString();
        public String LocationName
        {
            get { return _location_name; }
            set { Set(ref _location_name, value); }
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

        private Boolean _toinit_enable = true;
        public Boolean ToInitEnable
        {
            get { return _toinit_enable; }
            set { Set(ref _toinit_enable, value); }
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
                            if (!String.IsNullOrWhiteSpace(Message))
                                Singleton<CommunicationService>.Instance.RequestSendMessage(UserID, UserName, Message);
                        });
                }

                return _message_send;
            }
        }

        private ICommand _to_Init;
        public ICommand ToInit
        {
            get
            {
                if (_to_Init == null)
                {
                    _to_Init = new RelayCommand(
                        () =>
                        {
                            ToInitEnable = false;
                            Singleton<CommunicationService>.Instance.RequestSendMessage(UserID, UserName, $"{UserName}님이 떠났습니다", true);
                            Singleton<CommunicationService>.Instance.RequestMoveGroup(UserID, CurrentLocation.Init.ToString(), LocationName);
                        });
                }

                return _to_Init;
            }
        }

        public WaitingRoomViewModel()
        {
            Singleton<CommunicationService>.Instance.RequestGetUserList(UserID);
            Singleton<CommunicationService>.Instance.Packet0005Received += CommunicationService_Packet0005Received;
            Singleton<CommunicationService>.Instance.Packet0006Received += CommunicationService_Packet0006Received;
            Singleton<CommunicationService>.Instance.Packet0007Received += CommunicationService_Packet0007Received;
        }

        private void CommunicationService_Packet0005Received(object sender, Packet0005ReceivedEventArgs e)
        {
            MessageList.Add(new MessageInfo(e.RequestTime, e.SendFrom, e.Message, e.SendFrom.Equals(UserName), e.IsSystemMessage));
        }

        private void CommunicationService_Packet0006Received(object sender, Packet0006ReceivedEventArgs e)
        {
            if (e.IsMoved)
            {
                if (e.NewGroupName.Equals(CurrentLocation.Init.ToString()) & e.SendFrom.Equals(UserName))
                {
                    Singleton<CommunicationService>.Instance.Packet0005Received -= CommunicationService_Packet0005Received;
                    Singleton<CommunicationService>.Instance.Packet0006Received -= CommunicationService_Packet0006Received;
                    Singleton<CommunicationService>.Instance.Packet0007Received -= CommunicationService_Packet0007Received;
                }

                if(!e.SendFrom.Equals(UserName))
                {
                    UserInfo target_user = UserList.FirstOrDefault<UserInfo>(x => x.UserName.Equals(e.SendFrom));
                    CurrentLocation location = (CurrentLocation)Enum.Parse(typeof(CurrentLocation), e.NewGroupName);
                    if (target_user == null)
                    {                        
                        UserList.Add(new UserInfo() { UserName = e.SendFrom, Location = location });
                    }
                    else
                    {
                        UserList.Remove(target_user);
                        UserList.Add(new UserInfo() { UserName = e.SendFrom, Location = location });
                    }
                }
            }
        }

        private void CommunicationService_Packet0007Received(object sender, Packet0007ReceivedEventArgs e)
        {
            UserList.Clear();

            if (e.UserList != null)
            {
                foreach (UserInfo user in e.UserList)
                {
                    UserList.Add(user);
                }
            }
        }
    }
}
