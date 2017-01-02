using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ChannelDesign
{
    class ChannelRedis
    {
        public void Save()
        {

        }
    }
    
    class ChannelContainer
    {
        public class Channel
        {
            public class Handle
            {
                public int id = 0;
                Channel channel = null;

                protected Handle(int id)
                {
                    this.id = id;
                }

                bool is_assigned
                {
                    get { return channel == null; }
                }

                public bool AssignChannel(ChannelContainer container)
                {
                    var ch = container.GetAssignChannel(this);
                    if (ch == null)
                        return false;

                    return AddTo(ch);
                }

                public bool ChangeChannel(ChannelContainer container, int channel_number, out bool need_to_move_server, out string to_server)
                {
                    var ch = container.GetChangeChannel(this, channel_number, out need_to_move_server, out to_server);
                    if (ch == null)
                        return false;

                    return AddTo(ch);
                }

                bool AddTo(Channel channel)
                {
                    if (channel.users.TryAdd(id, this))
                    {
                        this.channel = channel;
                        SaveChannelToRedis();

                        return true;
                    }
                    return false;
                }

                public bool RemoveFromChannel()
                {
                    if (this.channel != null)
                    {
                        this.channel = null;
                        SaveChannelToRedis();

                        Handle outUser;
                        return channel.users.TryRemove(id, out outUser);
                    }

                    return true;
                }

                void SaveChannelToRedis()
                {
                    if (channel != null)
                    {
                        // save to redis
                    }
                }

                public bool ForAllNeighbors(Action<Handle> func)
                {
                    if (channel == null)
                        return false;

                    channel.ForAll(func);

                    return true;
                }
            }

            public readonly int number = 0;
            ConcurrentDictionary<int, Handle> users;

            public Channel(int number)
            {
                this.number = number;
            }

            public void ForAll(Action<Handle> func)
            {
                foreach (var cub in users)
                {
                    func(cub.Value);
                }
            }

            public int CountUser()
            {
                return users.Count;
            }
        }

        ConcurrentDictionary<int, Channel> channels { get; set; }

        public void Init()
        {

        }

        //bool AssignChannel(Channel.Handle handle)
        //{
        //    handle.RemoveFromChannel();

        //    // 할당할 채널을 찾고, 
        //    var min_ch = GetAssignChannel(handle);

        //    // 해당 채널에 유저를 넣는다.
        //    if (min_ch != null)
        //    {
        //        return handle.AddTo(min_ch);
        //    }

        //    return false;
        //}

        Channel GetAssignChannel(Channel.Handle handle)
        {
            // 할당할 채널을 찾고, 
            int min = int.MaxValue;
            Channel min_ch = null;
            foreach (var ch in channels)
            {
                int current = ch.Value.CountUser();
                if (current < min)
                {
                    min = current;
                    min_ch = ch.Value;
                }
            }

            return min_ch;
        }

        //bool ChangeChannel(Channel.Handle handle, int channel_number, out bool need_to_move_server, out string to_server)
        //{
        //    handle.RemoveFromChannel();

        //    var ch = GetChangeChannel(handle, channel_number, out need_to_move_server, out to_server);
        //    return handle.AddTo(ch);
        //}

        Channel GetChangeChannel(Channel.Handle handle, int channel_number, out bool need_to_move_server, out string to_server)
        {
            Channel outChannel;
            if (channels.TryGetValue(channel_number, out outChannel))
            {
                need_to_move_server = false;
                to_server = "";
                return outChannel;
            }

            // check move server
            if (false) // temp
            {
                need_to_move_server = true;
                to_server = "127.0.0.1";
                return null;
            }

            // make custom channel
            need_to_move_server = false;
            to_server = "";

            return new Channel(channel_number);
        }
    }
}
