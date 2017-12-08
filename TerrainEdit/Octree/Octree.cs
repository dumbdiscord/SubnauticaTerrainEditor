using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainEdit
{
    public class Octree
    {
        public float Size { get; private set; }
        public Octree[] Children { get; private set; }
        public Octree ParentNode { get; private set; }
        public static readonly Vector3[] corners = new Vector3[]{
            (Vector3.forward + Vector3.up + Vector3.right)/2,
            (Vector3.forward + Vector3.up - Vector3.right)/2,
            (-Vector3.forward + Vector3.up + Vector3.right)/2,
            (-Vector3.forward + Vector3.up - Vector3.right)/2,
            (Vector3.forward - Vector3.up + Vector3.right)/2,
            (Vector3.forward - Vector3.up - Vector3.right)/2,
            (-Vector3.forward - Vector3.up + Vector3.right)/2,
            (-Vector3.forward - Vector3.up - Vector3.right)/2
        };
        LeafNodeData? _leafData;
        
        public LeafNodeData? LeafData
        {
            get
            {
                if (!IsLeafNode)
                {
                    if (_leafData != null)
                    {
                        return _leafData;
                    }
                    if (IsRootNode) return null;
                    return ParentNode.LeafData;
                }
                if (_leafData == null)
                {
                    return ParentNode.LeafData;
                }
                return _leafData;
            }
            set
            {
                _leafData = value;
            }
        }
        public Octree this[int index]
        {
            get
            {
                if (IsLeafNode)
                    return null;
                return Children[index];
            }
        }
        const float SqrtThreeOverTwo = 0.866025405f;
        public int Level
        {
            get
            {
                
                if (IsRootNode)
                    return 0;
                return ParentNode.Level + 1;
            }
        }
        public bool IsRootNode
        {
            get
            {
                return ParentNode == null;
            }
        }
        public bool IsLeafNode
        {
            get
            {
                return Children != null;
            }
        }
        public Vector3 Position { get; private set; }
        Octree(Vector3 position, float size, Octree Parent)
        {
            Position = position;
            Size = size;
            ParentNode = Parent;
        }
        public static Octree StartNewTree(Vector3 rootPosition, float startSize)
        {
            return new Octree(rootPosition, startSize, null);
        }
        public Octree GetRootNode()
        {
            if (IsRootNode) return this;
            return ParentNode.GetRootNode();
        }
        public IEnumerable<Octree> GetLeafNodes()
        {
            if(IsLeafNode)
            {
                yield return this;
                
            }
            else
            {
                foreach (var c in Children)
                {
                    foreach (var h in c.GetLeafNodes())
                    {
                        yield return h;
                    }
                }
            }


        }
        public void Split()
        {
            if (!IsLeafNode)
                return;
            Children = new Octree[8];
            for (int i = 0; i < 8; i++)
            {
                Children[i] = new Octree(Position + corners[i] * Size / 2, Size / 2f, this);
            }

        }
    }
    public struct LeafNodeData
    {
        public byte Material;
        public byte SignedDistance;
        public LeafNodeData(byte Material, byte SignedDistance)
        {
            this.Material = Material;
            this.SignedDistance = SignedDistance;
        }
        public static float GetActualDistanceFromByte(byte dist)
        {
            return 0;
        }
        public byte GetSignedDistanceFromFloat(float f)
        {
            return 0;
        }
    }
}
