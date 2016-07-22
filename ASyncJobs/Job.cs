using System;
using System.Diagnostics;

namespace ASyncJobs
{
    public enum JobMode
    {
        Adjacent, // Adjactent - Each job fires and waits for the work to complete
        Parallel  // Parallel  - All jobs are run in parallel.
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
}
