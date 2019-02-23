using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGTClient.Models
{
    public class UserInfo
    {
        public String UserId { get; set; } = String.Empty;

        public String UserName { get; set; } = String.Empty;

        public String UserPassword { get; set; } = String.Empty;

        public CurrentLocation Location { get; set; } = CurrentLocation.None;

        public String ExtraLocation { get; set; } = String.Empty;


        public override String ToString()
        {
            return $"{UserName}";
        }

        public String GetLocation()
        {
            switch (Location)
            {
                case CurrentLocation.None:
                    return $"{UserName}({"지정되지 않음"})";
                case CurrentLocation.Init:
                    return $"{UserName}({"초기화면"})";
                case CurrentLocation.WaitingRoom:
                    return $"{UserName}({"대기실"})";
                case CurrentLocation.PlayingRoom:
                    return $"{UserName}({"게임방"}{ExtraLocation})";
                default:
                    return $"{UserName}({"지정되지 않음"})";
            }
        }
    }
}
