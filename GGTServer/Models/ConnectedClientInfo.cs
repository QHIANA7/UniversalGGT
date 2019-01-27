using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGTServer.Models
{
    public class ConnectedClientInfo
    {
        public String ClientIp { get; set; }

        public String Tag { get; set; }

        public ConnectedClientInfo(String ip, String tag)
        {
            ClientIp = ip;
            Tag = tag;
        }

        public override string ToString()
        {
            return $"{ClientIp} : {Tag}";
        }
    }
}
