using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGTClient.Models
{
    public class UserInfo
    {
        public String UserId { get; set; }

        public String UserName { get; set; }

        public String UserPassword { get; set; }

        public UserInfo(String id, String pw)
        {
            UserId = id;
            UserName = String.Empty;
            UserPassword = pw;
        }

        public override String ToString()
        {
            return $"{UserName}";
        }

    }
}
