using System.Runtime.Serialization.Json;
using System.Threading;
using System;

namespace csharpsnipcode
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryMessageBus.Instance.Subscribe<string>("channel1", "subscriber1", (id) =>
            {
                Console.WriteLine("channel1 subscriber1: " + id);
            });

            new Thread(() =>
                      {
                          var counter = 0;
                          while (true)
                          {
                              var data = MemoryMessageBus.Instance.Dequeue<string>("queuetest");
                              Console.WriteLine(data);
                              counter++;
                              Thread.Sleep(1000);
                          }
                      }).Start();

                    MemoryMessageBus.Instance.Subscribe<string>("channel1", "subscriber2", (id) =>
                      {
                          Console.WriteLine("channel1 subscriber2: " + id);
                      });

            //------

            new Thread(() =>
            {
                var counter = 0;
                while (true)
                {
                    var data = MemoryMessageBus.Instance.Pop<string>("stacktest");
                    Console.WriteLine(data);
                    counter++;
                    Thread.Sleep(1000);
                }
            }).Start();


            new Thread(() =>
            {

                var counter = 0;

                while (true)
                {
                    MemoryMessageBus.Instance.Publish<string>("channel1", "Temp1: " + counter);

                    MemoryMessageBus.Instance.Enqueue<string>("queuetest", "queue1: " + counter);

                    MemoryMessageBus.Instance.Push<string>("stacktest", "stack1: " + counter);

                    counter++;
                    Thread.Sleep(1000);
                }

            }).Start();

            //------

            MemoryMessageBus.Instance.CacheSetUseSlideExpire("TestSlideExpire", DateTime.Now.ToString(), new TimeSpan(0, 0, 3));

            new Thread(() =>
            {
                Console.WriteLine("Before 3 seconds TestSlideExpire: " + MemoryMessageBus.Instance.CacheGet<string>("TestSlideExpire"));

                Thread.Sleep(5000);

                Console.WriteLine("After 5 seconds TestSlideExpire: " + MemoryMessageBus.Instance.CacheGet<string>("TestSlideExpire"));

                Console.WriteLine("Un subscribe channel1 by subscriber2");
                MemoryMessageBus.Instance.Unsubscribe("channel1", "subscriber2");
            }).Start();


            MemoryMessageBus.Instance.CacheSetUseSlideExpire("TestSlideExpire3Seconds", DateTime.Now.ToString(), new TimeSpan(0, 0, 3));

            new Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine("After 1 seconds TestSlideExpire3Seconds: " + MemoryMessageBus.Instance.CacheGet<string>("TestSlideExpire3Seconds"));
                    Thread.Sleep(1000);
                }
            }).Start();


            new Thread(() =>
            {
                Thread.Sleep(10000);
                Console.WriteLine("Clear all");
                MemoryMessageBus.Instance.ClearAll();

                var listKey = MemoryMessageBus.Instance.ListAllKey();

                Console.WriteLine("Count all key: " + listKey.Count);
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(listKey));

            }).Start();

            MemoryMessageBus.Instance.HashSet<string>("hashset1", "field1", "data1");

            var field1Val = MemoryMessageBus.Instance.HashGet<string>("hashset1", "field1");
            Console.WriteLine("hashset1:field1: " + field1Val);

            Console.ReadLine();

        }
    }
}
