using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TerrainEdit.DualContouring;
namespace TerrainEdit.BatchTools{
    public class Batch {
        public int Version;
        public IOctree[] Octrees;
        public Vector3 Position;
        public bool amempty = true;
        public string name;
        public bool IsLoaded;
        public bool IsLoading;
        public BatchWorld world;
        public Int3 intPos;
        public Batch(BatchWorld world, Vector3 pos)
        {
            this.world = world;
            this.Position = pos;
            this.intPos = world.GetBatchPos(pos);
        }
        public static Batch Deserialize(BinaryReader reader,Vector3 Position, BatchWorld world)
        {
            var now = DateTime.Now;
            var batch = new Batch(world,Position);
            //Debug.Log("Loading "+world.GetBatchPos(Position));
            batch.Version = reader.ReadInt32();
            batch.Octrees= new IOctree[125];
            Debug.Log(batch.Octrees==null);
            Vector3 Offset = new Vector3(1, 1, 1) * -64f;
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    for (int z = 0; z < 5; z++)
                    {
                        var octree = new CompactOctree();//new RootNode(new Vector3(x * 32,  y * 32, z * 32) + Offset, 32);
                        int count = reader.ReadUInt16();
                        octree.Nodes = new ProtoOctreeNode[count];
                        for (int i = 0; i < count; i++)
                        {
                            octree.Nodes[i] = ProtoOctreeNode.Deserialize(reader);
                        }
                        
                        octree.Position = new Vector3(x * 32, y * 32, z * 32) + Offset;
                        octree.SetIfEmpty();
                        if (!octree.IsEmpty) batch.amempty = false;
                        batch.Octrees[x*25  + y * 5 + z] = octree as IOctree;
                    }
                }
            }
            
            if (batch.amempty) batch.Octrees = null;
            //batch.amempty = true;
            batch.IsLoaded = true;
            //Debug.Log((DateTime.Now - now).TotalSeconds+" to deserialize batch");
            return batch;
        }
        public BatchDataProvider GetDataProvider(Vector3 offset)
        {
            return new BatchDataProvider(this,offset);
        }
        public static Batch CreateEmpty(BatchWorld world, Vector3 position)
        {
            return new Batch(world,position) {amempty=true };
        }

        public NodeData GetDataAtPosition(Vector3 val)
        {
                if (amempty) return new NodeData(0, 0);   
                float mysize = 160;
                val -= Position;
                if (!((val.x >= -mysize / 2f && val.x <= mysize / 2f) && (val.y >= -mysize / 2f && val.y <= mysize / 2f) && (val.z >= -mysize / 2f && val.z <= mysize / 2f))) { return new NodeData(0, 0); }
                
                Vector3 Correctedval = ((val+ new Vector3(80,80,80))) / 32f;
                Correctedval=new Vector3(Math.Abs(Correctedval.x),Math.Abs(Correctedval.y),Math.Abs(Correctedval.z));
                
                try{
                    var tree = Octrees[Mathf.FloorToInt((Correctedval.x) >= 5 ? 4 : Correctedval.x) * 25 + Mathf.FloorToInt(Correctedval.y >= 5 ? 4 : Correctedval.y) * 5 + Mathf.FloorToInt(Correctedval.z >= 5 ? 4 : Correctedval.z)];
                    //Debug.Log(val - tree.Position+" "+val);
                    var h = val - ((tree).Position);
                    return tree.GetLeafNodeDataAt(new Vector3(h.z, h.y, h.x));
                }
                catch
                {
                    throw new Exception(" " + Correctedval);
                }
                
            
        }
        public IOctree GetOctreeAt(Vector3 val)
        {
            float mysize = 160;
            val -= Position;
            if (!((val.x >= -mysize / 2f && val.x <= mysize / 2f) && (val.y >= -mysize / 2f && val.y <= mysize / 2f) && (val.z >= -mysize / 2f && val.z <= mysize / 2f))) { return null; }
            // - (Position)
            Vector3 Correctedval = ((val + new Vector3(80, 80, 80))) / 32f;
            Correctedval = new Vector3(Math.Abs(Correctedval.x), Math.Abs(Correctedval.y), Math.Abs(Correctedval.z));
            if (amempty) return null;
            //Debug.Log(Mathf.FloorToInt((Correctedval.x) >= 5 ? 4 : Correctedval.x) * 25 + Mathf.FloorToInt(Correctedval.y >= 5 ? 4 : Correctedval.y) * 5 + Mathf.FloorToInt(Correctedval.z >= 5 ? 4 : Correctedval.z));
            //  Debug.Log(name);
            return Octrees[Mathf.FloorToInt((Correctedval.x) >= 5 ? 4 : Correctedval.x) * 25 + Mathf.FloorToInt(Correctedval.y >= 5 ? 4 : Correctedval.y) * 5 + Mathf.FloorToInt(Correctedval.z >= 5 ? 4 : Correctedval.z)];
                //Debug.Log(val - tree.Position+" "+val);
               

        }
        
    }
    public class BatchDataProvider : IDataProvider
    {
        Batch batch;
        Vector3 offset;
        public BatchDataProvider(Batch batch, Vector3 offset)
        {
            this.batch = batch;
            this.offset = offset;
        }
        public float GetDistanceValue(Vector3 val)
        {
            var data = batch.GetDataAtPosition(val+offset-new Vector3(2,2,2));
            var h = GetRealDistance(data.Material,(data.Distance - 126) / 126f);

            return  h;
        }
        float GetRealDistance(byte Material, float dist)
        {
            return Material == 0 ? -1 : (dist == -1 ? 1 : dist);
        }
        public byte GetMaterialValue(Vector3 val)
        {
            return batch.GetDataAtPosition(val + offset - new Vector3(2, 2, 2)).Material;
        }
    }
}
