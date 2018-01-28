using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainEdit.MultiThreading
{
    public class ThreadedQueue<T>:ICollection
    {
        
        object lockObject;
        Queue<T> queue;
        public ThreadedQueue()
            : base()
        {
            lockObject = new object();
            queue=new Queue<T>();
        }
        public void Enqueue(T val)
        {
            lock (lockObject)
            {
                queue.Enqueue(val);
            }
        }
        public T Dequeue()
        {
            lock (lockObject)
            {
                return queue.Dequeue();
            }
        }
        public void CopyTo(System.Array array, int index)
        {
            throw new System.NotImplementedException();
        }

        public int Count { get { return queue.Count; } }


        public bool IsSynchronized
        {
            get { throw new System.NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new System.NotImplementedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
        
    }
}