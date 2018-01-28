using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainEdit.MultiThreading
{
    public class ThreadResourceProvider<K> where K:IThreadResource                                  
    {

        ThreadedQueue<K> availableResources;
        
        public ThreadResourceProvider()
        {
            availableResources = new ThreadedQueue<K>();
        }
       // public void AddResource
        public bool HasFreeResources
        {
            get {return availableResources.Count != 0;}
        }
        public bool TryAssignResource(IThreadResourceConsumer<K> consumer)
        {
            
            if(!HasFreeResources||consumer.HasResource) return false;
            var resource =availableResources.Dequeue();
            resource.Reset();
            consumer.HasResource = true;
            consumer.Resource = resource;
            
            return true;
        }
        public void TakeResourceFrom(IThreadResourceConsumer<K> consumer)
        {
            if (!consumer.HasResource) return;
            availableResources.Enqueue(consumer.Resource);
            consumer.HasResource = false;
            consumer.Resource = default(K);
        }
        public void AddResource(K resource)
        {
            availableResources.Enqueue(resource);
        }
    }
}