using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
namespace TerrainEdit.MultiThreading
{
    public class ThreadedJobCoordinator<T>:IThreadedJobCoordinator<T> where T:ThreadedJob
    {
        protected List<T> inProgJobs;
        protected object lockObject = new object();
        protected ThreadedQueue<T> incomingQueue;
        protected ThreadedQueue<T> finishedQueue;
        public ThreadedJobCoordinator()
        {
            inProgJobs = new List<T>();
            incomingQueue = new ThreadedQueue<T>();
            finishedQueue = new ThreadedQueue<T>();
        }
        public void Tick()
        {
            
            while (incomingQueue.Count != 0)
            {
                var job = incomingQueue.Dequeue();
                job.Start();
                job.OnFinishedEvent+=OnFinished;
                inProgJobs.Add(job);
            }
            while (finishedQueue.Count != 0)
            {
                var job = finishedQueue.Dequeue();
                job.OnFinishedMainThread();
                job.OnFinishedEvent -= OnFinished;
                inProgJobs.Remove(job);
                
            }
        }
        public void OnFinished(ThreadedJob job)
        {
            finishedQueue.Enqueue((T)job);
        }
        public void EnqueueJob(T val)
        {
            
            incomingQueue.Enqueue(val);
            
        }
    }
}
