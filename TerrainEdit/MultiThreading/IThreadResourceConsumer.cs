using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainEdit.MultiThreading
{
    public interface IThreadResourceConsumer<T> where T:IThreadResource
    {
        bool HasResource { get; set; }
        T Resource { get; set; }
    }
   
    
}
