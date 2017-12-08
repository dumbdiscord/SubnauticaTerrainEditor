using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainEdit.DualContouring
{
    public class GridMeshBuilder {
        CubeGrid cube;
        List<Vector3> verts;
        List<int> tris;
        public GridMeshBuilder(int xsize, int ysize, int zsize, IDataProvider provider)
        {
            cube = new CubeGrid(xsize, ysize, zsize, provider);
            verts = new List<Vector3>();
            tris = new List<int>();
        }
        void AddMesh(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
        {
            var count = verts.Count;
            var tri = tris.Count;
            verts.Add(a1);
            verts.Add(a2);
            verts.Add(b1);
            verts.Add(b2);
            tris.Add(count);
            tris.Add(count + 1);
            tris.Add(count + 2);
            tris.Add(count + 1);
            tris.Add(count + 3);
            tris.Add(count + 2);

        }
        void AddMesh(Cube a1, Cube a2, Cube b1, Cube b2)
        {
            var count = verts.Count;
            if (a1.CurVertIndex == -1)
            {
                a1.CurVertIndex = count++;
                verts.Add(a1.VertexPoint - new Vector3(1, 1, 1) / 2f);
            }

            if (a2.CurVertIndex == -1)
            {
                a2.CurVertIndex = count++;
                verts.Add(a2.VertexPoint - new Vector3(1, 1, 1) / 2f);
            }

            if (b1.CurVertIndex == -1)
            {
                b1.CurVertIndex = count++;
                verts.Add(b1.VertexPoint - new Vector3(1, 1, 1) / 2f);
            }

            if (b2.CurVertIndex == -1)
            {
                b2.CurVertIndex = count++;
                verts.Add(b2.VertexPoint - new Vector3(1, 1, 1) / 2f);

            }
            tris.Add(a1.CurVertIndex);
            tris.Add(a2.CurVertIndex);
            tris.Add(b1.CurVertIndex);
            tris.Add(a2.CurVertIndex);
            tris.Add(b2.CurVertIndex);
            tris.Add(b1.CurVertIndex);

        }
        public void ComputeMesh()
        {
            verts.Clear();
            tris.Clear();
            foreach (var edge in cube.Edges)
            {
                var vec = cube.GetPointCoords(edge.pointA);
                if (vec.x > 0 && vec.y > 0 && vec.z > 0 && vec.x < cube.xsize && vec.y < cube.ysize && vec.z < cube.zsize)
                    if (edge.signChange)
                    {
                        if (edge.cubes.Count != 4) continue;


                        if (edge.reverse)
                        {
                            AddMesh(cube.Cubes[edge.cubes[0]], cube.Cubes[edge.cubes[1]], cube.Cubes[edge.cubes[2]], cube.Cubes[edge.cubes[3]]);
                            //AddMesh(cube.Cubes[edge.cubes[0]].VertexPoint, cube.Cubes[edge.cubes[1]].VertexPoint, cube.Cubes[edge.cubes[2]].VertexPoint, cube.Cubes[edge.cubes[3]].VertexPoint);
                        }
                        else
                        {
                            AddMesh(cube.Cubes[edge.cubes[2]], cube.Cubes[edge.cubes[3]], cube.Cubes[edge.cubes[0]], cube.Cubes[edge.cubes[1]]);
                            //AddMesh(cube.Cubes[edge.cubes[2]].VertexPoint, cube.Cubes[edge.cubes[3]].VertexPoint, cube.Cubes[edge.cubes[0]].VertexPoint, cube.Cubes[edge.cubes[1]].VertexPoint);
                        }
                    }


            }
        }
        public void AssignMesh(Mesh mesh)
        {
            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
        }
    }
}
