using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ASyncJobs
{
    public enum JobMode
    {
        Adjacent, // Adjactent - Each job fires and waits for the work to complete
        Parallel  // Parallel  - All jobs are run in parallel.
    }

    public class JobCollection : List<Job>
    {
        private Stopwatch Timer { get; } = new Stopwatch();

        public async void Run(JobMode mode = JobMode.Parallel)
        {
            var tasks = new List<Task>();
            var count = 0;

            Timer.Start();
            foreach (var job in this)
            {
                if (mode == JobMode.Parallel)
                    tasks.Add(Task.Run(() => job.Start()));
                else
                    await Task.Run(() => job.Start());
                count++;
                job.Index = count;
            }

            if (mode == JobMode.Parallel)
                await Task.WhenAll(tasks);

            Timer.Stop();
            OnCompleted(new JobCollectionEventArgs()
            {
                Count = count,
                Name = this.GetType().ToString(),
                ElapsedMilliseconds = Timer.ElapsedMilliseconds,
            });
        }

        public event EventHandler<JobCollectionEventArgs> JobCompleted;

        protected virtual void OnCompleted(JobCollectionEventArgs e)
        {
            EventHandler<JobCollectionEventArgs> handler = JobCompleted;
            if (handler != null)
                handler(this, e);
        }
    }

    public class JobCollectionEventArgs : EventArgs
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public long ElapsedMilliseconds { get; set; }

        public override string ToString() => $"Job Collection '{Name}' =>\nTime :{ElapsedMilliseconds}ms\n";
    }

    public abstract class Job
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public Stopwatch Timer { get; } = new Stopwatch();

        public void Start()
        {
            Timer.Start();
            var isSuccess = Work();
            Timer.Stop();
            OnCompleted(new JobEventArgs()
            {
                Name = Name,
                ElapsedMilliseconds = Timer.ElapsedMilliseconds,
                IsSuccess = isSuccess
            });
        }

        protected abstract bool Work();

        public event EventHandler<JobEventArgs> JobCompleted;

        protected virtual void OnCompleted(JobEventArgs e)
        {
            EventHandler<JobEventArgs> handler = JobCompleted;
            if (handler != null)
                handler(this, e);
        }
    }

    public class JobEventArgs : EventArgs
    {
        public string Name { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public bool IsSuccess { get; set; }

        public override string ToString()
        {
            var status = IsSuccess ? "Succeeded" : "Failed";
            return $"Job '{Name}' =>\nTime :{ElapsedMilliseconds}ms\nStatus :{status}\n";
        }
    }
}
