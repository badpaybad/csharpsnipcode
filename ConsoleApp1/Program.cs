// See https://aka.ms/new-console-template for more information
using System.Collections.Concurrent;

Console.WriteLine("Hello, World!");

ConcurrentBag<Log> logs = new System.Collections.Concurrent.ConcurrentBag<Log>();

async Task<string> DoSth(string name)
{    var start = $"start {name}";
    logs.Add(new Log(Thread.CurrentThread.ManagedThreadId, DateTime.Now.ToString("yyyyMMddHHmmss.fffff"), start, "start"));
    await Task.Delay(1000);
    var end = $"continual {name}";
    logs.Add(new Log(Thread.CurrentThread.ManagedThreadId, DateTime.Now.ToString("yyyyMMddHHmmss.fffff"), end, "continual"));
    return $"[{start} , {end}]";
}

var schedulePair = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Current, 4);

List<Task<string>> tasks = new List<Task<string>>();

foreach (var i in Enumerable.Range(0, 10000))
{
    tasks.Add(Task.Factory.StartNew<Task<string>>(async () => await DoSth($"job_{i}")
    , CancellationToken.None, TaskCreationOptions.None, schedulePair.ConcurrentScheduler).Unwrap());
}

var res = await Task.WhenAll(tasks);
//Console.WriteLine(string.Join(Environment.NewLine, res));

Console.WriteLine("Total threads planing to run in ");
Console.WriteLine(logs.DistinctBy(i => i.threadId).Count());

Console.WriteLine("More than 2 threads start in the same time ");
var r1 = logs.Where(i => i.type == "start").GroupBy(i => i.at, (k, v) => new { at = k, count = v.Count() })
    .Where(i=>i.count>2).ToList();
Console.WriteLine(r1.Count());

Console.WriteLine("More than 2 threads continual part start in the same time ");
var r2 = logs.Where(i => i.type == "continual").GroupBy(i => i.at, (k, v) => new { at = k, count = v.Count() })
    .Where(i => i.count > 2).ToList();
Console.WriteLine(r2.Count());

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
