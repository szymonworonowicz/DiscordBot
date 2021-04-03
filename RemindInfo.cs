using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    internal class RemindInfo
    {
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public ulong ChannelId { get; set; }
    }
}
