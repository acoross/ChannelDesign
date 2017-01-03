using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelDesign
{
    class ChatServer
    {
        public ChattingChannelSystem chatSystem { get; } = new ChattingChannelSystem();

        public ChatServer()
        {
            void OnSocketConnected(ChatSocket socket)
            {
                
            }

            async Task OnSocketDisconnected(ChatSocket socket)
            {
                await socket.OnDisconnected();
            }
        }
    }
}
