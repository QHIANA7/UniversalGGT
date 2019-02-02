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
using SignalRGGT.Models;
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
            String UserStatus = Singleton<DatabaseService>.Instance.GetUserStatus(id, pw);

            if (!String.IsNullOrWhiteSpace(UserName))
            {
                if (UserStatus == "O")
                {
                    Console.WriteLine($"로그인 성공 {UserName}");
                    Clients.Caller.ResponseLogin(UserName);
                }
                else
                {
                    Console.WriteLine($"로그인 실패 : {UserName}");
                    Clients.Caller.ResponseLogin("이미 로그인 상태입니다");
                }
            }
            else
            {
                Console.WriteLine($"로그인 실패 : {id} - {pw}");
                Clients.Caller.ResponseLogin("ID 또는 Password가 일치하지 않음");
            }
        }



        public void ConnectionCheck(String data)
        {
            Console.WriteLine($"연결 체크");
            Clients.Caller.OnConnectionCheckResponse(data);
        }

        public Res0001 RequestIdCheck(Req0001 req)
        {
            Boolean result = Singleton<DatabaseService>.Instance.GetIdExist(req.UserID);

            Res0001 res = new Res0001() { Request = req, IsRegisterableID = result, Message = result ? "사용가능한 아이디입니다" : "이미 사용중인 아이디입니다" };

            return res;
        }

        public Res0002 RequestRegister(Req0002 req)
        {
            Boolean result = Singleton<DatabaseService>.Instance.InsertCarRegistrationInfo(req.UserID, req.Password,req.UserName);

            Res0002 res = new Res0002() { Request = req, IsRegisterd = result, Message = "회원가입 성공" };

            return res;
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