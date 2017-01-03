using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelDesign
{
    class ChatSocket
    {
        readonly int user_id = 0;
        readonly ChatServer server;
        readonly ChattingUser user;

        public ChatSocket(ChatServer server)
        {
            this.server = server;
            user = new ChattingUser(1, "shin", new ChattingUserData(this));
        }

        public async Task OnDisconnected()
        {
            await user.RemoveFromChannel();
        }

        public void ProcessPacket()
        {
        }

        public Task SendAsync(string msg)
        {
            return Task.FromResult(0);
        }

        async Task LoginHandler(object obj)
        {
            // validate login...
            await server.chatSystem.AssignChannel(user);
        }

        async Task ChatHandler(string msg)
        {
            await user.ForAllNeighbors(async (handle, userdata) =>
            {
                if (userdata.socket != this)
                    await userdata.socket.SendAsync("dummy");
            });
        }
    }
}
