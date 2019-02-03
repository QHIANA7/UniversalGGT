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
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
            Singleton<CommunicationService>.Instance.Packet0005Received += CommunicationService_Packet0005Received;
        }

        private void CommunicationService_Packet0005Received(object sender, Events.Packet0005ReceivedEventArgs e)
        {
            //RichEditBox_Message.Document.SetText(Windows.UI.Text.TextSetOptions.None, e.Response.Message + Environment.NewLine);
            //RichEditBox_Message.Document.SetText(Windows.UI.Text.TextSetOptions.None, e.Response.Message + Environment.NewLine);
            String Message = e.Response.Message;
            Paragraph paragraph = new Paragraph();
            Inline inline = null;
            if (e.Response.SendFrom.Equals(ViewModel.UserName))
            {                 
                inline = new Run() { Text = Message, FontSize = 10, Foreground = new SolidColorBrush(Colors.Blue) };
            }
            else
            {
                inline = new Run() { Text = Message, FontSize = 10 };
            }
            paragraph.Inlines.Add(inline);
            //RichEditBox_Message.Document
            RichTextBlock_Message.Blocks.Add(paragraph);
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
