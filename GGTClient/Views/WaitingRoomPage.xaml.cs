using GGTClient.Events;
using GGTClient.Helpers;
using GGTClient.Models;
using GGTClient.Services;
using GGTClient.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace GGTClient.Views
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class WaitingRoomPage : Page
    {
        public WaitingRoomViewModel ViewModel { get; } = new WaitingRoomViewModel();

        public WaitingRoomPage()
        {
            this.InitializeComponent();
        }

        private void RegisterEvent()
        {
            Singleton<CommunicationService>.Instance.Packet0006Received += CommunicationService_Packet0006Received;
        }

        private void UnregisterEvent()
        {
            Singleton<CommunicationService>.Instance.Packet0006Received -= CommunicationService_Packet0006Received;
        }



        private async void CommunicationService_Packet0006Received(object sender, Packet0006ReceivedEventArgs e)
        {
            if (e.IsMoved)
            {
                if (e.SendFrom.Equals(Singleton<GGTService>.Instance.UserName))
                {
                    if (e.NewGroupName.Equals(CurrentLocation.Init.ToString()))
                    {
                        if (this.Frame.CanGoBack)
                        {
                            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("WaitingRoomToInitTextBlockConnectedAnimation", TextBlock_UserName);
                            this.NavigationCacheMode = NavigationCacheMode.Disabled;
                            UnregisterEvent();
                            this.Frame.GoBack();
                        }
                    }
                    else if (e.NewGroupName.Contains(CurrentLocation.PlayingRoom.ToString()))
                    {
                        GridView_Rooms.IsEnabled = true;
                        String extra_location = e.NewGroupName.Replace(CurrentLocation.PlayingRoom.ToString(), String.Empty);
                        ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("WaitingRoomToPlayingRoomTextBlockConnectedAnimation", TextBlock_UserName);
                        this.NavigationCacheMode = NavigationCacheMode.Enabled;
                        UnregisterEvent();
                        this.Frame.Navigate(typeof(PlayingRoomPage), extra_location, new EntranceNavigationTransitionInfo());
                    }
                }
            }
            else
            {
                if (e.SendFrom.Equals(Singleton<GGTService>.Instance.UserName))
                {

                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "입장 실패",
                        Content = $"{e.Message}",
                        CloseButtonText = "닫기",
                        DefaultButton = ContentDialogButton.Close
                    };
                    dialog.Loading += async (send, args) => await this.Blur(value: 5, duration: 1000, delay: 0).StartAsync();
                    dialog.Closing += async (send, args) => await this.Blur(value: 0, duration: 500, delay: 0).StartAsync();
                    await dialog.ShowAsync();
                }
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ConnectedAnimationService service = ConnectedAnimationService.GetForCurrentView();
            service.GetAnimation("InitToWaitingRoomTextBlockConnectedAnimation")?.TryStart(TextBlock_UserName);
            service.GetAnimation("PlayingRoomToWaitingRoomTextBlockConnectedAnimation")?.TryStart(TextBlock_UserName);

            RegisterEvent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {

            }

        }

        private void TextBox_Message_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.MessageSend?.Execute(null);
                ViewModel.Message = String.Empty;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //ViewModel.RoomList.Clear();
            //for (Int32 i = 0; i < 100; i++)
            //{
            //    ViewModel.RoomList.Add(new RoomInfo() { RoomNumber = i, MaxJoinCount = 10, CurrentJoinCount = 1, RoomMaster = "정성용", RoomTitle = "방제목 표시" });
            //}
        }

        #region Interop Animation

        private void RoomGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Grid room)
            {
                room.Background = new SolidColorBrush(Colors.LightPink);
            }
        }

        private void RoomGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Grid room)
            {
                room.Background = new SolidColorBrush(Colors.LightBlue);
            }
        }

        #endregion

        private async void Button_MakeRoom_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                MakeRoomContentDialog dialog = new MakeRoomContentDialog();
                dialog.Loading += async (send, args) => await this.Blur(value: 5, duration: 1000, delay: 0).StartAsync();
                dialog.Closing += async (send, args) => await this.Blur(value: 0, duration: 500, delay: 0).StartAsync();
                await dialog.ShowAsync();
            }
        }
    }
}
