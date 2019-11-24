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
    public class PlayingRoomViewModel : Observable
    {
        public ObservableCollection<MessageInfo> MessageList = new ObservableCollection<MessageInfo>();
        public ObservableCollection<UserInfo> EntryList = new ObservableCollection<UserInfo>();

        private String _location_name = CurrentLocation.PlayingRoom.ToString();
        public String LocationName
        {
            get { return _location_name; }
            set { Set(ref _location_name, value); }
        }

        private String _user_name = Singleton<GGTService>.Instance.UserName;
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

        private ICommand _ready_or_start;
        public ICommand ReadyOrStart
        {
            get
            {
                if (_ready_or_start == null)
                {
                    _ready_or_start = new RelayCommand(
                        () =>
                        {

                        });
                }

                return _ready_or_start;
            }
        }

        private ICommand _to_waiting_room;
        public ICommand ToWaitingRoom
        {
            get
            {
                if (_to_waiting_room == null)
                {
                    _to_waiting_room = new RelayCommand(
                        () =>
                        {
                            Singleton<CommunicationService>.Instance.RequestSendMessage(Singleton<GGTService>.Instance.UserId, Singleton<GGTService>.Instance.UserName, $"{Singleton<GGTService>.Instance.UserName}님이 떠났습니다", true);
                            Singleton<CommunicationService>.Instance.RequestMoveGroup(Singleton<GGTService>.Instance.UserId, CurrentLocation.WaitingRoom.ToString(), LocationName);
                        });
                }

                return _to_waiting_room;
            }
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
                                Singleton<CommunicationService>.Instance.RequestSendMessage(Singleton<GGTService>.Instance.UserId, Singleton<GGTService>.Instance.UserName, Message);
                        });
                }

                return _message_send;
            }
        }

        public PlayingRoomViewModel()
        {
            //Singleton<CommunicationService>.Instance.RequestGetUserList(Singleton<GGTService>.Instance.UserId);
            Singleton<CommunicationService>.Instance.Packet0005Received += CommunicationService_Packet0005Received;
        }

        private void CommunicationService_Packet0005Received(object sender, Packet0005ReceivedEventArgs e)
        {
            MessageList.Add(new MessageInfo(e.RequestTime, e.SendFrom, e.Message, e.SendFrom.Equals(Singleton<GGTService>.Instance.UserName), e.IsSystemMessage));
        }
    }
}
