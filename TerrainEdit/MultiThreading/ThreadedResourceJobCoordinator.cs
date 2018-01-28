using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainEdit.MultiThreading
{
    public class ThreadedResourceJobCoordinator<T,K> :IThreadedJobCoordinator<T> where T : ThreadedResourceJob<K> where K:IThreadResource
    {
        object lockObject;
        public Queue<T> awaitingResources;
        ThreadResourceProvider<K> resourceProvider;
        public ThreadedQueue<T> incomingJobs;
        ThreadedQueue<T> finishedQueue;
        public ThreadedResourceJobCoordinator(ThreadResourceProvider<K> provider){
            resourceProvider = provider;
            lockObject = new object();
            awaitingResources = new Queue<T>();
            finishedQueue = new ThreadedQueue<T>();
            incomingJobs = new ThreadedQueue<T>();
        }
        

        public void Tick()
        {
            while (incomingJobs.Count != 0)
            {
                var job = incomingJobs.Dequeue();
                awaitingResources.Enqueue(job);
                //Debug.Log("Scheduling Job...");
            }
            while (resourceProvider.HasFreeResources && awaitingResources.Count != 0)
            {
                var job = awaitingResources.Dequeue();
                resourceProvider.TryAssignResource(job);
                job.OnFinishedEvent += OnFinished;
                //Debug.Log("Giving Resources and starting");
                job.Start();
            }
            //int i = 0;
            while (finishedQueue.Count != 0)
            {
                var job = finishedQueue.Dequeue();
                job.OnFinishedMainThread();
                //i++;
                job.OnFinishedEvent -= OnFinished;
                //Debug.Log("Job Finished");
                resourceProvider.TakeResourceFrom(job);
            }
        }

        public void EnqueueJob(T job)
        {
            incomingJobs.Enqueue(job);
        }

        public void OnFinished(ThreadedJob job)
        {
            finishedQueue.Enqueue((T)job);
        }
    }
}