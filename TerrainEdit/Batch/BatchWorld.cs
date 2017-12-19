using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainEdit.DualContouring;
using System;
namespace TerrainEdit.BatchTools
{
    public class BatchWorld
    {
        public Batch[] Batches;
        public int xsize = 25;
        public int ysize = 20;
        public int zsize = 25;
        public string BatchPath;
        public Vector3 WorldCorner= new Vector3(-2048,-3040,-2048);
        public BatchWorld()
        {
            Batches = new Batch[xsize* ysize*zsize];
        }
        public int GetBatchIndexFromPos(Vector3 pos)
        {
            pos -= WorldCorner;
            pos /= 160f;
            int x = Mathf.FloorToInt(Mathf.Abs(pos.x));
            int y = Mathf.FloorToInt(Mathf.Abs(pos.y));
            int z = Mathf.FloorToInt(Mathf.Abs(pos.z));
            return x*ysize*zsize+y*zsize+z;
        }
        public Vector3 GetBatchCenter(int x, int y, int z)
        {
            return new Vector3(x * 160, y * 160, z * 160) + WorldCorner + new Vector3(80, 80, 80);
        }
        public NodeData GetDataAtPos(Vector3 pos)
        {
            int index = GetBatchIndexFromPos(pos);
            if (Batches[index] == null)
            {
                var pos2 = (pos-WorldCorner) / 160f;
                int x = Mathf.FloorToInt(Mathf.Abs(pos2.x));
                int y = Mathf.FloorToInt(Mathf.Abs(pos2.y));
                int z = Mathf.FloorToInt(Mathf.Abs(pos2.z));
                try
                {
                    UnityEngine.Profiling.Profiler.BeginSample("Deserializing Them Batches");
                    Batches[index] = BatchSerializer.ReadBatch(BatchSerializer.GetBatchPath(BatchPath, x + 1, y + 1, z + 1), GetBatchCenter(x, y, z));
                    UnityEngine.Profiling.Profiler.EndSample();
                }
                catch(Exception e)
                {
                    //Debug
                    //Debug.Log(e);
                    Batches[index] = Batch.CreateEmpty();
                }
            }
            return Batches[index].GetDataAtPosition(pos);
        }
    }
    public class BatchWorldDataProvider : IDataProvider
    {
        BatchWorld batch;
        Vector3 offset;
        public BatchWorldDataProvider(BatchWorld batch, Vector3 offset)
        {
            this.batch = batch;
            this.offset = offset;
        }
        public float GetDistanceValue(Vector3 val)
        {
            var data = batch.GetDataAtPos(val + offset - new Vector3(2, 2, 2));
            var h = GetRealDistance(data.Material, (data.Distance - 126) / 126f);

            return h;
        }
        float GetRealDistance(byte Material, float dist)
        {
            return Material == 0 ? -1 : (dist == -1 ? 1 : dist);
        }
        public byte GetMaterialValue(Vector3 val)
        {
            return batch.GetDataAtPos(val + offset - new Vector3(2, 2, 2)).Material;
        }
    }
}