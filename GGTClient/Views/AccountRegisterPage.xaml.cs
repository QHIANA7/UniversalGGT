using GGTClient.ViewModels;
using GGTClient.Helpers;
using GGTClient.Services;
using Windows.UI.Xaml.Controls;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace GGTClient.Views
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class AccountRegisterPage : Page
    {
        public AccountRegisterViewModel ViewModel { get; } = new AccountRegisterViewModel();

        public AccountRegisterPage()
        {
            this.InitializeComponent();
            Singleton<CommunicationService>.Instance.IdCheckApplied += CommunicationService_IdCheckApplied;
        }

        private void CommunicationService_IdCheckApplied(object sender, Events.IdCheckAppliedEventArgs e)
        {
            if(e.IsIdUsable)
            {
                ViewModel.IsRegisterable = true;
            }
            else
            {
                ViewModel.IsRegisterable = false;
            }
        }
    }
}
