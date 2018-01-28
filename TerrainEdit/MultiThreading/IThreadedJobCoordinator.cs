using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainEdit.MultiThreading
{
    public interface IThreadedJobCoordinator<K> where K:ThreadedJob
    {
        void Tick();
        void EnqueueJob(K job);
        void OnFinished(ThreadedJob job);
    }
}