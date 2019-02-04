﻿using System;
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
        public Res0001 RequestIdCheck(Req0001 req)
        {
            Boolean result = Singleton<DatabaseService>.Instance.GetIdExist(req.UserID);

            Res0001 res = new Res0001() { Request = req, IsRegisterableID = result, Message = result ? "사용가능한 아이디입니다" : "이미 사용중인 아이디입니다" };

            return res;
        }

        public Res0002 RequestRegister(Req0002 req)
        {
            Boolean result = Singleton<DatabaseService>.Instance.InsertUserInfo(req.UserID, req.Password,req.UserName);

            Res0002 res = new Res0002() { Request = req, IsRegisterd = result, Message = "회원가입 성공" };

            return res;
        }

        public Res0003 RequestLogin(Req0003 req)
        {

            String LoginUserName = Singleton<DatabaseService>.Instance.GetUserName(req.UserID, req.Password);
            String UserStatus = Singleton<DatabaseService>.Instance.GetUserStatus(req.UserID, req.Password);
            String Message = String.Empty;
            Res0003 res = null;
            if (!String.IsNullOrWhiteSpace(LoginUserName))
            {
                if (UserStatus == "O")
                {
                    Boolean UpdateResult = Singleton<DatabaseService>.Instance.UpdateConnectionID(req.UserID, this.Context.ConnectionId);
                    if (UpdateResult)
                        res = new Res0003() { Request = req, IsLoginSuccess = true, UserName = LoginUserName, Message = "로그인 성공" };
                    else
                        res = new Res0003() { Request = req, IsLoginSuccess = false, UserName = LoginUserName, Message = "로그인에 성공했으나 연결ID 갱신 문제 발생 " + this.Context.ConnectionId };
                }
                else
                {
                    res = new Res0003() { Request = req, IsLoginSuccess = false, UserName = LoginUserName, Message = "이미 로그인 상태입니다" };
                }
            }
            else
            {
                res = new Res0003() { Request = req, IsLoginSuccess = false, Message = "ID 또는 Password가 일치하지 않음" };
            }            

            return res;
        }

        public void RequestSendMessage(Req0005 req)
        {
            Res0005 res = new Res0005() { Request = req };
            Clients.All.ResponseSendMessage(req, res);
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