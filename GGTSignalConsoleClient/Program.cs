using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGTSignalConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            HubConnection hubConnection = null;

            // init hub connection with url ...
            hubConnection = new HubConnection("http://localhost:8079/");
            var myHubProxy = hubConnection.CreateHubProxy("MyHub");

            // attach event handler from server sent message.
            myHubProxy.On<string, string>("addMessage", _OnAddMessage);
            myHubProxy.On<string>("showMsg", _OnShowMsg);

            // retry connection every 3 seconds ...
            while (hubConnection.State != ConnectionState.Connected)
            {
                try
                {
                    hubConnection.Start().Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(@"
connection failed !
sleep 3 sec ...
try reconnect ...
                ");

                    Task.Delay(3000).Wait();
                }
            }

            // run actions (Send, StartTimer)
            _RunServerLoop(myHubProxy);

            // exit program
            Console.WriteLine("ended!!!");
            Console.ReadLine();
        }

        private static void _OnShowMsg(string msg)
        {
            Console.WriteLine($"RECV showMsg : {msg}");
        }



        private static void _OnAddMessage(string name, string message)
        {
            Console.WriteLine($"RECV addMessage : {name} : {message}");
        }



        private static void _RunServerLoop(IHubProxy myHubProxy)
        {
            while (true)
            {
                Console.WriteLine(@"
commands
------------------------------------
Q : quit
S : Send(""william"", ""hello"")
T : StartTimer(10)
            ");

                var cmd = Console.ReadKey();

                if (cmd.Key == ConsoleKey.Q)
                {
                    break;
                }
                else if (cmd.Key == ConsoleKey.S)
                {
                    myHubProxy.Invoke("Send", new object[] { "william", "hello" });
                }
                else if (cmd.Key == ConsoleKey.T)
                {
                    myHubProxy.Invoke("StartTimer", new object[] { 10 });
                }
            }
        }
    }
}
