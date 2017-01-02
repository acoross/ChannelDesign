using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelDesign
{
    class ChatServer
    {
        public readonly ChannelContainer channels = new ChannelContainer();

        public ChatServer()
        {
            void OnSocketConnected(ChatSocket socket)
            {
                
            }

            void OnSocketDisconnected(ChatSocket socket)
            {
                socket.OnDisconnected();
            }
        }
    }
}
