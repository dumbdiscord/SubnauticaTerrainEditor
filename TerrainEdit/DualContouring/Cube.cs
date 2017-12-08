using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainEdit.DualContouring
{
    public class CubeGrid
    {
        public Cube[] Cubes;
        public Edge[] Edges;
        public Dictionary<Vector3, int> edgeindex;
        public ValuePoint[] Points;
        public int xsize;
        public int ysize;
        public int zsize;
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
        public static readonly Vector3[] EdgeOffsets = new Vector3[12]{
            (Vector3.forward-Vector3.up)/2f,
            (-Vector3.forward-Vector3.up)/2f,
            (Vector3.right-Vector3.up)/2f,
            (-Vector3.right-Vector3.up)/2f,
            (Vector3.forward+Vector3.up)/2f,
            (-Vector3.forward+Vector3.up)/2f,
            (Vector3.right+Vector3.up)/2f,
            (-Vector3.right+Vector3.up)/2f,
            (Vector3.forward+Vector3.right)/2f,
            (-Vector3.forward+Vector3.right)/2f,
            (Vector3.forward-Vector3.right)/2f,
            (-Vector3.forward-Vector3.right)/2f,
        };
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
        public CubeGrid(int xsize, int ysize, int zsize,IDataProvider provider)
        {
            edgeindex = new Dictionary<Vector3,int>();
            Points = new ValuePoint[xsize * ysize * zsize];
            Edges = new Edge[((zsize - 1) * ysize + (ysize - 1) * zsize) * xsize + (xsize - 1) * zsize * ysize];
            Cubes = new Cube[(xsize - 1) * (zsize - 1) * (ysize - 1)];
            this.xsize = xsize;
            this.ysize = ysize;
            this.zsize = zsize;
            int edgecounter = 0;
            for(int x = 0;x<xsize;x++)
            {
                for(int y = 0;y<ysize;y++)
                {
                    for(int z = 0;z<zsize;z++)
                    {
                        var h = new Vector3(x,y,z);
                        int ind = z + y * zsize + x * (zsize * ysize);
                        Points[ind] = new ValuePoint(this,ind, provider.GetDistanceValue(h));                        
                    }
                }
            }
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
                            Edges[edgecounter++] = new Edge(this, curpos, curpos + zoffset).AddToNeighboringCubes(x,y,z,edgecounter-1,Edge.EdgeDirection.Z).CalculateInterpolationPoint();
                        }

                        if (y != ysize - 1)
                        {
                            //edgeindex.Add((Points[curpos].Point + Points[curpos + yoffset].Point) / 2, edgecounter);
                            //Debug.Log((Points[curpos].Point + Points[curpos + yoffset].Point) / 2);
                            Edges[edgecounter++] = new Edge(this, curpos, curpos + yoffset).AddToNeighboringCubes(x, y, z, edgecounter - 1, Edge.EdgeDirection.Y).CalculateInterpolationPoint();
                        }

                        if (x != xsize - 1)
                        {
                            //edgeindex.Add((Points[curpos].Point + Points[curpos + xoffset].Point) / 2, edgecounter);
                            //Debug.Log((Points[curpos].Point + Points[curpos + xoffset].Point) / 2);
                            Edges[edgecounter++] = new Edge(this, curpos, curpos + xoffset).AddToNeighboringCubes(x, y, z, edgecounter - 1, Edge.EdgeDirection.X).CalculateInterpolationPoint(); 
                        }
                    }
                }
            }
            //Debug.Log(h);
            foreach (Cube c in Cubes)
            {
                c.CalculateVertexPoint();
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
        public Vector3 VertexPoint;
        public bool signChange=false;
        public int Index;
        public int CurVertIndex = -1;
        public Cube(CubeGrid grid, int index)
        {
            this.grid = grid;
            Index = index;
        }
        public void CalculateVertexPoint()
        {
            int i =0;
            
            int myid = Index;
            for (int h = 0; h < curedgeindex;h++ )
            {
                int edge = Edges[h];
                grid.Edges[edge].cubes.Add(myid);
                if (grid.Edges[edge].signChange)
                {
                    signChange = true;

                }
                i++;

                VertexPoint += grid.Edges[edge].interpolationPoint;

            }
            VertexPoint /= i;

        }

    }
    public class Edge
    {
        public CubeGrid cube;
        public int pointA;
        public int pointB;
        public Vector3 interpolationPoint;
        public bool signChange = false;
        public List<int> cubes = new List<int>();
        public bool reverse;
        public Edge(CubeGrid cube, int pointa, int pointb)
        {
            this.cube = cube;
            pointA = pointa;
            pointB = pointb;
        }
        public Edge AddToNeighboringCubes(int x, int y, int z,int edgeindex,EdgeDirection dir)
        {
            switch (dir)
            {
                case EdgeDirection.X:
                    
                    Vector3 curpos = cube.GetPointCoords(pointA);
                    if(y!=cube.ysize-1&&z!=cube.zsize-1)
                    cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y, (int)curpos.z)].Edges[cube.Cubes[cube.GetCubeIndex((int)curpos.x, (int)curpos.y, (int)curpos.z)].curedgeindex++] = edgeindex;
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
        public Edge CalculateInterpolationPoint()
        {
            
            float t = 0f;
            float b = cube.Points[pointB].Value;
            float a = cube.Points[pointA].Value;
            var pointa = cube.Points[pointA].Point;
            var pointb = cube.Points[pointB].Point;
            if (Mathf.Sign(a) != Mathf.Sign(b))
            {
                signChange = true;
                t = .5f;// -a / (b - a);
                
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

            interpolationPoint = pointa * (1 - t) + (pointb * t);
            return this;
        }
        public enum EdgeDirection
        {
            X,
            Y,
            Z
        }
    }
    public class ValuePoint
    {
        public CubeGrid grid;
        public int Index;
        public float Value;
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
    }
}
