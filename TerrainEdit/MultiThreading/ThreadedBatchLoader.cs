using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;
//using System.Threading.Tasks;
using TerrainEdit.BatchTools;
namespace TerrainEdit.MultiThreading
{
    public class ThreadedBatchLoader:ThreadedJob
    {
        public Batch batch;
        public string path;
        public Vector3 position;
        
        //public Task task;
        void LoadBatch(string path)
        {
            
            try
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {




                        batch.Version = reader.ReadInt32();
                        batch.Octrees = new IOctree[125];
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
                                    batch.Octrees[x * 25 + y * 5 + z] = octree as IOctree;
                                }
                            }
                        }

                        if (batch.amempty) batch.Octrees = null;

                    }
                }
            }
            catch
            {
                batch.amempty = true;
            }
            batch.IsLoaded = true;
            
        }
        public override void ThreadCode()
        {
            LoadBatch(path);
        }
        public static ThreadedBatchLoader GetLoader(Batch batch)
        {
            return new ThreadedBatchLoader() { batch = batch, path = BatchSerializer.GetBatchPath(batch.world.BatchPath,batch.intPos) };
        }
    }
    
}