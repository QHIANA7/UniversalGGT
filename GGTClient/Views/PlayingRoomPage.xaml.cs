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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
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
    public sealed partial class PlayingRoomPage : Page
    {

        public PlayingRoomViewModel ViewModel { get; } = new PlayingRoomViewModel();

        public PlayingRoomPage()
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is String extra_location)
            {
                ViewModel.LocationName += extra_location;
            }

            ConnectedAnimation animation_textblock = ConnectedAnimationService.GetForCurrentView().GetAnimation("WaitingRoomToPlayingRoomTextBlockConnectedAnimation");
            if (animation_textblock != null)
            {
                animation_textblock.TryStart(TextBlock_UserName);
            }

            RegisterEvent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.SourcePageType.Equals(typeof(WaitingRoomPage)))
            {

            }
        }

        private async void CommunicationService_Packet0006Received(object sender, Packet0006ReceivedEventArgs e)
        {
            if (e.IsMoved)
            {
                if (e.SendFrom.Equals(Singleton<GGTService>.Instance.UserName))
                {
                    if (e.NewGroupName.Equals(CurrentLocation.WaitingRoom.ToString()))
                    {
                        if (this.Frame.CanGoBack)
                        {
                            //Singleton<CommunicationService>.Instance.Packet0006Received -= CommunicationService_Packet0006Received;
                            ConnectedAnimationService service = ConnectedAnimationService.GetForCurrentView();
                            service.PrepareToAnimate("PlayingRoomToWaitingRoomTextBlockConnectedAnimation", TextBlock_UserName);
                            UnregisterEvent();
                            this.Frame.GoBack();
                        }
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

        private void TextBox_Message_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.MessageSend?.Execute(null);
                ViewModel.Message = String.Empty;
            }
        }

    }
}
