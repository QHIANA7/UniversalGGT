﻿using GGTClient.Events;
using GGTClient.Helpers;
using GGTClient.Services;
using GGTClient.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            Singleton<CommunicationService>.Instance.HubConnectionErrorFiredInfo += CommunicationService_HubConnectionErrorFiredInfo;
            Singleton<CommunicationService>.Instance.HubConnectionConnectedInfo += CommunicationService_HubConnectionConnectedInfo;
            Singleton<CommunicationService>.Instance.HubConnectionConnectingInfo += CommunicationService_HubConnectionConnectingInfo;
            Singleton<CommunicationService>.Instance.HubConnectionDisconnectedInfo += CommunicationService_HubConnectionDisconnectedInfo;
        }

        private async void CommunicationService_HubConnectionConnectedInfo(object sender, HubConnectionConnectedEventArgs e)
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

        private async void CommunicationService_HubConnectionDisconnectedInfo(object sender, HubConnectionDisconnectedEventArgs e)
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

        private async void CommunicationService_HubConnectionConnectingInfo(object sender, HubConnectionConnectingEventArgs e)
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

        private void CommunicationService_HubConnectionErrorFiredInfo(object sender, HubConnectionErrorFiredEventArgs e)
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

        private void Button_AccountRegister_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                AccountRegisterOpenStoryboard.Begin();
                Frame_AccountRegister.Navigate(typeof(AccountRegisterPage));
            }
        }
    }
}
