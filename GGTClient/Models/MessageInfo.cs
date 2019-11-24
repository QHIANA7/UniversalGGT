using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace GGTClient.Models
{
    public class MessageInfo
    {
        public DateTime MessageTime { get; set; }

        public String UserName { get; set; }

        public String Message { get; set; }

        public Boolean IsMine { get; set; }

        public Boolean IsSystemMessage { get; set; }

        public MessageInfo(DateTime time, String name, String msg, Boolean is_mine, Boolean is_sys_msg = false)
        {
            MessageTime = time;
            UserName = name;
            Message = msg;
            IsMine = is_mine;
            IsSystemMessage = is_sys_msg;
        }

        public override String ToString()
        {
            if (IsSystemMessage)
                return $"[{MessageTime.ToString("HH:mm:ss")}] [SYSTEM] {Message}";
            else
                return $"[{MessageTime.ToString("HH:mm:ss")}] [{UserName}] {Message}";
        }

        public SolidColorBrush TextColor()
        {
            if (IsSystemMessage)
                return new SolidColorBrush(Colors.Green);
            else if (IsMine)
                return new SolidColorBrush(Colors.Blue);
            else
                return new SolidColorBrush(Colors.Black);
        }

    }
}
