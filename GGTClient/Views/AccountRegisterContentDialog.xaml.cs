using GGTClient.Events;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 콘텐츠 대화 상자 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace GGTClient.Views
{
    public sealed partial class AccountRegisterContentDialog : ContentDialog
    {
        public AccountRegisterViewModel ViewModel { get; } = new AccountRegisterViewModel();


        public AccountRegisterContentDialog()
        {
            this.InitializeComponent();
            Singleton<CommunicationService>.Instance.Packet0001Received += CommunicationService_Packet0001Received;
            Singleton<CommunicationService>.Instance.Packet0002Received += CommunicationService_Packet0002Received;
        }
        
        private void CommunicationService_Packet0001Received(object sender, Packet0001ReceivedEventArgs e)
        {
            if (e.IsRegisterable)
            {
                ViewModel.IsRegisterable = true;
            }
            else
            {
                ViewModel.IsRegisterable = false;
            }
        }

        private async void CommunicationService_Packet0002Received(object sender, Packet0002ReceivedEventArgs e)
        {
            if (e.IsRegisterd)
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "회원가입 성공",
                    Content = $"{e.Request.UserID}님의 회원가입이 성공적으로 완료되었습니다.",
                    CloseButtonText = "닫기",
                    DefaultButton = ContentDialogButton.Close
                };
                switch (await dialog.ShowAsync())
                {
                    case ContentDialogResult.Primary:

                        break;
                }
            }
            else
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "회원가입 실패",
                    Content = $"{e.Request.UserID} 회원가입이 성공적으로 완료되었습니다.{Environment.NewLine}{e.Message}",
                    CloseButtonText = "닫기",
                    DefaultButton = ContentDialogButton.Close
                };
                switch (await dialog.ShowAsync())
                {
                    case ContentDialogResult.Primary:

                        break;
                }
            }
        }

        private void AccountRegisterContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = false;
        }

        private void RegisterContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            Singleton<CommunicationService>.Instance.Packet0001Received += CommunicationService_Packet0001Received;
            Singleton<CommunicationService>.Instance.Packet0002Received += CommunicationService_Packet0002Received;
        }
    }
}
