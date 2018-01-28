using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainEdit.DualContouring;
namespace TerrainEdit.MultiThreading
{
    public class ThreadedBatchRenderer:ThreadedResourceJob<BatchRenderResource>
    {
           
    }
    public class BatchRenderResource:IThreadResource
    {
        public CubeGrid grid;
        public void Reset()
        {
           
        }
    }
}