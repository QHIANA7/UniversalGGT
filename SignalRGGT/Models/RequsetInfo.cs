using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRGGT.Models
{
    /// <summary>
    /// ID중복검사에 대한 요청
    /// </summary>
    public class Req0001
    {
        public String UserID { get; set; }
        public DateTime RequestTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 회원가입에 대한 요청
    /// </summary>
    public class Req0002
    {
        public String UserID { get; set; }
        public String Password { get; set; }
        public String UserName { get; set; }
        public DateTime RequestTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 로그인에 대한 요청
    /// </summary>
    public class Req0003
    {
        public String UserID { get; set; }
        public String Password { get; set; }
        public DateTime RequestTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 메시지 전송에 대한 요청
    /// </summary>
    public class Req0005
    {
        public String UserID { get; set; }
        public String UserName { get; set; }
        public String Message { get; set; }
        public DateTime RequestTime { get; set; } = DateTime.Now;
    }
}
