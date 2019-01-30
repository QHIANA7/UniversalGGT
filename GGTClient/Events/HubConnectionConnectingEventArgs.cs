using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGTClient.Events
{
    public class HubConnectionConnectingEventArgs : EventArgs
    {
        public HubConnection Connection { get; private set; }
        public String ConnectionUrl
        {
            get
            {
                return Connection.Url;
            }
        }
        public DateTime FiredTime { get; private set; }

        internal HubConnectionConnectingEventArgs(DateTime fired_time, HubConnection connection)
        {
            FiredTime = fired_time;
            Connection = connection;
        }
    }
}
