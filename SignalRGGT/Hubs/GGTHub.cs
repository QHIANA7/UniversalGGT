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
        public Res0001 RequestIdCheck(Req0001 req)
        {
            Boolean result = Singleton<DatabaseService>.Instance.GetIdExist(req.UserID);

            Res0001 res = new Res0001() { Request = req, IsRegisterableID = result, Message = result ? "사용가능한 아이디입니다" : "이미 사용중인 아이디입니다" };

            return res;
        }

        public Res0002 RequestRegister(Req0002 req)
        {
            Boolean result = Singleton<DatabaseService>.Instance.InsertUserInfo(req.UserID, req.Password, req.UserName);

            Res0002 res = new Res0002() { Request = req, IsRegisterd = result, Message = "회원가입 성공" };

            return res;
        }

        public Res0003 RequestLogin(Req0003 req)
        {

            String LoginUserName = Singleton<DatabaseService>.Instance.GetUserName(req.UserID, req.Password);
            String Message = String.Empty;
            Res0003 res = null;
            if (!String.IsNullOrWhiteSpace(LoginUserName))
            {
                String UserStatus = Singleton<DatabaseService>.Instance.GetUserStatus(req.UserID);
                if (UserStatus.Equals("O"))
                {
                    Boolean ConnectionIDUpdateResult = Singleton<DatabaseService>.Instance.UpdateConnectionID(req.UserID, this.Context.ConnectionId);
                    Boolean UserLoginUpdateResult = Singleton<DatabaseService>.Instance.UpdateUserLogin(req.UserID);
                    if (ConnectionIDUpdateResult & UserLoginUpdateResult)
                    {
                        UserStatus = Singleton<DatabaseService>.Instance.GetUserStatus(req.UserID);
                        if (UserStatus.Equals("X"))
                            res = new Res0003() { Request = req, IsLoginSuccess = true, UserName = LoginUserName, Message = "로그인 성공" };
                        else
                            res = new Res0003() { Request = req, IsLoginSuccess = false, UserName = LoginUserName, Message = "로그인에 성공했으나 연결ID 갱신 문제 발생" };
                    }
                    else
                    {
                        res = new Res0003() { Request = req, IsLoginSuccess = false, UserName = LoginUserName, Message = "로그인에 성공했으나 DB 갱신 문제 발생" };
                    }
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

        public Res0004 RequestLogout(Req0004 req)
        {
            Res0004 res = null;
            String UserStatus = Singleton<DatabaseService>.Instance.GetUserStatus(req.UserID);
            if (UserStatus.Equals("O"))
            {
                res = new Res0004() { Request = req, IsLogoutSuccess = false, Message = "로그인 상태가 아닙니다" };
            }
            else
            {
                Boolean UserLoginUpdateResult = Singleton<DatabaseService>.Instance.UpdateUserLogout(req.UserID);
                if (UserLoginUpdateResult)
                {
                    UserStatus = Singleton<DatabaseService>.Instance.GetUserStatus(req.UserID);
                    if (UserStatus.Equals("O"))
                        res = new Res0004() { Request = req, IsLogoutSuccess = true, Message = "로그아웃 성공" };
                    else
                        res = new Res0004() { Request = req, IsLogoutSuccess = false, Message = "로그아웃에 성공했으나 DB 갱신 문제 발생" };
                }
                else
                {
                    res = new Res0004() { Request = req, IsLogoutSuccess = false, Message = "로그아웃에 성공했으나 DB 갱신 문제 발생" };
                }
            }
            return res;
        }

        public void RequestSendMessage(Req0005 req)
        {
            Res0005 res = new Res0005() { Request = req };
            Clients.All.ResponseSendMessage(req, res);
        }

        public void RequestJoinGroup(Req0006 req)
        {
            Res0006 res = null;

            String CurrentLocation = Singleton<DatabaseService>.Instance.GetUserCurrentLocation(req.UserID, out Boolean ex);
            if (ex)
            {
                res = new Res0006() { Request = req, NewGroupName = "Exception", OldGroupName = "Exception", IsJoined = false, Message = "DB조회에 오류가 있습니다" };
                Clients.Caller.ResponseJoinGroup(req, res);

            }
            else
            {
                if (String.IsNullOrWhiteSpace(CurrentLocation))
                {
                    Boolean Result = Singleton<DatabaseService>.Instance.UpdateUserCurrentLocation(req.UserID, req.GroupName);
                    if (Result)
                    {
                        Groups.Add(this.Context.ConnectionId, req.GroupName);
                        res = new Res0006() { Request = req, NewGroupName = req.GroupName, OldGroupName = String.Empty, IsJoined = true, Message = $"{req.GroupName}에 가입하였습니다" };
                        Clients.Group(req.GroupName).ResponseJoinGroup(req, res);
                    }
                    else
                    {
                        res = new Res0006() { Request = req, NewGroupName = "Exception", OldGroupName = "Exception", IsJoined = false, Message = "DB갱신에 오류가 있습니다" };
                        Clients.Caller.ResponseJoinGroup(req, res);
                    }
                }
                else
                {
                    res = new Res0006() { Request = req, NewGroupName = "Exception", OldGroupName = "Exception", IsJoined = false, Message = "이미 가입된 그룹이 있습니다" };
                    Clients.Caller.ResponseJoinGroup(req, res);
                }
            }
        }

        public void RequestLeaveGroup(Req0007 req)
        {
            Res0007 res = null;

            String CurrentLocation = Singleton<DatabaseService>.Instance.GetUserCurrentLocation(req.UserID, out Boolean ex);
            if (ex)
            {
                res = new Res0007() { Request = req, NewGroupName = "Exception", OldGroupName = "Exception", IsLeaved = false, Message = "DB조회에 오류가 있습니다" };
                Clients.Caller.ResponseLeaveGroup(req, res);
            }
            else
            {
                if (String.IsNullOrWhiteSpace(CurrentLocation))
                {
                    res = new Res0007() { Request = req, NewGroupName = "Exception", OldGroupName = CurrentLocation, IsLeaved = false, Message = "가입된 그룹이 없습니다" };
                    Clients.Caller.ResponseLeaveGroup(req, res);
                }
                else
                {
                    Groups.Remove(this.Context.ConnectionId, req.GroupName);
                    Boolean Result = Singleton<DatabaseService>.Instance.UpdateUserCurrentLocation(req.UserID, String.Empty);
                    if (Result)
                    {
                        res = new Res0007() { Request = req, NewGroupName = req.GroupName, OldGroupName = String.Empty, IsLeaved = true, Message = $"{req.GroupName}에서 탈퇴하였습니다" };
                        Clients.Group(req.GroupName).ResponseLeaveGroup(req, res);
                    }
                    else
                    {
                        res = new Res0007() { Request = req, NewGroupName = "Exception", OldGroupName = "Exception", IsLeaved = false, Message = "DB갱신에 오류가 있습니다" };
                        Clients.Caller.ResponseLeaveGroup(req, res);
                    }
                }
            }
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