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
    public class AccountRegisterViewModel : Observable
    {
        private String _user_id = String.Empty;
        public String UserId
        {
            get { return _user_id; }
            set { Set(ref _user_id, value); }
        }

        private String _user_pw = String.Empty;
        public String UserPassword
        {
            get { return _user_pw; }
            set { Set(ref _user_pw, value); }
        }

        private String _user_name = String.Empty;
        public String UserName
        {
            get { return _user_name; }
            set { Set(ref _user_name, value); }
        }

        private Boolean _is_registerable = false;
        public Boolean IsRegisterable
        {
            get { return _is_registerable; }
            set { Set(ref _is_registerable, value); }
        }

        private ICommand _id_check;
        public ICommand IDCheck
        {
            get
            {
                if (_id_check == null)
                {
                    _id_check = new RelayCommand(
                        () =>
                        {
                            Singleton<CommunicationService>.Instance.RequestIdCheck(UserId);
                        });
                }

                return _id_check;
            }
        }
    }
}
