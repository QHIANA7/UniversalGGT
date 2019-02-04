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

        public MessageInfo(DateTime time, String name, String msg, Boolean is_mine)
        {
            MessageTime = time;
            UserName = name;
            Message = msg;
            IsMine = is_mine;
        }

        public override String ToString()
        {        
            return $"[{MessageTime.ToString("HH:mm:ss")}] [{UserName}] {Message}";
        }

        public SolidColorBrush TextColor()
        {
            if(IsMine)
                return new SolidColorBrush(Colors.Blue);
            else
                return new SolidColorBrush(Colors.Black);
        }

    }
}
