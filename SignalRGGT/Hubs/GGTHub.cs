using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using SignalRGGT.Helpers;
using SignalRGGT.Services;

namespace SignalRGGT.Hubs
{
    [HubName("GGTHub")]
    public class GGTHub : Hub
    {
        public void RequestLogin(String id, String pw)
        {
            Console.WriteLine($"로그인 요청 : {id} - {pw}");

            String UserName = Singleton<DatabaseService>.Instance.GetUserName(id, pw);
            
            if(String.IsNullOrWhiteSpace(UserName))
            {
                Console.WriteLine($"로그인 성공 {UserName}");
                Clients.Caller.ResponseLogin(UserName);
            }
            else
            {
                Console.WriteLine($"로그인 실패 : {id} - {pw}");
                Clients.Caller.ResponseLogin("ID 또는 Password가 일치하지 않음");                
            }
        }
        
        // server side method #1 : Send
        // echo name and message
        public void Send(string name, string message)
        {
            Console.WriteLine($"Send name : {name} message :{message}");
            Clients.All.addMessage(name, message);
        }

        // server side method #2 : StartTimer
        // send msgs every seconds until count variable ...
        public void StartTimer(int count)
        {
            Console.WriteLine($"StartTimer count : {count}");

            Task.Run(async () =>
            {
                var msg = $"타이머 시작됨...";
                Console.WriteLine(msg);
                Clients.Caller.showMsg(msg);

                for (int i = 0; i < count; i++)
                {
                    await Task.Delay(1000);
                    msg = $"타이머 카운트 {i}/{count}...";
                    Console.WriteLine(msg);
                    Clients.Caller.showMsg(msg);
                }

                msg = $"타이머 종료됨...";
                Console.WriteLine(msg);
                Clients.Caller.showMsg(msg);
            });
        }

        public override Task OnConnected()
        {
            Console.WriteLine("OnConnected");
            _PrintContext();
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Console.WriteLine("OnDisconnected");
            _PrintContext();
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            Console.WriteLine("OnReconnected");
            _PrintContext();
            return base.OnReconnected();
        }

        /// <summary>
        /// print context object
        /// we can know about additional information (like account, auth, ...) in this.Context.
        /// </summary>
        private void _PrintContext()
        {
            Console.WriteLine($"this.Context.ConnectionId : {this.Context.ConnectionId}");
            Console.WriteLine($"this.Context.Request.Url : {this.Context.Request.Url}");
            Console.WriteLine($"this.Context.Headers : {JsonConvert.SerializeObject(this.Context.Headers)}");
        }
    }
}