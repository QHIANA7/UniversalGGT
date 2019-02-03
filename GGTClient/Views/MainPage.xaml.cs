using GGTClient.Events;
using GGTClient.Helpers;
using GGTClient.Services;
using GGTClient.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI.Xaml.Media.Animation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace GGTClient.Views
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public static MainPage Current { get; set; }

        public MediaPlayer Player { get; set; } = new MediaPlayer();

        public MainPage()
        {
            this.InitializeComponent();

            Current = this;
            Singleton<CommunicationService>.Instance.HubConnectionErrorFired += CommunicationService_HubConnectionErrorFired;
            Singleton<CommunicationService>.Instance.HubConnectionConnected += CommunicationService_HubConnectionConnected;
            Singleton<CommunicationService>.Instance.HubConnectionConnecting += CommunicationService_HubConnectionConnecting;
            Singleton<CommunicationService>.Instance.HubConnectionDisconnected += CommunicationService_HubConnectionDisconnected;
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
                NotificationStoryboard.RepeatBehavior = new Windows.UI.Xaml.Media.Animation.RepeatBehavior(1);
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
                NotificationStoryboard.RepeatBehavior = new Windows.UI.Xaml.Media.Animation.RepeatBehavior(1);
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
                NotificationStoryboard.RepeatBehavior = Windows.UI.Xaml.Media.Animation.RepeatBehavior.Forever;
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
            if(sender is Button btn)
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

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                Frame.Navigate(typeof(WaitingRoomPage), null, new SuppressNavigationTransitionInfo());
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            //ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("forwardAnimation", SourceImage);
        }
    }
}
