using GGTClient.Events;
using GGTClient.Helpers;
using GGTClient.Models;
using GGTClient.Services;
using GGTClient.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace GGTClient.Views
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MediaPlayer Player { get; set; } = new MediaPlayer();

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            Singleton<CommunicationService>.Instance.HubConnectionErrorFired += CommunicationService_HubConnectionErrorFired;
            Singleton<CommunicationService>.Instance.HubConnectionConnected += CommunicationService_HubConnectionConnected;
            Singleton<CommunicationService>.Instance.HubConnectionConnecting += CommunicationService_HubConnectionConnecting;
            Singleton<CommunicationService>.Instance.HubConnectionDisconnected += CommunicationService_HubConnectionDisconnected;
            Singleton<CommunicationService>.Instance.Packet0003Received += CommunicationService_Packet0003Received;
            Singleton<CommunicationService>.Instance.Packet0004Received += CommunicationService_Packet0004Received;
            Singleton<CommunicationService>.Instance.Packet0006Received += CommunicationService_Packet0006Received;
        }

        private async void CommunicationService_Packet0003Received(object sender, Packet0003ReceivedEventArgs e)
        {
            if (e.IsLoginSuccess)
            {
                Player.Pause();
                Player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/LoginSuccessNotification.wav"));
                Player.IsLoopingEnabled = false;
                Player.Play();

                ProgressRing_Information.IsActive = false;
                TextBlock_Message.Text = "로그인에 성공하였습니다";
                NotificationStoryboard.RepeatBehavior = new RepeatBehavior(1);
                NotificationStoryboard.Begin();
                OnLoginSuccessStoryboard.Begin();
            }
            else
            {
                OnLoginFailedStoryboard.Begin();
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "로그인 실패",
                    Content = $"{e.Message}",
                    CloseButtonText = "닫기",
                    DefaultButton = ContentDialogButton.Close
                };
                dialog.Loading += async (send, args) => await this.Blur(value: 5, duration: 1000, delay: 0).StartAsync();
                dialog.Closing += async (send, args) => await this.Blur(value: 0, duration: 500, delay: 0).StartAsync();
                await dialog.ShowAsync();
            }
        }

        private async void CommunicationService_Packet0004Received(object sender, Packet0004ReceivedEventArgs e)
        {
            if (e.IsLogoutSuccess)
            {
                ProgressRing_Information.IsActive = false;
                TextBlock_Message.Text = "로그아웃에 성공하였습니다";
                NotificationStoryboard.RepeatBehavior = new RepeatBehavior(1);
                NotificationStoryboard.Begin();
                OnLogoutSuccessStoryboard.Begin();
            }
            else
            {
                OnLogoutFailedStoryboard.Begin();
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "로그아웃 실패",
                    Content = $"{e.Message}",
                    CloseButtonText = "닫기",
                    DefaultButton = ContentDialogButton.Close
                };
                dialog.Loading += async (send, args) => await this.Blur(value: 5, duration: 1000, delay: 0).StartAsync();
                dialog.Closing += async (send, args) => await this.Blur(value: 0, duration: 500, delay: 0).StartAsync();
                await dialog.ShowAsync();
            }
        }

        private async void CommunicationService_Packet0006Received(object sender, Packet0006ReceivedEventArgs e)
        {

            if (e.IsMoved)
            {
                if (e.SendFrom.Equals(ViewModel.UserName))
                {
                    if (e.NewGroupName.Equals(CurrentLocation.WaitingRoom.ToString()))
                    {                        
                        Frame.Navigate(typeof(WaitingRoomPage), new UserInfo() { UserId = ViewModel.UserID, UserPassword = ViewModel.UserPassword, UserName = ViewModel.UserName }, new EntranceNavigationTransitionInfo());
                    }
                }
            }
            else
            {
                if (e.SendFrom.Equals(ViewModel.UserName))
                {
                    if (e.Request.NewGroupName.Equals(CurrentLocation.Init.ToString()) & e.Request.ExpectedOldGroupName.Equals(CurrentLocation.None.ToString()))
                    {

                    }
                    else
                    {
                        OnLogoutFailedStoryboard.Begin();
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
        }

        private async void CommunicationService_HubConnectionConnected(object sender, HubConnectionConnectedEventArgs e)
        {
            Player.Pause();
            Player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/Windows Proximity Notification.wav"));
            Player.IsLoopingEnabled = false;
            Player.Play();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ProgressRing_Information.IsActive = false;
                TextBlock_Message.Text = "GGT 서버에 연결되었습니다";
                OnConnectedStoryboard.Begin();
                NotificationStoryboard.RepeatBehavior = new RepeatBehavior(1);
                NotificationStoryboard.Begin();
            });
        }

        private async void CommunicationService_HubConnectionDisconnected(object sender, HubConnectionDisconnectedEventArgs e)
        {
            Player.Pause();
            Player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/Windows Device Connection Removed.wav"));
            Player.IsLoopingEnabled = false;
            Player.Play();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ProgressRing_Information.IsActive = false;
                OnDisconnectedStoryboard.Begin();
                TextBlock_Message.Text = "GGT 서버와의 연결이 끊어졌습니다";
                NotificationStoryboard.RepeatBehavior = new RepeatBehavior(1);
                NotificationStoryboard.Begin();
            });
        }

        private async void CommunicationService_HubConnectionConnecting(object sender, HubConnectionConnectingEventArgs e)
        {
            Player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/Windows Proximity Connection.wav"));
            Player.IsLoopingEnabled = true;
            Player.Play();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ProgressRing_Information.IsActive = true;
                TextBlock_Message.Text = "서버에 연결중입니다";
                OnConnectingStoryboard.Begin();
                NotificationStoryboard.RepeatBehavior = RepeatBehavior.Forever;
                NotificationStoryboard.Begin();
                Button_Connect.IsEnabled = true;
            });
        }

        private void CommunicationService_HubConnectionErrorFired(object sender, HubConnectionErrorFiredEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                btn.IsEnabled = false;
            }
        }

        private async void Button_AccountRegister_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                AccountRegisterContentDialog dialog = new AccountRegisterContentDialog();
                dialog.Loading += async (send, args) => await this.Blur(value: 5, duration: 1000, delay: 0).StartAsync();
                dialog.Closing += async (send, args) => await this.Blur(value: 0, duration: 500, delay: 0).StartAsync();
                await dialog.ShowAsync();
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("WaitingroomTextBlockAnimation", TextBlock_UserName);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("WaitingroomTextBlockBackAnimation");
            if (animation != null)
            {
                animation.TryStart(TextBlock_UserName);
            }
            if (e.NavigationMode == NavigationMode.Back)
            {
                OnLogoutFailedStoryboard.Begin();
                ViewModel.LogoutEnable = true;
            }
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                OnLoginTryingStoryboard.Begin();
            }
        }

        private void Button_Logout_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                OnLogoutTryingStoryboard.Begin();
            }
        }

        private void Button_Entrance_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                OnLogoutTryingStoryboard.Begin();
            }
        }
    }
}
