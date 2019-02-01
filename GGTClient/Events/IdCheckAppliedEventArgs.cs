using GGTClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGTClient.Events
{
    public class IdCheckAppliedEventArgs : EventArgs
    {
        public Req0001 Request { get; private set; }
        public Res0001 Response { get; private set; }
        public String Message { get => Response.Message; }
        public DateTime RequestTime { get => Request.RequestTime; }
        public DateTime ResponseTime { get => Response.ResponseTime;  }
        public Boolean IsIdUsable { get => Response.IsUsableID; }

        internal IdCheckAppliedEventArgs(Req0001 req, Res0001 res)
        {
            Request = req;
            Response = res;
        }
    }
}
