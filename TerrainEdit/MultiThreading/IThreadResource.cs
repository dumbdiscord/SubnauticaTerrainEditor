using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainEdit.MultiThreading
{
    public interface IThreadResource
    {
        //bool IsBeingUsed { get; set; }
        void Reset();
        //ThreadResourceProvider<IThreadResource> Provider { get; set; }
        //void ReturnToProvider();
    }
}
