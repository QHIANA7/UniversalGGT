using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGTClient.Events
{
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
