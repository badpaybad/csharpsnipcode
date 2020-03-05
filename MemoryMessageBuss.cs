using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace csharpsnipcode
{
    public class MemoryMessageBuss
    {
        static MemoryMessageBuss _instance;
        public static MemoryMessageBuss Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new MemoryMessageBuss();
                return _instance;
            }
        }

        ConcurrentDictionary<string, ConcurrentQueue<object>>
             _queue = new ConcurrentDictionary<string, ConcurrentQueue<object>>();

        ConcurrentDictionary<string, ConcurrentStack<object>>
            _stack = new ConcurrentDictionary<string, ConcurrentStack<object>>();

        ConcurrentDictionary<string, object>
            _cache = new ConcurrentDictionary<string, object>();

        ConcurrentDictionary<string, ConcurrentDictionary<string, Action<object>>>
            _channelSubscriber = new ConcurrentDictionary<string, ConcurrentDictionary<string, Action<object>>>();
        ConcurrentDictionary<string, bool> _channelTypeIsQueue = new ConcurrentDictionary<string, bool>();

        ConcurrentDictionary<string, DateTime?> _keyExpire = new ConcurrentDictionary<string, DateTime?>();

        Thread _channelThread;
        private MemoryMessageBuss()
        {
            _channelThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {

                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            List<KeyValuePair<string, ConcurrentDictionary<string, Action<object>>>> tempChannel;
                            lock (_channelSubscriber)
                            {
                                tempChannel = _channelSubscriber.ToList();
                            }
                            foreach (var itm in tempChannel)
                            {
                                ThreadPool.QueueUserWorkItem((o) =>
                                {
                                    if (_channelTypeIsQueue.ContainsKey(itm.Key) && itm.Value.Count > 0)
                                    {
                                        var channelType = _channelTypeIsQueue[itm.Key];
                                        object data = channelType ? Dequeue(itm.Key) : Pop(itm.Key);
                                        if (data != null)
                                        {
                                            List<KeyValuePair<string, Action<object>>> tempSubscribe;
                                            lock (itm.Value)
                                            {
                                                tempSubscribe = itm.Value.ToList();
                                            }

                                            foreach (var subscriber in tempSubscribe)
                                            {
                                                ThreadPool.QueueUserWorkItem((o) =>
                                                {
                                                    subscriber.Value(data);
                                                });
                                            }
                                        }
                                    }
                                });
                            }

                        });

                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            List<KeyValuePair<string, DateTime?>> tempExpired;

                            lock (_keyExpire)
                            {
                                tempExpired = _keyExpire.ToList();
                            }

                            var listExpired = tempExpired.Where(i => i.Value != null && i.Value.Value < DateTime.Now).ToList();

                            foreach (var item in listExpired)
                            {
                                _cache.TryRemove(item.Key, out object cacheVal);

                                _queue.TryRemove(item.Key, out ConcurrentQueue<object> queueVal);

                                _stack.TryRemove(item.Key, out ConcurrentStack<object> stackVal);

                                _channelSubscriber.TryRemove(item.Key, out ConcurrentDictionary<string, Action<object>> oldChannelVal);

                                _channelTypeIsQueue.TryRemove(item.Key, out bool oldTypeVal);

                                _keyExpire.TryRemove(item.Key, out DateTime? oldExpired);
                            }

                        });
                    }
                    catch
                    {
                    }
                    finally
                    {
                        Thread.Sleep(1);
                    }
                }

            });

            _channelThread.Start();
        }

        public void Set<T>(string key, T data, DateTime? expireAt = null)
        {
            _cache[key] = data;

            SetExpire(key, expireAt);
        }

        public T Get<T>(string key)
        {
            return _cache.TryGetValue(key, out object val) && val != null ? (T)val : default(T);
        }

        public void SetExpire(string key, DateTime? expireAt = null)
        {
            if (expireAt == null)
            {
                _keyExpire.TryRemove(key, out expireAt);
            }
            else
            {
                _keyExpire[key] = expireAt;
            }
        }

        public void Enqueue<T>(string queueName, T data)
        {
            if (!_queue.TryGetValue(queueName, out ConcurrentQueue<object> queueData))
            {
                queueData = new ConcurrentQueue<object>();
                _queue[queueName] = queueData;
            }

            queueData.Enqueue(data);
        }

        public object Dequeue(string queueName)
        {
            if (_queue.TryGetValue(queueName, out ConcurrentQueue<object> queueData) && queueData != null)
            {
                if (queueData.TryDequeue(out object data) && data != null)
                {
                    return data;
                }
            }

            return null;
        }

        public T Dequeue<T>(string queueName)
        {
            var data = Dequeue(queueName);
            return data == null ? default(T) : (T)data;
        }

        public void Push<T>(string stackName, T data)
        {
            if (!_stack.TryGetValue(stackName, out ConcurrentStack<object> stackData))
            {
                stackData = new ConcurrentStack<object>();
                _stack[stackName] = stackData;
            }

            stackData.Push(data);
        }
        object Pop(string stackName)
        {
            if (_stack.TryGetValue(stackName, out ConcurrentStack<object> stackData) && stackData != null)
            {
                if (stackData.TryPop(out object data) && data != null)
                {
                    return data;
                }
            }

            return null;
        }
        public T Pop<T>(string stackName)
        {
            var data = Pop(stackName);
            return data == null ? default(T) : (T)data;
        }

        public void Publish<T>(string channelName, T data)
        {
            _channelTypeIsQueue[channelName] = true;
            Enqueue(channelName, data);
        }

        public void PublishUseStack<T>(string channelName, T data)
        {
            _channelTypeIsQueue[channelName] = false;
            Push(channelName, data);
        }

        public void Subscribe<T>(string channelName, string subscribeName, Action<T> callback)
        {
            if (!_channelSubscriber.TryGetValue(channelName, out ConcurrentDictionary<string, Action<object>> subscribers))
            {
                subscribers = new ConcurrentDictionary<string, Action<object>>();
                _channelSubscriber[channelName] = subscribers;
            }

            subscribers[subscribeName] = (o) =>
            {
                if (o == null) callback(default(T));
                else callback((T)o);
            };
        }
    }


    /*
     //usage
       MemoryMessageBuss.Instance.Subscribe<string>("channel1", "subscriber1", (id) =>
                 {
                     Console.WriteLine("channel1 subscriber1: " + id);
                 });

           MemoryMessageBuss.Instance.Publish<string>("channel1", "Temp1: " + counter);

        //------

         new Thread(()=> {
                    while (true)
                    {
                        var data = MemoryMessageBuss.Instance.Dequeue<string>("queuetest");
                        Console.WriteLine(data);
                        Thread.Sleep(1000);
                    }
                }).Start();

           MemoryMessageBuss.Instance.Enqueue<string>("queuetest", "queue1: " + counter);

        //------

                new Thread(() => {
                    while (true)
                    {
                        var data = MemoryMessageBuss.Instance.Pop<string>("stacktest");
                        Console.WriteLine(data);
                        Thread.Sleep(1000);
                    }
                }).Start();

                  MemoryMessageBuss.Instance.Push<string>("stacktest", "stack1: "+ counter);

         */


}