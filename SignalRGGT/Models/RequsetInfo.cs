using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRGGT.Models
{
    public class RequestPacket
    {
        public DateTime RequestTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// ID중복검사에 대한 요청
    /// </summary>
    public class Req0001 : RequestPacket
    {
        public String UserID { get; set; }
    }

    /// <summary>
    /// 회원가입에 대한 요청
    /// </summary>
    public class Req0002 : RequestPacket
    {
        public String UserID { get; set; }
        public String Password { get; set; }
        public String UserName { get; set; }
    }

    /// <summary>
    /// 로그인에 대한 요청
    /// </summary>
    public class Req0003 : RequestPacket
    {
        public String UserID { get; set; }
        public String Password { get; set; }
    }

    /// <summary>
    /// 로그아웃에 대한 요청
    /// </summary>
    public class Req0004 : RequestPacket
    {
        public String UserID { get; set; }
    }

    /// <summary>
    /// 메시지 전송에 대한 요청
    /// </summary>
    public class Req0005 : RequestPacket
    {
        public String UserID { get; set; }
        public String UserName { get; set; }
        public Boolean IsSystemMessage { get; set; }
        public String Message { get; set; }
    }

    /// <summary>
    /// 그룹 이동에 대한 요청
    /// </summary>
    public class Req0006 : RequestPacket
    {
        public String UserID { get; set; }
        public String NewGroupName { get; set; }
        public String ExpectedOldGroupName { get; set; }
    }

    /// <summary>
    /// 사용자 정보 조회에 대한 요청
    /// </summary>
    public class Req0007 : RequestPacket
    {
        public String UserID { get; set; }
    }

    /// <summary>
    /// 방 생성에 대한 요청
    /// </summary>
    public class Req0008 : RequestPacket
    {
        public String UserID { get; set; }
        public RoomInfo Room { get; set; }
    }
}
