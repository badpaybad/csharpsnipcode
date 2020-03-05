using System.Threading;
using System;

namespace csharpsnipcode
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryMessageBuss.Instance.Subscribe<string>("channel1", "subscriber1", (id) =>
            {
                Console.WriteLine("channel1 subscriber1: " + id);
            });

            new Thread(() =>
                      {
                          var counter = 0;
                          while (true)
                          {
                              var data = MemoryMessageBuss.Instance.Dequeue<string>("queuetest");
                              Console.WriteLine(data);
                              counter++;
                              Thread.Sleep(1000);
                          }
                      }).Start();

            MemoryMessageBuss.Instance.Subscribe<string>("channel1", "subscriber2", (id) =>
                      {
                          Console.WriteLine("channel1 subscriber2: " + id);
                      });

            //------

            new Thread(() =>
            {
                var counter = 0;
                while (true)
                {
                    var data = MemoryMessageBuss.Instance.Pop<string>("stacktest");
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
                    MemoryMessageBuss.Instance.Publish<string>("channel1", "Temp1: " + counter);

                    MemoryMessageBuss.Instance.Enqueue<string>("queuetest", "queue1: " + counter);

                    MemoryMessageBuss.Instance.Push<string>("stacktest", "stack1: " + counter);

                    counter++;
                    Thread.Sleep(1000);

                    
                }

            }).Start();

            //------

            new Thread(()=>{
                Thread.Sleep(5000);
                Console.WriteLine("Un subscribe channel1 by subscriber2");
                MemoryMessageBuss.Instance.Unsubscribe("channel1", "subscriber2");
            }).Start();

            Console.ReadLine();

        }
    }
}
