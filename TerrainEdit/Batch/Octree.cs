using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
namespace TerrainEdit.BatchTools
{
    public class Octree
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
        private float size;
        Vector3 pos;
        public Octree ParentNode { get; protected set; }
        public NodeData Data { get; protected set; }
        public Octree[] Children{get; protected set;}
        public bool IsLeafNode{
            get{
                return Children==null;
            }
        }
        public virtual bool IsRootNode
        {
            get
            {
                return ParentNode == null;
            }
        }
        public virtual Vector3 Position
        {
            get
            {
                if (pos == Vector3.zero)
                {
                    if (IsRootNode)
                    {
                        return Vector3.up;
                    }
                    pos= ParentNode.GetChildPosition(this);
                }
                return pos;
            }
        }
        public virtual float Size{
            get
            {
                if (size == 0)
                {
                    if (IsRootNode)
                    {
                        return 32f;
                    }
                    size=ParentNode.Size / 2f;
                    
                }
                return size;
            }
        }
        public Vector3 GetChildPosition(Octree tree)
        {
            if (IsLeafNode || tree.ParentNode != this) throw new Exception();
            int childind = Array.IndexOf(Children, tree);
            try
            {
                
                return Position + Size * ChildCenters[childind];
            }
            catch(Exception e)
            {
                Debug.Log(childind);
                throw e;
            }
        }
        public Octree(Octree Parent)
        {
            ParentNode=Parent;
        }
        public Octree() { }
        protected static Octree CreateFromProtoData(ref ProtoOctreeNode[] Nodes, int Index)
        {
            var tree = new Octree();
            if (Nodes[Index].ChildIndex != 0)
            {
                var cindex = Nodes[Index].ChildIndex;
                tree.Children = new Octree[8];

                for (int i = 0; i < 8; i++)
                {
                    var child = Octree.CreateFromProtoData(ref Nodes,cindex+i);
                    child.ParentNode = tree;
                    
                    tree.Children[i] = child;
                }
                
            }
            
            tree.Data = Nodes[Index].Data;
            return tree;
        }

        public NodeData GetLeafNodeDataAt(Vector3 val)
        {
            if(IsLeafNode) return Data;
            if (Data.Distance == 0) return Data;
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
            return Children[corneroffset].GetLeafNodeDataAt(val-ChildCenters[corneroffset]*Size);
        }

        public void SetParent(Octree tree)
        {
            this.ParentNode = tree;
        }
    }
    
    public class RootNode:Octree
    {
        public Vector3 Center;
        public float BaseSize;
        public override Vector3 Position
        {
            get
            {
                return Center;
            }
        }
        
        public override float Size
        {
            get
            {
                return BaseSize;
            }
        }
        public RootNode(Vector3 Position, float Size)
        {
            this.Center = Position;
            this.BaseSize = Size;
        }
        public void InitFromProtoData(ref ProtoOctreeNode[] Nodes)
        {

            Data = Nodes[0].Data;
            if(Nodes.Length==1) return;
            var cindex = Nodes[0].ChildIndex;
            Children = new Octree[8];

            for (int i = 0; i < 8; i++)
            {
                var child = Octree.CreateFromProtoData(ref Nodes, cindex + i);
                child.SetParent(this);
                Children[i] = child;
            }
        }
    }
    public struct ProtoOctreeNode {
        public NodeData Data { get; private set; }
        public ushort ChildIndex { get; private set; }
        public ProtoOctreeNode(NodeData data, ushort index)
        {
            Data = data;
            ChildIndex = index;
        }
        public ProtoOctreeNode(byte dist, byte mat, ushort index)
        {
            Data = new NodeData(dist, mat);
            ChildIndex = index;
        }
        public static ProtoOctreeNode Deserialize(BinaryReader read)
        {
            return new ProtoOctreeNode(read.ReadByte(), read.ReadByte(), read.ReadUInt16());
        }
        public void Serialize(BinaryWriter write)
        {
            write.Write(Data.Material);
            write.Write(Data.Distance);
            write.Write(ChildIndex);
        }
    }
    public struct NodeData
    {
        public byte Distance{get; private set;}
        public byte Material{get; private set;}
        public NodeData(byte Material, byte Distance)
        {
            this.Distance = Distance;
            this.Material = Material;
        }
    }
}
