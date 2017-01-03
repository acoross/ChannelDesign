using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acoross;

namespace ChannelDesign
{
    public class SyncedChannel<TSyncData>
    {
        public class Handle
        {
            public int handle_id = 0;
            public SyncedChannel<TSyncData> channel = null;
            TSyncData data = default(TSyncData);

            public bool is_assigned
            {
                get { return channel != null; }
            }

            public Task<int> Count
            {
                get {
                    if (channel == null)
                        return Task.FromResult(0);

                    return channel.CountUser();
                }
            }

            protected Handle(int id, TSyncData data)
            {
                this.handle_id = id;
                this.data = data;
            }
            
            public async Task<bool> AddTo(SyncedChannel<TSyncData> channel)
            {
                if (!await RemoveFromChannel())
                    return false;

                return await channel.sync.InvokeAsync(context =>
                {
                    if (context.participants.TryAdd(handle_id, this))
                    {
                        this.channel = channel;
                        return true;
                    }

                    return false;
                });
            }

            public async Task<bool> RemoveFromChannel()
            {
                if (this.channel != null)
                {
                    return await channel.sync.InvokeAsync(context =>
                    {
                        Handle outUser;
                        if (context.participants.TryRemove(handle_id, out outUser))
                        {
                            this.channel = null;
                            return true;
                        }

                        return false;
                    });
                }

                return true;
            }

            public async Task<bool> ForAllNeighbors(Func<Handle, TSyncData, Task> func)
            {
                if (channel == null)
                    return false;

                await channel.ForAll((handle) => {
                    return func(handle, data);
                });

                return true;
            }
        }

        class SynchronizationContext
        {
            public ConcurrentDictionary<int, Handle> participants = new ConcurrentDictionary<int, Handle>();
        }
        Synchronizer<SynchronizationContext> sync { get; } = new Synchronizer<SynchronizationContext>(new SynchronizationContext());

        public SyncedChannel()
        {
        }

        public Task ForAll(Func<Handle, Task> func)
        {
            return sync.InvokeAsync(async context =>
            {
                foreach (var p in context.participants.Values)
                {
                    await func(p);
                }
            });
        }

        public Task<int> CountUser()
        {
            return sync.InvokeAsync(context =>
            {
                return context.participants.Count;
            });
        }
    }
}
