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
            BinaryReader read = new BinaryReader(new FileStream(path, FileMode.Open));

            return Batch.Deserialize(read,Position);
        }
    }
}
