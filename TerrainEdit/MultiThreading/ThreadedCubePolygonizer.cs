using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainEdit.DualContouring;
using TerrainEdit.BatchTools;
namespace TerrainEdit.MultiThreading
{
    public class ThreadedCubePolygonizer : ThreadedResourceJob<CubePolygonizerResource>
    {
        GridMeshBuilder builder;
        IDataProvider dataprovider;
        Mesh mesh;
        public override void ThreadCode()
        {
            builder = new GridMeshBuilder(Resource.grid, dataprovider);
            builder.CatmullClark();
        }
        public override void OnFinishedMainThread()
        {
            builder.AssignMesh(mesh);
            builder = null;
            //GetComponent<MeshCollider>().sharedMesh = mesh;
            mesh.RecalculateNormals();
        }
        public ThreadedCubePolygonizer(Mesh mesh,IDataProvider data)
        {
            this.mesh = mesh;
            this.dataprovider = data;
        }
    }
    public class CubePolygonizerResource : IThreadResource
    {
        public CubeGridData grid;
        public CubePolygonizerResource(CubeGridData cube){
            grid= cube;
        }

        public void Reset()
        {
            
        }
    }
}