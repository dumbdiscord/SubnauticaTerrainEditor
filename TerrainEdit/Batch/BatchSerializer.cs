using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
namespace TerrainEdit.BatchTools
{
    public static class BatchSerializer
    {

        public static Batch ReadBatch(string path,Vector3 Position)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Deserializing Batch");
            using (var fs = new FileStream(path, FileMode.Open)){
                using (BinaryReader read = new BinaryReader(fs))
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    
                    var h = Batch.Deserialize(read, Position);
                    read.Close();
                    return h;
                    
                }
            }
            
        }
        public static string GetBatchPath(string basepath, int x, int y, int z)
        {
            return Path.Combine(basepath, String.Format("compiled-batch-{0}-{1}-{2}.optoctrees",x,y,z));
        }
    }
}
