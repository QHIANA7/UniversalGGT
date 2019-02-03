using GGTClient.Helpers;
using GGTClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GGTClient.ViewModels
{
    public class WaitingRoomViewModel : Observable
    {
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
            
        }

    }
}
