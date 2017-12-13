using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace TerrainEdit.DualContouring
{
    public class GridMeshBuilder
    {
        CubeGrid cube;
        List<Vector3> verts;
        List<int> tris;
        CatmullManager catmull;
        public GridMeshBuilder(int xsize, int ysize, int zsize, IDataProvider provider)
        {
            cube = new CubeGrid(xsize, ysize, zsize);
            cube.PopulateGrid(provider);
            verts = new List<Vector3>();
            tris = new List<int>();
        }
        public GridMeshBuilder(CubeGrid cube, IDataProvider provider)
        {
            var now = DateTime.Now;
            verts = new List<Vector3>();
            tris = new List<int>();
            this.cube = cube;
            cube.PopulateGrid(provider);
            //Debug.Log((DateTime.Now-now).TotalSeconds + " to initialize gridmeshbuilder");
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
            var tricount = tris.Count;
            if (a1.CurVertIndex == -1)
            {
                a1.CurVertIndex = count++;
                verts.Add(a1.VertexPoint);// - new Vector3(1, 1, 1) / 2f);
            }

            if (a2.CurVertIndex == -1)
            {
                a2.CurVertIndex = count++;
                verts.Add(a2.VertexPoint);// - new Vector3(1, 1, 1) / 2f);
            }

            if (b1.CurVertIndex == -1)
            {
                b1.CurVertIndex = count++;
                verts.Add(b1.VertexPoint);// - new Vector3(1, 1, 1) / 2f);
            }

            if (b2.CurVertIndex == -1)
            {
                b2.CurVertIndex = count++;
                verts.Add(b2.VertexPoint);// - new Vector3(1, 1, 1) / 2f);

            }

            tris.Add(a1.CurVertIndex);
            tris.Add(a2.CurVertIndex);
            tris.Add(b1.CurVertIndex);
            tris.Add(a2.CurVertIndex);
            tris.Add(b2.CurVertIndex);
            tris.Add(b1.CurVertIndex);

        }
        void AssignVerts(Cube a1, Cube a2, Cube b1, Cube b2)
        {
            var count = verts.Count;
            var tricount = tris.Count;
            if (a1.CurVertIndex == -1)
            {
                a1.CurVertIndex = count++;
                verts.Add(a1.VertexPoint);// - new Vector3(1, 1, 1) / 2f);
            }

            if (a2.CurVertIndex == -1)
            {
                a2.CurVertIndex = count++;
                verts.Add(a2.VertexPoint);// - new Vector3(1, 1, 1) / 2f);
            }

            if (b1.CurVertIndex == -1)
            {
                b1.CurVertIndex = count++;
                verts.Add(b1.VertexPoint);// - new Vector3(1, 1, 1) / 2f);
            }

            if (b2.CurVertIndex == -1)
            {
                b2.CurVertIndex = count++;
                verts.Add(b2.VertexPoint);// - new Vector3(1, 1, 1) / 2f);

            }



        }
        public void ComputeMesh()
        {
            verts.Clear();
            tris.Clear();
            foreach (var edge in cube.Edges)
            {
                var vec = cube.GetPointCoords(edge.pointA);
                if (vec.x > 0 && vec.y > 0 && vec.z > 0)// && vec.x < cube.xsize && vec.y < cube.ysize && vec.z < cube.zsize)
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
        public void CatmullClark()
        {
            var now = DateTime.Now;
            catmull = new CatmullManager(cube);


            verts.Clear();
            tris.Clear();
            foreach (var edge in cube.Edges)
            {
                var vec = cube.GetPointCoords(edge.pointA);
                // && vec.x < cube.xsize-1 && vec.y < cube.ysize-1 && vec.z < cube.zsize-1
                if ((vec.x > 1 && vec.y > 1 && vec.z > 1) && (vec.x < cube.xsize - 2 && vec.y < cube.ysize - 2 && vec.z < cube.zsize - 2))//if (vec.x > 0 && vec.y > 0 && vec.z > 0)// && vec.x < cube.xsize && vec.y < cube.ysize && vec.z < cube.zsize)
                    if (edge.signChange)
                    {
                        if (edge.cubes.Count != 4) continue;


                        if (edge.reverse)
                        {
                            AssignVerts(cube.Cubes[edge.cubes[0]], cube.Cubes[edge.cubes[1]], cube.Cubes[edge.cubes[2]], cube.Cubes[edge.cubes[3]]);
                            //AddMesh(cube.Cubes[edge.cubes[0]].VertexPoint, cube.Cubes[edge.cubes[1]].VertexPoint, cube.Cubes[edge.cubes[2]].VertexPoint, cube.Cubes[edge.cubes[3]].VertexPoint);
                        }
                        else
                        {
                            AssignVerts(cube.Cubes[edge.cubes[2]], cube.Cubes[edge.cubes[3]], cube.Cubes[edge.cubes[0]], cube.Cubes[edge.cubes[1]]);
                            //AddMesh(cube.Cubes[edge.cubes[2]].VertexPoint, cube.Cubes[edge.cubes[3]].VertexPoint, cube.Cubes[edge.cubes[0]].VertexPoint, cube.Cubes[edge.cubes[1]].VertexPoint);
                        }
                    }


            }
            //Debug.Log((DateTime.Now - now).TotalSeconds+" for making fake verts");
            catmull.Init();
            catmull.CalculateExtraVerts(verts);
            now = DateTime.Now;
            foreach (var edge in cube.Edges)
            {

                if (edge.signChange)
                {
                    var vec = cube.GetPointCoords(edge.pointA);
                    if ((vec.x > 1 && vec.y > 1 && vec.z >1) && (vec.x < cube.xsize - 2 && vec.y < cube.ysize - 2 && vec.z < cube.zsize - 2))
                    {
                        if (edge.cubes.Count < 4) continue;
                        AddCatmullMesh(edge);
                    }
                }
            }
            //Debug.Log((DateTime.Now - now).TotalSeconds+" for Catmulling");
        }
        void AddCatmullMesh(Edge centeredge)
        {
            var count = verts.Count;
            var tricount = tris.Count;
            
            var CatmullVert = centeredge.CatmullFacePoint;
            var catmullvertindex = verts.Count; 
            verts.Add(CatmullVert);
            var cube = centeredge.cube;
            catmull.ResolveCatmullEdges(centeredge,verts);
            
            if (centeredge.reverse)
            {
                
                AddQuad(cube.Cubes[centeredge.cubes[0]].CurVertIndex, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[1]).Index], catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[2]).Index], catmullvertindex);
                AddQuad(catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[1]).Index], cube.Cubes[centeredge.cubes[1]].CurVertIndex, catmullvertindex, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[1], centeredge.cubes[3]).Index]);
                AddQuad(catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[2]).Index], catmullvertindex, cube.Cubes[centeredge.cubes[2]].CurVertIndex, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[2], centeredge.cubes[3]).Index]);
                AddQuad(catmullvertindex, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[1], centeredge.cubes[3]).Index], catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[2], centeredge.cubes[3]).Index], cube.Cubes[centeredge.cubes[3]].CurVertIndex);
            }
            else
            {
                //Debug.Log(verts[cube.Cubes[centeredge.cubes[0]].CurVertIndex] + " " + verts[catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[1]).Index]] + " " + verts[catmullvertindex] + " " +verts[ catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[2]).Index]]);
                AddQuad(cube.Cubes[centeredge.cubes[0]].CurVertIndex, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[1]).Index],catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[2]).Index], catmullvertindex,true);
                AddQuad(catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[1]).Index], cube.Cubes[centeredge.cubes[1]].CurVertIndex, catmullvertindex, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[1], centeredge.cubes[3]).Index],true);
                AddQuad(catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[2]).Index], catmullvertindex, cube.Cubes[centeredge.cubes[2]].CurVertIndex, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[2], centeredge.cubes[3]).Index],true);
                AddQuad(catmullvertindex, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[1], centeredge.cubes[3]).Index], catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[2], centeredge.cubes[3]).Index], cube.Cubes[centeredge.cubes[3]].CurVertIndex,true);
            }
        }
        void AddQuad(int v1, int v2, int v3, int v4, bool reverse = false)
        {
            if (v1 == v2 || v1 == v3 || v1 == v4 || v2==v3||v2==v4||v3==v4)
            {
                throw new Exception(v1 + " " + v2 + " " + v3 + " " + v4);
            }
            if (!reverse)   
            {
                tris.Add(v1);
                tris.Add(v2);
                tris.Add(v3);
                tris.Add(v2);
                tris.Add(v4);
                tris.Add(v3);
            }
            else
            {
                // v1, v2 ,v3 ,v4 = v3, v4, v1,v2
                tris.Add(v3);
                tris.Add(v4);
                tris.Add(v1);
                tris.Add(v4);
                tris.Add(v2);
                tris.Add(v1);
            }
        }
        public void AssignMesh(Mesh mesh)
        {
            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            //mesh.RecalculateNormals(cube);
            verts.Clear();
            tris.Clear();
            cube = null;

        }
    }
    public class CatmullManager
    {
        public int[] ExtraCatmullVerts;
        public CubeGrid grid;
        public CatmullManager(CubeGrid grid)
        {
            this.grid = grid;
        }
        public void Init()
        {
            ExtraCatmullVerts = new int[grid.Edges.Length];
        }

        public void CalculateExtraVerts(List<Vector3> verts)
        {
            var now = DateTime.Now;
            foreach (var edge in grid.Edges)
            {
                if (edge.signChange)
                {
                    var h = Vector3.zero;
                    int counter = 0;
                    foreach(var i in edge.cubes)
                    {
                        
                        h += grid.Cubes[i].VertexPoint;
                        counter++;
                    }
                    if (counter == 0) continue;
                    h /= counter;
                    edge.CatmullFacePoint = h;
                }
            }
            foreach (var edge in grid.Edges)
            {

                if (IsCatmullEdge(edge))
                    {
                        var q = Vector3.zero;
                        
                        var pointcoords = grid.GetPointCoords(edge.pointB);
                        var cubeb=grid.Cubes[grid.GetCubeIndex((int)pointcoords.x, (int)pointcoords.y, (int)pointcoords.z)];
                        
                        pointcoords = grid.GetPointCoords(edge.pointA);
                        var cubea= grid.Cubes[grid.GetCubeIndex((int)pointcoords.x, (int)pointcoords.y, (int)pointcoords.z)];
                        

                        if (!cubea.signChange || !cubeb.signChange) continue;
                        q += cubea.VertexPoint;
                        q += cubeb.VertexPoint;
                        int i = 2;

                        int[] list = cubea.Edges.Where(x => cubeb.Edges.Contains(x) && grid.Edges[x].signChange&&IsRenderedEdge(grid.Edges[x])).ToArray();
                        if (list.Length % 2 == 0)
                        {
                            foreach (var c in list)
                            {

                                 q += grid.Edges[c].CatmullFacePoint;
                                 i++;
                            }
                        }
                        
                        edge.CatmullEdgePoint = q/i;
                    }
                
            }
            foreach (Cube c in grid.Cubes)
            {
                if (!c.signChange)
                    continue;
                if (c.CurVertIndex == -1) continue;
                var v = grid.GetCubeCoords(c.Index)-new Vector3(1,1,1)/2f;
                int counter = 0;
                int counter1 = 0;
                Vector3 facepoints= Vector3.zero;
                Vector3 edgepoints= Vector3.zero;


                for (int i = 0; i < 6; i++)
                {

                    if(v.x<1||v.x>=grid.xsize-2||v.y<1||v.y>=grid.ysize-2||v.z<1||v.z>=grid.zsize-2)
                        continue;
                    var normvec = Vector3.zero;
                    switch (i)
                    {
                        case 0:
                            normvec = new Vector3(1, 0, 0);
                            break;
                        case 1:
                            normvec = new Vector3(-1, 0, 0);
                            break;
                        case 2:
                            normvec = new Vector3(0, 1, 0);
                            break;
                        case 3:
                            normvec = new Vector3(0, -1, 0);
                            break;
                        case 4:
                            normvec = new Vector3(0, 0, 1);
                            break;
                        case 5:
                            normvec = new Vector3(0, 0, -1);
                            break;
                    }
                    var h = normvec + v;
                    var newcube = grid.Cubes[grid.GetCubeIndex((int)h.x, (int)h.y, (int)h.z)];
                    if (float.IsNaN(newcube.VertexPoint.x)) continue;
                    if (!newcube.signChange)
                    {
                        continue;
                    }
                    counter1++;
                    

                    edgepoints+=(newcube.VertexPoint+c.VertexPoint)/2f;
                }
                
                edgepoints/=counter1;
                if (counter1 == 0) continue;
                foreach (var e in c.Edges)
                {
                    var edge = grid.Edges[e];
                    if (!edge.signChange) continue;
                    counter++;
                    facepoints += edge.CatmullFacePoint;
                }
                
                facepoints /= counter;
                //if (counter == 0) continue;

                //Debug.Log(edgepoints);
                
                //if (counter > 0)
                //{
                    //counter = 4;
                if (counter1 == counter)
                {
                    verts[c.CurVertIndex] = ((counter - 3) * c.VertexPoint + facepoints + 2 * edgepoints) / ((float)counter);
                }
                else
                {
                   verts[c.CurVertIndex] = (c.VertexPoint+edgepoints)/2f;
                }

                    
                //}
            }
            //Debug.Log((DateTime.Now - now).TotalSeconds+" for setting all the values");
        }
        public void ResolveCatmullEdges(Edge edge,List<Vector3> verts){
            var a1= MapCubesToEdge(edge.cubes[0],edge.cubes[1]);
            if (ExtraCatmullVerts[a1.Index] == 0)
            {
                ExtraCatmullVerts[a1.Index] = verts.Count;
                verts.Add(a1.CatmullEdgePoint);
            }
            var a2 = MapCubesToEdge(edge.cubes[0], edge.cubes[2]);
            if (ExtraCatmullVerts[a2.Index] == 0)
            {
                ExtraCatmullVerts[a2.Index] = verts.Count;
                verts.Add(a2.CatmullEdgePoint);
            }
            var a3 = MapCubesToEdge(edge.cubes[2], edge.cubes[3]);
            if (ExtraCatmullVerts[a3.Index] == 0)
            {
                ExtraCatmullVerts[a3.Index] = verts.Count;
                verts.Add(a3.CatmullEdgePoint);
            }
            var a4 = MapCubesToEdge(edge.cubes[1], edge.cubes[3]);
            if (ExtraCatmullVerts[a4.Index] == 0)
            {
                ExtraCatmullVerts[a4.Index] = verts.Count;
                verts.Add(a4.CatmullEdgePoint);
            }
        }

        public Edge MapCubesToEdge(Cube a, Cube b)
        {
            if (grid.GetCubeCoords(a.Index).magnitude> grid.GetCubeCoords(b.Index).magnitude)
            {
                var temp = a;
                a = b;
                b = temp;
            }
            var dir = GetEdgeDirection(a, b);
            switch (dir)
            {
                case Edge.EdgeDirection.X:
                    return grid.Edges[a.Edges[2]];
                    
                case Edge.EdgeDirection.Y:
                    return grid.Edges[a.Edges[1]];
                case Edge.EdgeDirection.Z:
                    return grid.Edges[a.Edges[0]];
            }
            throw new Exception();
        }
        public Edge MapCubesToEdge(int indexa, int indexb)
        {
            var a = grid.Cubes[indexa];
            var b = grid.Cubes[indexb];
            var dir = GetEdgeDirection(a, b);
            switch (dir)
            {
                case Edge.EdgeDirection.X:
                    return grid.Edges[a.Edges[2]];

                case Edge.EdgeDirection.Y:
                    return grid.Edges[a.Edges[1]];
                case Edge.EdgeDirection.Z:
                    return grid.Edges[a.Edges[0]];
            }
            throw new Exception();
        }
        public bool IsCatmullEdge(Edge edge)
        {
            var a = grid.GetPointCoords(edge.pointA);
            var b = grid.GetPointCoords(edge.pointB);
            return a.x < grid.xsize - 1 && a.y < grid.ysize - 1 && a.z < grid.zsize - 1 && b.x < grid.xsize - 1 && b.y < grid.ysize - 1 && b.z < grid.zsize - 1;
        }
        public bool IsRenderedEdge(Edge edge)
        {
            return true;
            var a = grid.GetPointCoords(edge.pointA);
            var b = grid.GetPointCoords(edge.pointB);
            return (a.x < grid.xsize - 1 && a.y < grid.ysize - 1 && a.z < grid.zsize - 1 && b.x < grid.xsize - 1 && b.y < grid.ysize - 1 && b.z < grid.zsize - 1) && (a.x > 1 && a.y > 1 && a.z >  1 && b.x >1 && b.y > 1 && b.z > 1);
        }
        public Edge.EdgeDirection GetEdgeDirection(Cube a, Cube b)
        {
            var dif = b.grid.GetCubeCoords(b.Index) - a.grid.GetCubeCoords(a.Index);

            if (Mathf.Abs(dif.x) > 0)
            {
                return Edge.EdgeDirection.X;
            }
            if ((Mathf.Abs(dif.y) > 0))
                return Edge.EdgeDirection.Y;
            if ((Mathf.Abs(dif.z) > 0))
                return Edge.EdgeDirection.Z;
            throw new Exception("Failure");
        }
    }
}
