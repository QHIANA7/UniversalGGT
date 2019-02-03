using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRGGT.Models
{
    /// <summary>
    /// ID중복검사에 대한 응답
    /// </summary>
    public class Res0001
    {
        public Req0001 Request { get; set; }
        public DateTime ResponseTime { get; set; } = DateTime.Now;
        public Boolean IsRegisterableID { get; set; }
        public String Message { get; set; }
    }

    /// <summary>
    /// 회원가입에 대한 응답
    /// </summary>
    public class Res0002
    {
        public Req0002 Request { get; set; }
        public DateTime ResponseTime { get; set; } = DateTime.Now;
        public Boolean IsRegisterd { get; set; }
        public String Message { get; set; }
    }

    /// <summary>
    /// 메시지 전송에 대한 응답
    /// </summary>
    public class Res0005
    {
        public Req0005 Request { get; set; }
        public DateTime ResponseTime { get; set; } = DateTime.Now;
        public String SendFrom { get => Request.UserName; }
        public String Message { get => $"[{Request.RequestTime.ToShortTimeString()}] [{SendFrom}] {Request.Message}"; }
    }
}
