using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TerrainEdit.DualContouring;
namespace TerrainEdit.BatchTools{
    public class Batch {
        public int Version;
        public Octree[] Octrees;
        public Vector3 Position;
        public static Batch Deserialize(BinaryReader reader,Vector3 Position)
        {
            var now = DateTime.Now;
            var batch = new Batch();
            batch.Position = Position;
            batch.Version = reader.ReadInt32();
            batch.Octrees= new Octree[125];
            Vector3 Offset = new Vector3(1, 1, 1) * -64f;
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    for (int z = 0; z < 5; z++)
                    {
                        var octree = new RootNode(new Vector3(x * 32,  y * 32, z * 32) + Offset, 32);
                        int count = reader.ReadUInt16();
                        ProtoOctreeNode[] nodes = new ProtoOctreeNode[count];
                        for (int i = 0; i < count; i++)
                        {
                            nodes[i] = ProtoOctreeNode.Deserialize(reader);
                        }
                        octree.InitFromProtoData(ref nodes);
                         
                        batch.Octrees[x*25  + y * 5 + z] = octree;
                    }
                }
            }
            //Debug.Log((DateTime.Now - now).TotalSeconds+" to deserialize batch");
            return batch;
        }
        public BatchDataProvider GetDataProvider(Vector3 offset)
        {
            return new BatchDataProvider(this,offset);
        }

        public NodeData GetDataAtPosition(Vector3 val)
        {
                float mysize = 160;
                val -= Position;
                if (!((val.x >= -mysize / 2f && val.x <= mysize / 2f) && (val.y >= -mysize / 2f && val.y <= mysize / 2f) && (val.z >= -mysize / 2f && val.z <= mysize / 2f))) { return new NodeData(0, 0); }
            // - (Position)
                Vector3 Correctedval = ((val+ new Vector3(80,80,80))) / 32f;
                Correctedval=new Vector3(Math.Abs(Correctedval.x),Math.Abs(Correctedval.y),Math.Abs(Correctedval.z));
                
                try{
                    var tree = Octrees[Mathf.FloorToInt((Correctedval.x) >= 5 ? 4 : Correctedval.x) * 25 + Mathf.FloorToInt(Correctedval.y >= 5 ? 4 : Correctedval.y) * 5 + Mathf.FloorToInt(Correctedval.z >= 5 ? 4 : Correctedval.z)];
                    //Debug.Log(val - tree.Position+" "+val);
                    var h = val - (tree.Position);
                    return tree.GetLeafNodeDataAt(new Vector3(h.z, h.y, h.x));
                }
                catch
                {
                    throw new Exception(" " + Correctedval);
                }
                
            
        }
        public Octree GetOctreeAt(Vector3 val)
        {
            float mysize = 160;
            val -= Position;
            if (!((val.x >= -mysize / 2f && val.x <= mysize / 2f) && (val.y >= -mysize / 2f && val.y <= mysize / 2f) && (val.z >= -mysize / 2f && val.z <= mysize / 2f))) { return null; }
            // - (Position)
            Vector3 Correctedval = ((val + new Vector3(80, 80, 80))) / 32f;
            Correctedval = new Vector3(Math.Abs(Correctedval.x), Math.Abs(Correctedval.y), Math.Abs(Correctedval.z));


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
