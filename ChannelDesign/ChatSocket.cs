using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelDesign
{
    class ChatSocket
    {
        class ChannelUser : ChannelContainer.Channel.Handle
        {
            public readonly ChatSocket socket;

            public ChannelUser(int id, ChatSocket socket) : base(id)
            {
                this.socket = socket;
            }
        }

        readonly int user_id = 0;
        readonly ChannelUser channelUser;
        readonly ChatServer server;

        public ChatSocket()
        {
            channelUser = new ChannelUser(user_id, this);
        }

        public void OnDisconnected()
        {
            channelUser.RemoveFromChannel();
        }

        public void ProcessPacket()
        {

        }

        public void Send(string msg)
        {

        }

        void LoginHandler(object obj)
        {
            // validate login...

            channelUser.AssignChannel(server.channels);
        }

        void ChangeChannel(object obj)
        {
            bool need_to_move;
            string to_server;
            if (channelUser.ChangeChannel(server.channels, 100, out need_to_move, out to_server))
            {

            }
        }

        void ChatHandler(object obj)
        {
            channelUser.ForAllNeighbors((cub) => {
                var cu = cub as ChannelUser;
                if (cu == null)
                    return;

                if (cu != channelUser)
                    cu.socket.Send("dummy");
            });
        }
    }
}
