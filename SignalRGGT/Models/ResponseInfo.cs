using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRGGT.Models
{
    public class ResponsePacket
    {
        public String Message { get; set; }
        public DateTime ResponseTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// ID중복검사에 대한 응답
    /// </summary>
    public class Res0001 : ResponsePacket
    {
        public Req0001 Request { get; set; }
        public Boolean IsRegisterableID { get; set; }
    }

    /// <summary>
    /// 회원가입에 대한 응답
    /// </summary>
    public class Res0002 : ResponsePacket
    {
        public Req0002 Request { get; set; }
        public Boolean IsRegisterd { get; set; }
    }

    /// <summary>
    /// 로그인에 대한 응답
    /// </summary>
    public class Res0003 : ResponsePacket
    {
        public Req0003 Request { get; set; }
        public String UserName { get; set; }
        public Boolean IsLoginSuccess { get; set; }
    }

    /// <summary>
    /// 로그아웃에 대한 응답
    /// </summary>
    public class Res0004 : ResponsePacket
    {
        public Req0004 Request { get; set; }
        public Boolean IsLogoutSuccess { get; set; }
    }

    /// <summary>
    /// 메시지 전송에 대한 응답
    /// </summary>
    public class Res0005 : ResponsePacket
    {
        public Req0005 Request { get; set; }
        public String SendFrom { get => Request.UserName; }
        public String UserMessage { get => $"[{Request.RequestTime.ToString("HH:mm:ss")}] [{SendFrom}] {Request.Message}"; }
    }

    /// <summary>
    /// 그룹 가입에 대한 응답
    /// </summary>
    public class Res0006 : ResponsePacket
    {
        public Req0006 Request { get; set; }
        public Boolean IsJoined { get; set; }
    }

    /// <summary>
    /// 그룹 탈퇴에 대한 응답
    /// </summary>
    public class Res0007 : ResponsePacket
    {
        public Req0007 Request { get; set; }
        public String IsLeaved { get; set; }
    }
}
