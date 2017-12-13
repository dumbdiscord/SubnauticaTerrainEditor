using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Profiling;
namespace TerrainEdit.DualContouring
{
    public class CubeGridData
    {
        public CubeGrid CubeGrid;
        public EdgeData[] EdgeData;
        public CubeData[] CubeData;
        public int[] CubeVertIndexes;
        public CubeGridData(CubeGrid cubegrid)
        {
            this.CubeGrid = cubegrid;
            EdgeData = new EdgeData[CubeGrid.Edges.Length];
            CubeData = new CubeData[CubeGrid.Cubes.Length];
        }
        public void Reset()
        {
            CubeVertIndexes = new int[CubeGrid.Cubes.Length];
        }
        public void PopulateGrid(IDataProvider data)
        {
            Profiler.BeginSample("Grid Population");
            Reset();
            
            
            foreach (Edge edge in CubeGrid.Edges)
            {
                edge.CalculateInterpolationPoint(data, ref EdgeData[edge.Index]);
            }
            foreach (Cube cube in CubeGrid.Cubes)
            {
                cube.CalculateVertexPoint(ref EdgeData,ref CubeData[cube.Index]);
            }
            Profiler.EndSample();
        }
        
    }
}
