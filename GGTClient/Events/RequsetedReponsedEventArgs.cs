using GGTClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGTClient.Events
{
    public class Packet0001ReceivedEventArgs : EventArgs
    {
        public Req0001 Request { get; private set; }
        public Res0001 Response { get; private set; }
        public String Message { get => Response.Message; }
        public DateTime RequestTime { get => Request.RequestTime; }
        public DateTime ResponseTime { get => Response.ResponseTime; }
        public Boolean IsRegisterable { get => Response.IsRegisterableID; }

        internal Packet0001ReceivedEventArgs(Req0001 req, Res0001 res)
        {
            Request = req;
            Response = res;
        }
    }

    public class Packet0002ReceivedEventArgs : EventArgs
    {
        public Req0002 Request { get; private set; }
        public Res0002 Response { get; private set; }
        public String Message { get => Response.Message; }
        public DateTime RequestTime { get => Request.RequestTime; }
        public DateTime ResponseTime { get => Response.ResponseTime; }
        public Boolean IsRegisterd { get => Response.IsRegisterd; }

        internal Packet0002ReceivedEventArgs(Req0002 req, Res0002 res)
        {
            Request = req;
            Response = res;
        }
    }
}
