using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainEdit.MultiThreading;
namespace TerrainEdit.BatchTools
{
    public class BatchMachine : MonoBehaviour
    {
        ThreadedJobCoordinator<ThreadedBatchLoader> batchLoadingScheduler;
        BatchWorld batchWorld;
        ThreadedQueue<Batch> incomingBatches;
        void StartLoadingBatch(Batch batch)
        {
            if (batch.IsLoading) return;
            batch.IsLoading = true;
            
        }
        // Use this for initialization
        void Start()
        {
            batchLoadingScheduler = new ThreadedJobCoordinator<ThreadedBatchLoader>();
        }
        public void SetBatchToLoad(Batch batch) 
        {
            incomingBatches.Enqueue(batch); 
        }
        // Update is called once per frame
        void Update()
        {
            batchLoadingScheduler.Tick();
        }
    }
}