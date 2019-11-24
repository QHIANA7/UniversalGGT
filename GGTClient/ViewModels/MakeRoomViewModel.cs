using GGTClient.Events;
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
    public class MakeRoomViewModel : Observable
    {
        private String _room_title_placeholder = String.Empty;
        public String RoomTitlePlaceHolder
        {
            get { return _room_title_placeholder; }
            set { Set(ref _room_title_placeholder, value); }
        }

        private String _room_title = String.Empty;
        public String RoomTitle
        {
            get { return _room_title; }
            set { Set(ref _room_title, value); }
        }

        private String _access_password = String.Empty;
        public String AccessPassword
        {
            get { return _access_password; }
            set { Set(ref _access_password, value); }
        }

        private Double _max_join_cnt = 2.0;
        public Double MaxJoinCount
        {
            get { return _max_join_cnt; }
            set { Set(ref _max_join_cnt, value); }
        }

        private ICommand _make_room;
        public ICommand MakeRoom
        {
            get
            {
                if (_make_room == null)
                {
                    _make_room = new RelayCommand(
                        () =>
                        {
                            Singleton<CommunicationService>.Instance.RequestMakeRoom(Singleton<GGTService>.Instance.UserId, RoomTitle, AccessPassword, MaxJoinCount);
                        });
                }

                return _make_room;
            }
        }

        public MakeRoomViewModel()
        {
            Singleton<CommunicationService>.Instance.Packet0008Received += CommunicationService_Packet0008Received;
        }

        private async void CommunicationService_Packet0008Received(object sender, Packet0008ReceivedEventArgs e)
        {
            if (!e.IsCreated)
            {
                Windows.UI.Xaml.Controls.ContentDialog dialog = new Windows.UI.Xaml.Controls.ContentDialog()
                {
                    Title = "방만들기 실패",
                    Content = $"{e.Request.UserID} {Environment.NewLine}{e.Message}",
                    CloseButtonText = "닫기",
                    DefaultButton = Windows.UI.Xaml.Controls.ContentDialogButton.Close
                };
                switch (await dialog.ShowAsync())
                {
                    case Windows.UI.Xaml.Controls.ContentDialogResult.Primary:

                        break;
                }
            }
        }
    }
}