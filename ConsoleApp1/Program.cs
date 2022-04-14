// See https://aka.ms/new-console-template for more information
using System.Collections.Concurrent;

Console.WriteLine("Hello, World!");

ConcurrentBag<Log> logs = new System.Collections.Concurrent.ConcurrentBag<Log>();
var maxLevel = 2;
string formatDate = "yyyyMMddHHmmss.fff";
async Task<string> DoSth(string name)
{
    var start = $"start {name}";
    logs.Add(new Log(Thread.CurrentThread.ManagedThreadId, DateTime.Now.ToString(formatDate), start, "start"));
    await Task.Delay(1000);
    var end = $"continual {name}";
    logs.Add(new Log(Thread.CurrentThread.ManagedThreadId, DateTime.Now.ToString(formatDate), end, "continual"));
    Thread.Sleep(1000);    
    //await Task.Delay(1000);
    return $"[{start} , {end} , {DateTime.Now.ToString(formatDate)}]";
}
var schedulePair = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Current, maxLevel);
List<Task<string>> tasks = new List<Task<string>>();
DateTime tstart = DateTime.Now;
int taskTodo = 20;
foreach (var i in Enumerable.Range(0, taskTodo))
{
    tasks.Add(Task.Factory.StartNew<Task<string>>(async () => await DoSth($"job_{i}")
    , CancellationToken.None, TaskCreationOptions.None, schedulePair.ConcurrentScheduler).Unwrap());
}
var res = await Task.WhenAll(tasks);
DateTime tend = DateTime.Now;
Console.WriteLine( $"Expected {taskTodo/ maxLevel} milisec, actual {(tend - tstart).TotalMilliseconds}");
//Console.WriteLine(string.Join(Environment.NewLine, res));
Console.WriteLine($"More than {maxLevel} threads continual part start in the same time ");
var r2 = logs.Where(i => i.type == "continual").GroupBy(i => i.at, (k, v) => new { at = k, count = v.Count() })
    .Where(i => i.count > maxLevel).ToList();
Console.WriteLine(r2.Count());

Console.WriteLine($"More than {maxLevel} threads start in the same time ");
var r1 = logs.Where(i => i.type == "start").GroupBy(i => i.at, (k, v) => new { at = k, count = v.Count() })
    .Where(i => i.count > maxLevel).ToList();
Console.WriteLine(r1.Count());

Console.WriteLine("Total threads planing to run in ");
Console.WriteLine(logs.DistinctBy(i => i.threadId).Count());

schedulePair.Complete();

await schedulePair.Completion;

Console.ReadLine();

record Log(int threadId, string at, string msg, string type)
{
    public override string ToString()
    {
        return $"type {type} th:{threadId} ti:{at} ms:{msg}";
    }
}
