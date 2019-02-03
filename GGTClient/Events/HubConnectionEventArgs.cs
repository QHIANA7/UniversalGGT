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

    public class HubConnectionErrorFiredEventArgs : EventArgs
    {
        public Exception ExceptionType { get; private set; }
        public HubConnection Connection { get; private set; }
        public String Message
        {
            get
            {
                return ExceptionType.Message;
            }
        }
        public DateTime ExceptionTime { get; private set; }

        internal HubConnectionErrorFiredEventArgs(DateTime _exception_time, Exception _exception, HubConnection _connection)
        {
            ExceptionTime = _exception_time;
            ExceptionType = _exception;
            Connection = _connection;
        }
    }
}
