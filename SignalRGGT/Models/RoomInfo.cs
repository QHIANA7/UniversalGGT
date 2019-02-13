using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRGGT.Models
{
    public class RoomInfo
    {
        public Int32 RoomNumber { get; set; } = 0;

        public String RoomTitle { get; set; } = "No Defined";

        public Boolean IsPrivateAccess { get; set; } = false;

        public String AccessPassword { get; set; } = String.Empty;

        public String RoomMaster { get; set; } = "방장";

        public Int32 MaxJoinCount { get; set; } = 0;

        public Int32 CurrentJoinCount { get; set; } = 0;

        public Boolean IsPlaying { get; set; } = false;

        public String FormattedRoomNumber()
        {
            return $"{RoomNumber.ToString("000")}";
        }

        public String EntranceCounterString()
        {
            return $"{CurrentJoinCount.ToString("00")} / {MaxJoinCount.ToString("00")}";
        }
    }
}