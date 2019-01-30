using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGTClient.Events
{
    public class HubConnectionConnectedEventArgs : EventArgs
    {
        public HubConnection Connection { get; private set; }
        public String ConnectionId
        {
            get
            {
                return Connection.ConnectionId;
            }
        }
        public DateTime FiredTime { get; private set; }

        internal HubConnectionConnectedEventArgs(DateTime fired_time, HubConnection connection)
        {
            FiredTime = fired_time;
            Connection = connection;
        }
    }
}
