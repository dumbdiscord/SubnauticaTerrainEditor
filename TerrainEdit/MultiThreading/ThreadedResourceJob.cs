using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainEdit.MultiThreading
{
    public class ThreadedResourceJob<K> : ThreadedJob,IThreadResourceConsumer<K> where K: IThreadResource
    {

        public bool HasResource
        {
            get;
            set;
        }

        public K Resource
        {
            get;
            set;
        }

        
    }
}