# ASync Jobs

 A class that can make a sync tasks much easier.
 
 
## Creating a job
```
class ExampleJob : Job
{
  protected override bool Work()
  {
    for (var i = 0; i < 100000; i++)
      Console.WriteLine(i.ToString());
    return true;
  }
}
```

## Executing Jobs
```
var jobs = new JobCollection();
var job = new ExampleJob();
jobs.Add(job);
jobs.Run();
```
