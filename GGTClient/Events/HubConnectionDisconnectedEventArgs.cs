using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGTClient.Events
{
    public class HubConnectionDisconnectedEventArgs : EventArgs
    {
        public HubConnection Connection { get; private set; }
        public String LastErrorMessage
        {
            get
            {
                return Connection.LastError.Message;
            }
        }
        public DateTime FiredTime { get; private set; }

        internal HubConnectionDisconnectedEventArgs(DateTime fired_time, HubConnection connection)
        {
            FiredTime = fired_time;
            Connection = connection;
        }
    }
}
