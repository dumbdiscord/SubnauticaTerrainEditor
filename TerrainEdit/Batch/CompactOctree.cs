using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainEdit.BatchTools
{
    public class CompactOctree:IOctree
    {
        public static readonly Vector3[] ChildCenters = new Vector3[8]{
            new Vector3(-1,-1,-1)/4f,
            new Vector3(1,-1,-1)/4f,
            new Vector3(-1,1,-1)/4f,
            new Vector3(1,1,-1)/4f,
            new Vector3(-1,-1,1)/4f,
            new Vector3(1,-1,1)/4f,
            new Vector3(-1,1,1)/4f,
            new Vector3(1,1,1)/4f
        };
        public ProtoOctreeNode[] Nodes;
        public bool IsEmpty
        {
            get;
            set;
        }
        public CompactOctree()
        {
            IsEmpty = false;
        }
        public NodeData GetLeafNodeDataAt(Vector3 pos)
        {

            if (IsEmpty) return new NodeData(0, 0);
            //Debug.Log("test");
            return GetLeafDataRecurse(0, Size, pos);
        }
        NodeData GetLeafDataRecurse(int nodeindex, float Size, Vector3 val)
        {
            var data = Nodes[nodeindex];

            if (data.ChildIndex == 0) { return data.Data; }
            //if (Data.Distance == 0) return Data;
            byte corneroffset = 0;
            if (val.z > 0)
            {
                corneroffset += 4;
            }
            if (val.y > 0)
            {
                corneroffset += 2;
            }
            if (val.x > 0)
            {
                corneroffset += 1;
            }
            return GetLeafDataRecurse(data.ChildIndex+corneroffset,Size/2f,(val - ChildCenters[corneroffset] * Size));
        }
        public float Size = 32f;
        public Vector3 Position
        {
            get;
            set;
        }
        public CompactOctree(ProtoOctreeNode[] data)
        {
            this.Nodes = data;
        }
        public void SetIfEmpty()
        {
            IsEmpty =true;

            if (Nodes.Length <= 1) return;

            setifemptyrecurse(0);
            

           
        }
        void setifemptyrecurse(int startindex)
        {
            if (!Nodes[startindex].Data.ShouldBeEmpty()) { IsEmpty = false; return; }
            if (Nodes[startindex].ChildIndex == 0) { return; }
            int curindex = Nodes[startindex].ChildIndex;
            for (int i = curindex; i < curindex + 8; i++)
            {
                setifemptyrecurse(i);

                if (!IsEmpty) return;
            }
            
        }
    }
}