using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLART.Objects
{
    public class StickyMessage
    {
        public ulong StaffId { get; set; }
        public ulong ChannelId { get; set; }
        public string Message { get; set; }

        public StickyMessage(ulong channelId, string message, ulong staffId)
        {
            ChannelId = channelId;
            Message = message;
            StaffId = staffId;
        }
    }
}
