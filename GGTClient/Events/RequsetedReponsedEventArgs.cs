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

    public class Packet0003ReceivedEventArgs : EventArgs
    {
        public Req0003 Request { get; private set; }
        public Res0003 Response { get; private set; }
        public String UserName { get => Response.UserName; }
        public String Message { get => Response.Message; }
        public DateTime RequestTime { get => Request.RequestTime; }
        public DateTime ResponseTime { get => Response.ResponseTime; }
        public Boolean IsLoginSuccess { get => Response.IsLoginSuccess; }

        internal Packet0003ReceivedEventArgs(Req0003 req, Res0003 res)
        {
            Request = req;
            Response = res;
        }
    }

    public class Packet0004ReceivedEventArgs : EventArgs
    {
        public Req0004 Request { get; private set; }
        public Res0004 Response { get; private set; }
        public String Message { get => Response.Message; }
        public DateTime RequestTime { get => Request.RequestTime; }
        public DateTime ResponseTime { get => Response.ResponseTime; }
        public Boolean IsLogoutSuccess { get => Response.IsLogoutSuccess; }

        internal Packet0004ReceivedEventArgs(Req0004 req, Res0004 res)
        {
            Request = req;
            Response = res;
        }
    }

    public class Packet0005ReceivedEventArgs : EventArgs
    {
        public Req0005 Request { get; private set; }
        public Res0005 Response { get; private set; }
        public Boolean IsSystemMessage { get => Response.IsSystemMessage; }
        public String Message { get => Response.UserMessage; }
        public String SendFrom { get => Request.UserName; }
        public DateTime RequestTime { get => Request.RequestTime; }
        public DateTime ResponseTime { get => Response.ResponseTime; }

        internal Packet0005ReceivedEventArgs(Req0005 req, Res0005 res)
        {
            Request = req;
            Response = res;
        }
    }

    public class Packet0006ReceivedEventArgs : EventArgs
    {
        public Req0006 Request { get; private set; }
        public Res0006 Response { get; private set; }
        public String SendFrom { get => Response.SendFrom; }
        public String NewGroupName { get => Response.NewGroupName; }
        public String OldGroupName { get => Response.OldGroupName; }
        public Boolean IsMoved { get => Response.IsMoved; }
        public String Message { get => Response.Message; }
        public DateTime RequestTime { get => Request.RequestTime; }
        public DateTime ResponseTime { get => Response.ResponseTime; }

        internal Packet0006ReceivedEventArgs(Req0006 req, Res0006 res)
        {
            Request = req;
            Response = res;
        }
    }

    public class Packet0007ReceivedEventArgs : EventArgs
    {
        public Req0007 Request { get; private set; }
        public Res0007 Response { get; private set; }
        public IEnumerable<UserInfo> UserList { get => Response.UserList; }
        public String Message { get => Response.Message; }
        public DateTime RequestTime { get => Request.RequestTime; }
        public DateTime ResponseTime { get => Response.ResponseTime; }

        internal Packet0007ReceivedEventArgs(Req0007 req, Res0007 res)
        {
            Request = req;
            Response = res;
        }
    }
}
