using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGTClient.Models
{
    public class RoomInfo
    {
        public Int32 RoomNumber { get; set; } = 0;

        public String RoomTitle { get; set; } = "No Defined";

        public String RoomMaster { get; set; } = "방장";

        public Int32 MaxEntrance { get; set; } = 0;

        public Int32 CurrentEntrance { get; set; } = 0;

        public String FormattedRoomNumber()
        {
            return $"{RoomNumber.ToString("000")}";
        }

        public String EntranceCounterString()
        {
            return $"{CurrentEntrance.ToString("00")} / {MaxEntrance.ToString("00")}";
        }

        //public CurrentLocation Location { get; set; } = CurrentLocation.None;

    }
}
