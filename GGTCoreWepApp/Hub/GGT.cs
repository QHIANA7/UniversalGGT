using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GGTCoreWepApp
{
    public class GGT : Hub
    {
        public void RequestLogin(String id, String pw)
        {
            Console.WriteLine($"로그인 요청 : {id} - {pw}");

            SqlConnection Sqlconn = new SqlConnection("Server=tcp:ggtsvr.database.windows.net,1433;Initial Catalog=GGTDB;Persist Security Info=False;User ID=ggtadmin@ggtsvr.database.windows.net;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            Sqlconn.Open();

            string strSQL = String.Format($"SELECT * FROM TB_USER_INFO WHERE USER_ID = @UserID AND USER_PASSWORD = @UserPassword");
            SqlCommand myCommand = new SqlCommand(strSQL, Sqlconn);
            SqlParameter param_userid = new SqlParameter("@UserID", id);
            myCommand.Parameters.Add(param_userid);

            SqlParameter param_userpw = new SqlParameter("@UserPassword", pw);
            myCommand.Parameters.Add(param_userpw);

            SqlDataReader myDataReader;

            //위 SQL문을 실행해서 가져온다.
            myDataReader = myCommand.ExecuteReader();

            String UserName = String.Empty;
            Int32 EffectedCount = 0;

            //Read를 할때마다 다음 레코드를 불러온다.
            while (myDataReader.Read())
            {
                UserName = myDataReader["USER_NAME"].ToString();
                EffectedCount++;
            }

            if (EffectedCount > 0)
            {
                Clients.Caller.SendAsync("ResponseLogin", UserName);
                Console.WriteLine($"로그인 성공 {UserName}");
            }
            else
            {
                Clients.Caller.SendAsync("ResponseLogin", "ID 또는 Password가 일치하지 않음");
                Console.WriteLine($"로그인 실패 : {id} - {pw}");
            }

            myDataReader.Close();
            myCommand.Dispose();
            Sqlconn.Close();
            Sqlconn.Dispose();
            //myHubProxy.Invoke("RequestLogin", new object[] { id, pw });
        }


        // server side method #1 : Send
        // echo name and message
        public void Send(string name, string message)
        {
            Console.WriteLine($"Send name : {name} message :{message}");
            Clients.Caller.SendAsync("addMessage", name, message);
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
                await Clients.Caller.SendAsync("showMsg",msg);

                for (int i = 0; i < count; i++)
                {
                    await Task.Delay(1000);
                    msg = $"타이머 카운트 {i}/{count}...";
                    Console.WriteLine(msg);
                    await Clients.Caller.SendAsync("showMsg", msg);
                }

                msg = $"타이머 종료됨...";
                Console.WriteLine(msg);
                await Clients.Caller.SendAsync("showMsg", msg);
            });
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("OnConnected");
            _PrintContext();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            Console.WriteLine("OnDisconnected");
            _PrintContext();
            return base.OnDisconnectedAsync(ex);
        }

        /// <summary>
        /// print context object
        /// we can know about additional information (like account, auth, ...) in this.Context.
        /// </summary>
        private void _PrintContext()
        {
            Console.WriteLine($"this.Context.ConnectionId : {this.Context.ConnectionId}");
            Console.WriteLine($"this.Context.Request.Url : {this.Context.GetHttpContext().Request.QueryString}");
            Console.WriteLine($"this.Context.Headers : {JsonConvert.SerializeObject(this.Context.GetHttpContext().Request.Headers)}");
        }
    }
}
