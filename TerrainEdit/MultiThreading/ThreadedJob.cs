using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
namespace TerrainEdit.MultiThreading{
public class ThreadedJob {
    public delegate void JobFinished(ThreadedJob job);
    protected Thread _thread;
    protected bool isdone;
    public event JobFinished OnFinishedEvent;
    public bool Finished
    {
        get
        {
            return isdone;
        }
    }
    public virtual void ThreadCode(){

    }
    void Run()
    {
        ThreadCode();
        
        isdone = true;
        OnFinishedEvent(this);
        
    }
    public virtual void OnFinishedMainThread() { }
    public void Start()
    {
        _thread = new Thread(Run);
        _thread.Start();
    }
}
}