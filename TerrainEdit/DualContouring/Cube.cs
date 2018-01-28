using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainEdit.DualContouring
{
    public class CubeGrid
    {
        public const bool MAKE_CUBIC = false;
        public Cube[] Cubes;
        public Edge[] Edges;
        public int xsize;
        public int ysize;
        public int zsize;
        public const float Scale = 1f;
        public int GetCubeIndex(int x, int y, int z)
        {
            if (x >= xsize - 1 || y >= ysize - 1 || z >= zsize - 1)
            {
                throw new Exception("Bad Number "+x+" "+y+" "+z);
            }
            return z + y * (zsize - 1) + x * (ysize - 1) * (zsize - 1);
        }
        public int GetPointIndex(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0)
            {
                throw new Exception("Negative Number Oh No");
            }
            return z + y * zsize + x + zsize * ysize;
        }

        public Vector3 GetCubeCoords(int index)
        {
            Vector3 output = Vector3.zero;
            Vector3 offset = new Vector3(1, 1, 1) / 2f;
            output.z = (index % ((ysize - 1) * (zsize - 1))) % (zsize - 1);
            output.y = ((index - (int)output.z) / (zsize - 1)) % (zsize - 1);
            output.x = ((index - (int)output.z - (zsize - 1) * (int)output.y) / ((ysize - 1) * (zsize - 1)));
            return output+offset;
        }
        public Vector3 GetPointCoords(int index)
        {
            Vector3 output = Vector3.zero;
            output.z = (index % ((ysize) * (zsize ))) % (zsize );
            output.y = ((index - (int)output.z) / (zsize)) % (zsize );
            output.x = ((index - (int)output.z - (zsize ) * (int)output.y) / ((ysize ) * (zsize )));
            return output;
        }
        public CubeGrid(int xsize, int ysize, int zsize)
        {
            Edges = new Edge[((zsize - 1) * ysize + (ysize - 1) * zsize) * xsize + (xsize - 1) * zsize * ysize];
            Cubes = new Cube[(xsize - 1) * (zsize - 1) * (ysize - 1)];
            this.xsize = xsize;
            this.ysize = ysize;
            this.zsize = zsize;
            int edgecounter = 0;
            Vector3 offset = new Vector3(1f, 1f, 1f) / 2;
            for (int x = 0; x < xsize - 1; x++)
            {
                for (int y = 0; y < ysize - 1; y++)
                {
                    for (int z = 0; z < zsize - 1; z++)
                    {
                        var center = offset + new Vector3(x, y, z);
                        Cube cube = new Cube(this, z + y * (zsize - 1) + x * (zsize - 1) * (ysize - 1));
                        cube.Edges = new int[12];
                        //cube.CalculateVertexPoint(z + y * (zsize - 1) + x * (zsize - 1) * (ysize - 1));
                        Cubes[z + y * (zsize - 1) + x * (zsize - 1) * (ysize - 1)] = cube;
                    }
                }
            }
            for (int x = 0; x < xsize; x++)
            {
                for (int y = 0; y < ysize; y++)
                {
                    for (int z = 0; z < zsize; z++)
                    {
                        var curpos = z + y * zsize + x * (zsize * ysize);
                        var zoffset = 1;
                        var yoffset = zsize;
                        var xoffset = zsize * ysize;
                        if (z != zsize - 1)
                        {
                            //edgeindex.Add((Points[curpos].Point + Points[curpos + zoffset].Point) / 2, edgecounter);
                            //Debug.Log((Points[curpos].Point + Points[curpos + zoffset].Point) / 2);
                            Edges[edgecounter++] = new Edge(this, curpos, curpos + zoffset);//.CalculateInterpolationPoint();
                            Edges[edgecounter - 1].AddToNeighboringCubes(x, y, z, edgecounter - 1, Edge.EdgeDirection.Z);
                        }

                        if (y != ysize - 1)
                        {
                            //edgeindex.Add((Points[curpos].Point + Points[curpos + yoffset].Point) / 2, edgecounter);
                            //Debug.Log((Points[curpos].Point + Points[curpos + yoffset].Point) / 2);
                            Edges[edgecounter++] = new Edge(this, curpos, curpos + yoffset);//.CalculateInterpolationPoint();
                            Edges[edgecounter - 1].AddToNeighboringCubes(x, y, z, edgecounter - 1, Edge.EdgeDirection.Y);
                        }

                        if (x != xsize - 1)
                        {
                            //edgeindex.Add((Points[curpos].Point + Points[curpos + xoffset].Point) / 2, edgecounter);
                            //Debug.Log((Points[curpos].Point + Points[curpos + xoffset].Point) / 2);
                            Edges[edgecounter++] = new Edge(this, curpos, curpos + xoffset);//.CalculateInterpolationPoint(); 
                            Edges[edgecounter - 1].AddToNeighboringCubes(x, y, z, edgecounter - 1, Edge.EdgeDirection.X);
                        }
                    }
                }
            }
            // I don't know why I need to do this after the edges are finished, it has something to do with the order they're placed in the cubes edge list and frankly I don't want to mess with it
            foreach (Cube c in Cubes)
            {
                c.AddToEdges();
            }


            
        }

    }
    public class Cube 
    {
        public static readonly Vector3[] cornerlist = new Vector3[]{
            (Vector3.forward + Vector3.up + Vector3.right)/2,
            (Vector3.forward + Vector3.up - Vector3.right)/2,
            (-Vector3.forward + Vector3.up + Vector3.right)/2,
            (-Vector3.forward + Vector3.up - Vector3.right)/2,
            (Vector3.forward - Vector3.up + Vector3.right)/2,
            (Vector3.forward - Vector3.up - Vector3.right)/2,
            (-Vector3.forward - Vector3.up + Vector3.right)/2,
            (-Vector3.forward - Vector3.up - Vector3.right)/2
        };
        public int[] Edges;
        public byte curedgeindex;
        public CubeGrid grid;
        public int Index;
        
        public Cube(CubeGrid grid, int index)
        {
            this.grid = grid;
            Index = index;
        }
        public void CalculateVertexPoint(ref EdgeData[] edgedatas,ref CubeData dat)
        {
            int i =0;
            Vector3 VertexPoint= Vector3.zero;
            bool signChange=false;
            for (int h = 0; h < curedgeindex;h++ )
            {
                int edge = Edges[h];
                if (edgedatas[edge].signChange)
                {
                    signChange = true;
                    if (!CubeGrid.MAKE_CUBIC)
                    {
                        i++;
                        VertexPoint += edgedatas[edge].interpolationPoint;
                    }
                }

                if (CubeGrid.MAKE_CUBIC)
                {
                    i++;
                    VertexPoint += edgedatas[edge].interpolationPoint;
                }


            }
            VertexPoint /= i;
            dat.VertexPoint = VertexPoint;
            dat.signChange = signChange;

        }
        public Vector3 VertexPoint(CubeGridData dat)
        {
            return dat.CubeData[Index].VertexPoint;
        }
        public int CurVertIndex(CubeGridData dat)
        {
            return dat.CubeVertIndexes[Index];
        }
        public void AddToEdges(){
            for(int h = 0;h<curedgeindex;h++){
                grid.Edges[Edges[h]].cubes.Add(Index);
            }
        }
       
    }
    public class Edge
    {
        public CubeGrid cube;
        public int pointA;
        public int Index;
        public int pointB;
        public List<int> cubes = new List<int>();
        public EdgeDirection Direction;

        public Edge(CubeGrid cube, int pointa, int pointb)
        {
            this.cube = cube;
            pointA = pointa;
            pointB = pointb;

        }

        public Edge AddToNeighboringCubes(int x, int y, int z,int edgeindex,EdgeDirection dir)
        {
            Index = edgeindex;
            Direction = dir;
            switch (dir)
            {
                case EdgeDirection.X:
                    
                    Vector3 curpos = cube.GetPointCoords(pointA);
                    if (y != cube.ysize - 1 && z != cube.zsize - 1)
                    {
                        var index = cube.GetCubeIndex((int)curpos.x, (int)curpos.y, (int)curpos.z);
                        cube.Cubes[index].Edges[cube.Cubes[index].curedgeindex++] = edgeindex;

                    }
                    if (curpos.y > 0 && z != cube.zsize - 1)
                    cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y-1, (int)curpos.z)].Edges[cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y-1, (int)curpos.z)].curedgeindex++] = edgeindex;
                    if (curpos.z > 0 && y != cube.ysize - 1)
                    cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y, (int)curpos.z-1)].Edges[cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y, (int)curpos.z-1)].curedgeindex++] = edgeindex;
                    if(curpos.z>0&&curpos.y>0)
                    cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y-1, (int)curpos.z-1)].Edges[cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y-1, (int)curpos.z-1)].curedgeindex++] = edgeindex;
                    break;
                case EdgeDirection.Y:
                    curpos = cube.GetPointCoords(pointA);
                    if(x!=cube.xsize-1&&z!=cube.zsize-1)
                    cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y, (int)curpos.z)].Edges[cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y, (int)curpos.z)].curedgeindex++] = edgeindex;
                    if (curpos.x > 0 && z != cube.zsize - 1)
                    cube.Cubes[cube.GetCubeIndex((int)curpos.x-1, (int)curpos.y, (int)curpos.z)].Edges[cube.Cubes[cube.GetCubeIndex((int)curpos.x-1, (int)curpos.y, (int)curpos.z)].curedgeindex++] = edgeindex;
                    if (curpos.z > 0 && x != cube.xsize - 1)
                    cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y, (int)curpos.z-1)].Edges[cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y, (int)curpos.z-1)].curedgeindex++] = edgeindex;
                    if (curpos.z > 0 && curpos.x > 0)
                    cube.Cubes[cube.GetCubeIndex((int)curpos.x-1, (int)curpos.y , (int)curpos.z - 1)].Edges[cube.Cubes[cube.GetCubeIndex((int)curpos.x - 1, (int)curpos.y, (int)curpos.z - 1)].curedgeindex++] = edgeindex;
                    
                    break;
                case EdgeDirection.Z:
                    
                    curpos = cube.GetPointCoords(pointA);
                    if(y!=cube.ysize-1&&x!=cube.xsize-1)
                    cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y, (int)curpos.z)].Edges[cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y, (int)curpos.z)].curedgeindex++] = edgeindex;
                    if (curpos.y > 0 && x != cube.xsize - 1)
                    cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y-1, (int)curpos.z)].Edges[cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y-1, (int)curpos.z)].curedgeindex++] = edgeindex;
                    if (curpos.x > 0 && y != cube.ysize - 1)
                    cube.Cubes[cube.GetCubeIndex((int)curpos.x-1, (int)curpos.y, (int)curpos.z)].Edges[cube.Cubes[cube.GetCubeIndex((int)curpos.x-1, (int)curpos.y, (int)curpos.z)].curedgeindex++] = edgeindex;
                    if(curpos.x>0&&curpos.y>0)
                    cube.Cubes[cube.GetCubeIndex((int)curpos.x-1, (int)curpos.y-1, (int)curpos.z)].Edges[cube.Cubes[cube.GetCubeIndex((int)curpos.x-1, (int)curpos.y-1, (int)curpos.z)].curedgeindex++] = edgeindex;
                    break;
            }
            return this;
        }
        
        public void CalculateInterpolationPoint(IDataProvider data,ref EdgeData dat)
        {
            
            float t = 0f;
            var pointa = cube.GetPointCoords(pointA) * CubeGrid.Scale;
            var pointb = cube.GetPointCoords(pointB) * CubeGrid.Scale;
            float b = data.GetDistanceValue(pointb);
            float a = data.GetDistanceValue(pointa);

            bool signChange = false;
            bool reverse = false;
            if (Mathf.Sign(a) != Mathf.Sign(b))
            {
                signChange = true;
                t = CubeGrid.MAKE_CUBIC?0.5f:(-a / (b - a));
                
                if (Mathf.Sign(b) > Mathf.Sign(a))
                {
                    reverse = true;

                }
                if (Mathf.Abs(Vector3.Dot((pointa - pointb), Vector3.up)) > .1)
                {
                    reverse = !reverse;
                }
            }
            else
            {
                t = .5f;
            }
            dat.reverse=reverse;
            dat.signChange=signChange;
            dat.interpolationPoint=(pointa * (1 - t) + (pointb * t));
            
        }
        public enum EdgeDirection
        {
            X,
            Y,
            Z
        }
    }
    public struct EdgeData
    {
        public Vector3 interpolationPoint{get; set;}
        public bool reverse { get; set; }
        public bool signChange { get; set; }
        public EdgeData(Vector3 interpoint, bool rev, bool signchange){
            interpolationPoint=interpoint;
            reverse = rev;
            signChange = signchange;
        }
    }
    public struct CubeData
    {
        public Vector3 VertexPoint { get; set; }
        public bool signChange { get; set; }
        public CubeData(Vector3 vertpoint, bool signchange)
        {
            VertexPoint = vertpoint;
            signChange = signchange;
        }
    }
    /*public class ValuePoint
    {
        public CubeGrid grid;
        public int Index;
        public float Value;
        public byte Material;
        public Vector3 Point
        {
            get
            {
                return grid.GetPointCoords(Index);
            }
        }
        public ValuePoint(CubeGrid grid, int index, float value)
        {
            Index = index;
            this.grid=grid;
            Value = value;
        }
        public ValuePoint() { }
    }*/
}
