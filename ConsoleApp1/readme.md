# ConcurrentExclusiveSchedulerPair
test if allow run conccurent task with maxLevel set. c# 9.0,  dotnet 6.0

				
				string formatDate = "yyyyMMddHHmmss.fffff";
				group by time start task.
				fff or ffff .. less than 5 "f" group by time will give count result more than maxLevel set


				Console.WriteLine($"More than {maxLevel} threads continual part start in the same time ");
				var r2 = logs.Where(i => i.type == "continual").GroupBy(i => i.at, (k, v) => new { at = k, count = v.Count() })
					.Where(i => i.count > maxLevel).ToList();
				Console.WriteLine(r2.Count());