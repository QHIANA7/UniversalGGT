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
            Res0005 res = null;

            String CurrentLocation = Singleton<DatabaseService>.Instance.GetUserCurrentLocation(req.UserID, out Boolean ex);
            if (ex)
            {
                res = new Res0005() { Request = req, Message = "DB조회에 오류가 있습니다" };
                Clients.Caller.ResponseMoveGroup(req, res);
            }
            else
            {
                res = new Res0005() { Request = req, Message = "정상" };
                Clients.Group(CurrentLocation).ResponseSendMessage(req, res);
            }
        }

        public void RequestMoveGroup(Req0006 req)
        {
            Res0006 res = null;

            String CurrentLocation = Singleton<DatabaseService>.Instance.GetUserCurrentLocation(req.UserID, out Boolean ex);
            String SendUser = Singleton<DatabaseService>.Instance.GetUserName(req.UserID);
            if (ex)
            {
                res = new Res0006() { Request = req, SendFrom = SendUser, NewGroupName = "Exception", OldGroupName = "Exception", Message = CurrentLocation };
                Clients.Caller.ResponseMoveGroup(req, res);
            }
            else
            {
                if (CurrentLocation.Equals(req.ExpectedOldGroupName))
                {

                    Boolean Result = Singleton<DatabaseService>.Instance.UpdateUserCurrentLocation(req.UserID, req.NewGroupName);
                    if (Result)
                    {
                        Groups.Remove(this.Context.ConnectionId, CurrentLocation);
                        Groups.Add(this.Context.ConnectionId, req.NewGroupName);
                        res = new Res0006() { Request = req, SendFrom = SendUser, NewGroupName = req.NewGroupName, OldGroupName = CurrentLocation, IsMoved = true, Message = $"{CurrentLocation} => {req.NewGroupName} 이동 성공" };
                        Clients.Caller.ResponseMoveGroup(req, res);
                        if (!String.IsNullOrWhiteSpace(req.ExpectedOldGroupName))
                        {
                            //Clients.OthersInGroup(req.ExpectedOldGroupName).ResponseMoveGroup(req, res);
                            Clients.Others.ResponseMoveGroup(req, res);
                        }
                        //Clients.OthersInGroup(req.NewGroupName).ResponseMoveGroup(req, res); //20190206 그룹내 전송
                        Clients.Others.ResponseMoveGroup(req, res); //나 이외에 전송

                    }
                    else
                    {
                        res = new Res0006() { Request = req, SendFrom = SendUser, NewGroupName = "Exception", OldGroupName = "Exception", Message = "DB갱신에 오류가 있습니다" };
                        Clients.Caller.ResponseMoveGroup(req, res);
                    }
                }
                else
                {
                    //개발용 로직
                    if (req.NewGroupName.Equals(Models.CurrentLocation.Init.ToString()) & req.ExpectedOldGroupName.Equals(Models.CurrentLocation.None.ToString()))
                    {
                        res = new Res0006() { Request = req, SendFrom = SendUser, NewGroupName = req.NewGroupName, OldGroupName = req.ExpectedOldGroupName, IsMoved = true, Message = "초기로그인 입니다" };
                        Clients.Others.ResponseMoveGroup(req, res); //나 이외에 전송
                    }
                    else
                        res = new Res0006() { Request = req, SendFrom = SendUser, NewGroupName = "Exception", OldGroupName = "Exception", Message = "요청한 현재 그룹이 서버와 일치하지않습니다" };
                    Clients.Caller.ResponseMoveGroup(req, res);
                }
            }
        }

        public Res0007 RequestGetUserList(Req0007 req)
        {
            Res0007 res = null;
            IEnumerable<UserInfo> UserList = Singleton<DatabaseService>.Instance.GetUsersInfo(out Boolean ex);
            if (ex)
            {
                res = new Res0007() { Request = req, UserList = null, Message = "DB조회에 오류가 있습니다" };
            }
            else
            {
                res = new Res0007() { Request = req, UserList = UserList, Message = "조회 성공" };
            }
            return res;
        }

        public Res0008 RequestMakeRoom(Req0008 req)
        {
            Res0008 res = null;

            Int32 available_room_no = Singleton<DatabaseService>.Instance.GetRoomNumbers(out Boolean ex).Max();
            available_room_no++;
            String UserName = Singleton<DatabaseService>.Instance.GetUserName(req.UserID);
            Boolean is_private = String.IsNullOrEmpty(req.AccessPassword);
            Boolean Result = Singleton<DatabaseService>.Instance.InsertRoomInfo(available_room_no, req.RoomTitle, is_private, req.AccessPassword, UserName, req.MaxJoinCount, out ex);

            if (ex)
            {
                res = new Res0008() { Request = req, Message = "DB조회에 오류가 있습니다" };
            }
            else
            {
                res = new Res0008() { Request = req, Message = "방 만들기 성공", IsCreated = Result, CreatedRoom = new RoomInfo() { RoomNumber = available_room_no, RoomTitle = req.RoomTitle, IsPrivateAccess = is_private, AccessPassword = req.AccessPassword, RoomMaster = UserName, MaxJoinCount = req.MaxJoinCount, CurrentJoinCount = req.CurrentJoinCount, IsPlaying = req.IsPlaying } };
                Clients.OthersInGroup(CurrentLocation.WaitingRoom.ToString()).ResponseMakeRoom(req, res);
            }
            return res;
        }

        public Res0009 RequestGetRoomList(Req0009 req)
        {
            Res0009 res = null;
            IEnumerable<RoomInfo> RoomList = Singleton<DatabaseService>.Instance.GetRoomsInfo(out Boolean ex);
            if (ex)
            {
                res = new Res0009() { Request = req, RoomList = null, Message = "DB조회에 오류가 있습니다" };
            }
            else
            {
                res = new Res0009() { Request = req, RoomList = RoomList, Message = "조회 성공" };
            }
            return res;
        }

        public override Task OnConnected()
        {
            Console.WriteLine("OnConnected");
            return base.OnConnected();
        }

        public override Task OnDisconnected(Boolean stopCalled)
        {
            Console.WriteLine("OnDisconnected");                        
                Singleton<DatabaseService>.Instance.UpdateUserDisconnectMessage(this.Context.ConnectionId, stopCalled ? "Client explicitly closed the connection" : "Timed out");

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            Console.WriteLine("OnReconnected");
            Singleton<DatabaseService>.Instance.UpdateUserDisconnectMessage(this.Context.ConnectionId, "Reconnected");
            return base.OnReconnected();
        }
    }
}