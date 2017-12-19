using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Profiling;
namespace TerrainEdit.DualContouring
{
    public class GridMeshBuilder
    {
        
        //CubeGrid cube;
        CubeGridData cubedata;
        List<Vector3> verts;
        List<int> tris;
        CatmullManager catmull;

        public GridMeshBuilder(int xsize, int ysize, int zsize, IDataProvider provider)
        {
            cubedata = new CubeGridData( new CubeGrid(xsize, ysize, zsize));
            cubedata.PopulateGrid(provider);
            verts = new List<Vector3>();
            tris = new List<int>();
        }
        public GridMeshBuilder(CubeGrid cube, IDataProvider provider)
        {
            var now = DateTime.Now;
            verts = new List<Vector3>();
            tris = new List<int>();
            this.cubedata = new CubeGridData(cube);
            cubedata.PopulateGrid(provider);
            //Debug.Log((DateTime.Now-now).TotalSeconds + " to initialize gridmeshbuilder");
        }
        public GridMeshBuilder(CubeGridData cube, IDataProvider data)
        {
            Profiler.BeginSample("Populating Grid");
            verts = new List<Vector3>();
            tris = new List<int>();
            
            this.cubedata = cube;
            cubedata.PopulateGrid(data);
            Profiler.EndSample();
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
            if (a1.CurVertIndex(cubedata)-1 == -1)
            {
                cubedata.CubeVertIndexes[a1.Index] = ++count;
                verts.Add(a1.VertexPoint(cubedata));// - new Vector3(1, 1, 1) / 2f);
            }

            if (a2.CurVertIndex(cubedata)-1 == -1)
            {
                cubedata.CubeVertIndexes[a2.Index] = ++count;
                verts.Add(a2.VertexPoint(cubedata));// - new Vector3(1, 1, 1) / 2f);
            }

            if (b1.CurVertIndex(cubedata) - 1 == -1)
            {
                cubedata.CubeVertIndexes[b1.Index] = ++count;
                verts.Add(b1.VertexPoint(cubedata));// - new Vector3(1, 1, 1) / 2f);
            }

            if (b2.CurVertIndex(cubedata) - 1 == -1)
            {
                cubedata.CubeVertIndexes[b2.Index] = ++count;
                verts.Add(b2.VertexPoint(cubedata));// - new Vector3(1, 1, 1) / 2f);

            }

            tris.Add(a1.CurVertIndex(cubedata)-1);
            tris.Add(a2.CurVertIndex(cubedata)-1);
            tris.Add(b1.CurVertIndex(cubedata)-1);
            tris.Add(a2.CurVertIndex(cubedata)-1);
            tris.Add(b2.CurVertIndex(cubedata)-1);
            tris.Add(b1.CurVertIndex(cubedata)-1);

        }
        void AssignVerts(Cube c1, Cube c2, Cube c3, Cube c4)
        {
            var count = verts.Count;
            var tricount = tris.Count;
            var a1 = cubedata.CubeData[c1.Index];
            var a2 = cubedata.CubeData[c1.Index];
            var b1 = cubedata.CubeData[c1.Index];
            var b2 = cubedata.CubeData[c1.Index];

            if (cubedata.CubeVertIndexes[c1.Index]-1 == -1)
            {
                cubedata.CubeVertIndexes[c1.Index]  = ++count;
                verts.Add(a1.VertexPoint);// - new Vector3(1, 1, 1) / 2f);
            }

            if (cubedata.CubeVertIndexes[c2.Index] - 1 == -1)
            {
                cubedata.CubeVertIndexes[c2.Index] = ++count;
                verts.Add(a2.VertexPoint);// - new Vector3(1, 1, 1) / 2f);
            }

            if (cubedata.CubeVertIndexes[c3.Index] - 1 == -1)
            {
                cubedata.CubeVertIndexes[c3.Index] = ++count;
                verts.Add(b1.VertexPoint);// - new Vector3(1, 1, 1) / 2f);
            }

            if (cubedata.CubeVertIndexes[c4.Index] - 1 == -1)
            {
                cubedata.CubeVertIndexes[c4.Index] = ++count;
                verts.Add(b2.VertexPoint);// - new Vector3(1, 1, 1) / 2f);

            }



        }
        public void ComputeMesh()
        {
            verts.Clear();
            tris.Clear();
            var cube = cubedata.CubeGrid;
            Profiler.BeginSample("Dual Contour Quad Calculation");
            foreach (var edges in cube.Edges)
            {
                var edge = cubedata.EdgeData[edges.Index];
                var vec = cube.GetPointCoords(edges.pointA);
                if (vec.x > 0 && vec.y > 0 && vec.z > 0)// && vec.x < cube.xsize && vec.y < cube.ysize && vec.z < cube.zsize)
                    if (edge.signChange)
                    {
                        if (edges.cubes.Count != 4) continue;


                        if (edge.reverse)
                        {
                            AddMesh(cube.Cubes[edges.cubes[0]], cube.Cubes[edges.cubes[1]], cube.Cubes[edges.cubes[2]], cube.Cubes[edges.cubes[3]]);
                            //AddMesh(cube.Cubes[edge.cubes[0]].VertexPoint, cube.Cubes[edge.cubes[1]].VertexPoint, cube.Cubes[edge.cubes[2]].VertexPoint, cube.Cubes[edge.cubes[3]].VertexPoint);
                        }
                        else
                        {
                            AddMesh(cube.Cubes[edges.cubes[2]], cube.Cubes[edges.cubes[3]], cube.Cubes[edges.cubes[0]], cube.Cubes[edges.cubes[1]]);
                            //AddMesh(cube.Cubes[edge.cubes[2]].VertexPoint, cube.Cubes[edge.cubes[3]].VertexPoint, cube.Cubes[edge.cubes[0]].VertexPoint, cube.Cubes[edge.cubes[1]].VertexPoint);
                        }
                    }


            }

            Profiler.EndSample();
        }
        public void CatmullClark()
        {
            var now = DateTime.Now;
            catmull = new CatmullManager(cubedata);


            verts.Clear();
            tris.Clear();
            var cube = cubedata.CubeGrid;
            Profiler.BeginSample("Catmull Stage A");
            foreach (var edges in cubedata.CubeGrid.Edges)
            {
                var vec = cube.GetPointCoords(edges.pointA);
                var edge = cubedata.EdgeData[edges.Index];
                // && vec.x < cube.xsize-1 && vec.y < cube.ysize-1 && vec.z < cube.zsize-1
                if ((vec.x > 1 && vec.y > 1 && vec.z > 1) && (vec.x < cube.xsize - 2 && vec.y < cube.ysize - 2 && vec.z < cube.zsize - 2))//if (vec.x > 0 && vec.y > 0 && vec.z > 0)// && vec.x < cube.xsize && vec.y < cube.ysize && vec.z < cube.zsize)
                    if (edge.signChange)
                    {
                        if (edges.cubes.Count != 4) continue;


                        if (edge.reverse)
                        {
                            AssignVerts(cube.Cubes[edges.cubes[0]], cube.Cubes[edges.cubes[1]], cube.Cubes[edges.cubes[2]], cube.Cubes[edges.cubes[3]]);
                            //AddMesh(cube.Cubes[edge.cubes[0]].VertexPoint, cube.Cubes[edge.cubes[1]].VertexPoint, cube.Cubes[edge.cubes[2]].VertexPoint, cube.Cubes[edge.cubes[3]].VertexPoint);
                        }
                        else
                        {
                            AssignVerts(cube.Cubes[edges.cubes[2]], cube.Cubes[edges.cubes[3]], cube.Cubes[edges.cubes[0]], cube.Cubes[edges.cubes[1]]);
                            //AddMesh(cube.Cubes[edge.cubes[2]].VertexPoint, cube.Cubes[edge.cubes[3]].VertexPoint, cube.Cubes[edge.cubes[0]].VertexPoint, cube.Cubes[edge.cubes[1]].VertexPoint);
                        }
                    }


            }
            Profiler.EndSample();
            //Debug.Log((DateTime.Now - now).TotalSeconds+" for making fake verts");
            catmull.Init();
            catmull.CalculateExtraVerts(verts);
            Profiler.BeginSample("Catmull Clark B");
            now = DateTime.Now;
            foreach (var edges in cubedata.CubeGrid.Edges)
            {
               
                var edge = cubedata.EdgeData[edges.Index];
                if (edge.signChange)
                {
                    var vec = cubedata.CubeGrid.GetPointCoords(edges.pointA);
                    if ((vec.x > 1 && vec.y > 1 && vec.z >1) && (vec.x < cube.xsize - 2 && vec.y < cube.ysize - 2 && vec.z < cube.zsize - 2))
                    {
                        if (edges.cubes.Count < 4) continue;
                        AddCatmullMesh(edges);
                    }
                }
            }
            Profiler.EndSample();
            //Debug.Log((DateTime.Now - now).TotalSeconds+" for Catmulling");
        }
        void AddCatmullMesh(Edge centeredge)
        {
            var count = verts.Count;
            var tricount = tris.Count;
            
            var CatmullVert = catmull.CatmullFacePoints[centeredge.Index];
            var catmullvertindex = verts.Count; 
            verts.Add(CatmullVert);
            var cube = cubedata;
            catmull.ResolveCatmullEdges(centeredge,verts);
            
            if (cubedata.EdgeData[centeredge.Index].reverse)
            {
                
                AddQuad(cube.CubeVertIndexes[centeredge.cubes[0]]-1, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[1]).Index], catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[2]).Index], catmullvertindex);
                AddQuad(catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[1]).Index], cube.CubeVertIndexes[centeredge.cubes[1]]-1, catmullvertindex, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[1], centeredge.cubes[3]).Index]);
                AddQuad(catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[2]).Index], catmullvertindex, cube.CubeVertIndexes[centeredge.cubes[2]]-1, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[2], centeredge.cubes[3]).Index]);
                AddQuad(catmullvertindex, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[1], centeredge.cubes[3]).Index], catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[2], centeredge.cubes[3]).Index], cube.CubeVertIndexes[centeredge.cubes[3]]-1);
            }
            else
            {
                //Debug.Log(verts[cube.Cubes[centeredge.cubes[0]].CurVertIndex] + " " + verts[catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[1]).Index]] + " " + verts[catmullvertindex] + " " +verts[ catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[2]).Index]]);
                AddQuad(cube.CubeVertIndexes[centeredge.cubes[0]] - 1, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[1]).Index], catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[2]).Index], catmullvertindex,true);
                AddQuad(catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[1]).Index], cube.CubeVertIndexes[centeredge.cubes[1]] - 1, catmullvertindex, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[1], centeredge.cubes[3]).Index],true);
                AddQuad(catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[0], centeredge.cubes[2]).Index], catmullvertindex, cube.CubeVertIndexes[centeredge.cubes[2]] - 1, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[2], centeredge.cubes[3]).Index],true);
                AddQuad(catmullvertindex, catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[1], centeredge.cubes[3]).Index], catmull.ExtraCatmullVerts[catmull.MapCubesToEdge(centeredge.cubes[2], centeredge.cubes[3]).Index], cube.CubeVertIndexes[centeredge.cubes[3]] - 1,true);
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
            //cubedata = null;

        }
    }
    public class CatmullManager
    {
        public int[] ExtraCatmullVerts;
        public Vector3[] CatmullEdgePoints;
        public Vector3[] CatmullFacePoints;
        public bool[] CatmullEdgeEdges;
        public byte[] CatmullEdgeFaceCount;
        public CubeGridData griddata;
        public CatmullManager(CubeGridData grid)
        {
            this.griddata = grid;
        }
        public void Init()
        {
            ExtraCatmullVerts = new int[griddata.CubeGrid.Edges.Length];
            CatmullEdgePoints= new Vector3[griddata.CubeGrid.Edges.Length];
            CatmullFacePoints = new Vector3[griddata.CubeGrid.Edges.Length];
            CatmullEdgeEdges = new bool[griddata.CubeGrid.Edges.Length];
            CatmullEdgeFaceCount = new byte[griddata.CubeGrid.Edges.Length];
        }

        public void CalculateExtraVerts(List<Vector3> verts)
        {
            var now = DateTime.Now;
            Profiler.BeginSample("FacePoints");
            foreach (var e in griddata.CubeGrid.Edges)
            {
                var edge = griddata.EdgeData[e.Index];
                
                if (edge.signChange)
                {
                    
                    var h = Vector3.zero;
                    int counter = 0;
                    for(int r = 0;r<e.cubes.Count;r++)
                    {
                        var i = e.cubes[r];
                        
                        h += griddata.CubeData[i].VertexPoint;
                        counter++;
                    }
                    if (counter == 0) continue;
                    h /= counter;
                    CatmullFacePoints[e.Index] = h;
                    if (e.cubes.Count != 4) continue;
                    for (int r = 0; r < 4; r++)
                    {
                        var i = e.cubes[r];
                        switch(r){
                            
                            case 0:
                                var newedge = MapCubesToEdge(i,e.cubes[1]);
                                CatmullEdgeEdges[newedge.Index]=true;
                                CatmullEdgePoints[newedge.Index]+=h;
                                CatmullEdgeFaceCount[newedge.Index]++;
                                break;
                            case 1:
                                newedge = MapCubesToEdge(i,e.cubes[3]);
                                CatmullEdgeEdges[newedge.Index]=true;
                                CatmullEdgePoints[newedge.Index]+=h;
                                CatmullEdgeFaceCount[newedge.Index]++;
                                break;
                            case 2:
                                newedge = MapCubesToEdge(e.cubes[0],i);
                                CatmullEdgeEdges[newedge.Index]=true;
                                CatmullEdgePoints[newedge.Index]+=h;
                                CatmullEdgeFaceCount[newedge.Index]++;
                                break;
                            case 3:
                                newedge = MapCubesToEdge(e.cubes[2],i);
                                CatmullEdgeEdges[newedge.Index]=true;
                                CatmullEdgePoints[newedge.Index]+=h;
                                CatmullEdgeFaceCount[newedge.Index]++;
                                break;
                        }
                    }
                }
                
            }
            Profiler.EndSample();
            Profiler.BeginSample("Edge points");
            
            foreach (var edge in griddata.CubeGrid.Edges)
            {
                if (!CatmullEdgeEdges[edge.Index]) continue;
                if (IsCatmullEdge(edge))
                    {
                        /*bool shouldcont = false;
                        foreach (var c in edge.cubes)
                        {
                            if (griddata.CubeData[c].signChange)
                            {
                                shouldcont = true; break;
                            }
                        }
                        if (!shouldcont) continue;*/
                        var q = Vector3.zero;
                        
                        var pointcoords = griddata.CubeGrid.GetPointCoords(edge.pointB);
                        var cubeb=griddata.CubeGrid.Cubes[griddata.CubeGrid.GetCubeIndex((int)pointcoords.x, (int)pointcoords.y, (int)pointcoords.z)];
                        
                        pointcoords = griddata.CubeGrid.GetPointCoords(edge.pointA);
                        var cubea= griddata.CubeGrid.Cubes[griddata.CubeGrid.GetCubeIndex((int)pointcoords.x, (int)pointcoords.y, (int)pointcoords.z)];
                        var cubeadata = griddata.CubeData[cubea.Index];
                        var cubebdata = griddata.CubeData[cubeb.Index];
                        //r++;
                        //if (!cubeadata.signChange || !cubebdata.signChange) continue;
                        var num = CatmullEdgeFaceCount[edge.Index];
                        q += cubeadata.VertexPoint;
                        q += cubebdata.VertexPoint;
                        
                        int i = 2;
                        if(num%2==0){
                            i += num;
                            q += CatmullEdgePoints[edge.Index];
                        }
                        CatmullEdgePoints[edge.Index] = q/i;
                    }
                
            }
            Profiler.EndSample();
            
            Profiler.BeginSample("Move original verts");
            foreach (Cube cub in griddata.CubeGrid.Cubes)
            {
                var c = griddata.CubeData[cub.Index];
                if (!c.signChange)
                    continue;
                if (griddata.CubeVertIndexes[cub.Index] -1 == -1) continue;
                var v = griddata.CubeGrid.GetCubeCoords(cub.Index)-new Vector3(1,1,1)/2f;
                int counter = 0;
                int counter1 = 0;
                Vector3 facepoints= Vector3.zero;
                Vector3 edgepoints= Vector3.zero;


                for (int i = 0; i < 6; i++)
                {

                    if(v.x<1||v.x>=griddata.CubeGrid.xsize-2||v.y<1||v.y>=griddata.CubeGrid.ysize-2||v.z<1||v.z>=griddata.CubeGrid.zsize-2)
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
                    var newcube = griddata.CubeData[griddata.CubeGrid.GetCubeIndex((int)h.x, (int)h.y, (int)h.z)];
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
                foreach (var e in cub.Edges)
                {
                    var edge = griddata.EdgeData[e];
                    if (!edge.signChange) continue;
                    counter++;
                    facepoints += CatmullFacePoints[e];
                }
                
                facepoints /= counter;
                //if (counter == 0) continue;

                //Debug.Log(edgepoints);
                
                //if (counter > 0)
                //{
                    //counter = 4;
                if (counter1 == counter)
                {
                    verts[griddata.CubeVertIndexes[cub.Index] -1] = ((counter - 3) * c.VertexPoint + facepoints + 2 * edgepoints) / ((float)counter);
                }
                else
                {
                   verts[griddata.CubeVertIndexes[cub.Index] -1] = (c.VertexPoint+edgepoints)/2f;
                }

                    
                //}
            }
            Profiler.EndSample();
            //Debug.Log((DateTime.Now - now).TotalSeconds+" for setting all the values");
        }
        public void ResolveCatmullEdges(Edge edge,List<Vector3> verts){
            var a1=MapCubesToEdge(edge.cubes[0],edge.cubes[1]);
            if (ExtraCatmullVerts[a1.Index] == 0)
            {
                ExtraCatmullVerts[a1.Index] = verts.Count;
                verts.Add(CatmullEdgePoints[a1.Index]);
            }
            var a2 = MapCubesToEdge(edge.cubes[0], edge.cubes[2]);
            if (ExtraCatmullVerts[a2.Index] == 0)
            {
                ExtraCatmullVerts[a2.Index] = verts.Count;
                verts.Add(CatmullEdgePoints[a2.Index]);
            }
            var a3 = MapCubesToEdge(edge.cubes[2], edge.cubes[3]);
            if (ExtraCatmullVerts[a3.Index] == 0)
            {
                ExtraCatmullVerts[a3.Index] = verts.Count;
                verts.Add(CatmullEdgePoints[a3.Index]);
            }
            var a4 = MapCubesToEdge(edge.cubes[1], edge.cubes[3]);
            if (ExtraCatmullVerts[a4.Index] == 0)
            {
                ExtraCatmullVerts[a4.Index] = verts.Count;
                verts.Add(CatmullEdgePoints[a4.Index]);
            }
        }

        public Edge MapCubesToEdge(Cube a, Cube b)
        {
            if (griddata.CubeGrid.GetCubeCoords(a.Index).magnitude> griddata.CubeGrid.GetCubeCoords(b.Index).magnitude)
            {
                var temp = a;
                a = b;
                b = temp;
            }
            var dir = GetEdgeDirection(a, b);
            switch (dir)
            {
                case Edge.EdgeDirection.X:
                    return griddata.CubeGrid.Edges[a.Edges[2]];
                    
                case Edge.EdgeDirection.Y:
                    return griddata.CubeGrid.Edges[a.Edges[1]];
                case Edge.EdgeDirection.Z:
                    return griddata.CubeGrid.Edges[a.Edges[0]];
            }
            throw new Exception();
        }
        public Edge MapCubesToEdge(int indexa, int indexb)
        {
            var grid = griddata.CubeGrid;
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
            var grid = griddata.CubeGrid;
            var a = grid.GetPointCoords(edge.pointA);
            var b = grid.GetPointCoords(edge.pointB);
            return a.x < grid.xsize - 1 && a.y < grid.ysize - 1 && a.z < grid.zsize - 1 && b.x < grid.xsize - 1 && b.y < grid.ysize - 1 && b.z < grid.zsize - 1;
        }
        public Edge.EdgeDirection GetEdgeDirection(Cube a, Cube b)
        {
            var grid = griddata.CubeGrid;
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
