using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelDesign
{
    class ChattingUserData
    {
        public int zone_id { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public ChatSocket socket { get; }

        public ChattingUserData(ChatSocket socket)
        {
            this.socket = socket;
        }
    }

    class ChattingChannel : SyncedChannel<ChattingUserData>
    {
        public int channel_id { get; }

        public ChattingChannel(int id)
        {
            channel_id = id;
        }
    }

    class ChattingUser : ChattingChannel.Handle
    {
        readonly int user_id;
        readonly string chatname;

        public ChattingUser(int id, string chatname, ChattingUserData data) : base(id, data)
        {
            user_id = id;
            this.chatname = chatname;
        }
    }

    class ChattingChannelSystem
    {
        protected ConcurrentDictionary<int, ChattingChannel> channels { get; set; }

        void Init()
        {
            for (int i = 1; i <= 10; ++i)
            {
                channels.TryAdd(i, new ChattingChannel(i));
            }
        }

        public async Task<bool> AssignChannel(ChattingUser user)
        {
            // select channel
            var ch = channels.First();
            return await user.AddTo(ch.Value);
        }
    }
}
