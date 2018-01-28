using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainEdit.MultiThreading;
using TerrainEdit.DualContouring;
using TerrainEdit.BatchTools;
using System;
public class GeneratorTest : MonoBehaviour {
    ThreadedResourceJobCoordinator<ThreadedCubePolygonizer, CubePolygonizerResource> coordinator;
    ThreadResourceProvider<CubePolygonizerResource> provider;
    CubeGrid grid;
    //IDataProvider data;
    DateTime now;
    public GameObject gameObjecter;
    public string path2 = "C:/Program Files (x86)/Steam/steamapps/common/Subnautica/SNUnmanagedData/Build18/CompiledOctreesCache";

    //public MeshFilter b;
	void Awake () {
        //Debug.Log(coordinator.incomingJobs.Count);
        grid = new CubeGrid(36, 36, 36);
        BatchWorld world = new BatchWorld();
        world.BatchPath = path2;
        //var cube = new CubeGridData(grid);
        //var cube2 = new CubeGridData(grid);
        now = DateTime.Now;
        provider = new ThreadResourceProvider<CubePolygonizerResource>();
        for (int i = 0; i < 4; i++)
        {
            provider.AddResource(new CubePolygonizerResource(new CubeGridData(grid)));
        }
        
        //var data2 = new PerlinNoiseProvider(a);
        coordinator = new ThreadedResourceJobCoordinator<ThreadedCubePolygonizer, CubePolygonizerResource>(provider);
        world.GetDataAtPos(transform.position);
        var mytree = world.Batches[world.GetBatchIndexFromPos(transform.position)];
        
        var batchdata = new BatchWorldDataProvider(world, transform.position);
        for (int x = 0; x < 30; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                for (int z = 0; z < 30; z++)
                {
                    var newpos = transform.position + new Vector3(x, y, z) * 32 - new Vector3(80, 80, 80);
                    world.GetDataAtPos(newpos);
                    var h = mytree.GetOctreeAt(newpos);
                    if (h == null || !h.IsEmpty)
                    {
                        //if (h != null)
                       // {
                            var g = Instantiate(gameObjecter);
                            g.transform.parent = transform;
                            g.transform.position = newpos;
                            var mesh = new Mesh();
                            g.GetComponent<MeshFilter>().mesh = mesh;
                            coordinator.EnqueueJob(new ThreadedCubePolygonizer(mesh, new BatchWorldDataProvider(world, newpos)));
                        //}
                        //g.GetComponent<CubeTester>().grid = grid;
                        //g.GetComponent<CubeTester>().batchworld = world;
                    }
                }
            }
        }
        
        //coordinator.EnqueueJob(new ThreadedCubePolygonizer(mesh2, data2));
        //var h = new Queue<int>();
        
        
	}
    bool finished;
	void Update () {
        coordinator.Tick();
        if (coordinator.awaitingResources.Count == 0&&!finished)
        {
            Debug.Log("Finished after "+(DateTime.Now-now).TotalSeconds+" seconds");
            finished = true;
        }
	}
}
