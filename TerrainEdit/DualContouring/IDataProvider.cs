using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;
namespace TerrainEdit.DualContouring
{
    public interface IDataProvider
    {
        float GetDistanceValue(Vector3 val);
        byte GetMaterialValue(Vector3 val);
    }
    public class PerlinNoiseProvider : IDataProvider
    {
        Vector3 offset;
        public PerlinNoiseProvider(Component c)
        {
            offset = c.gameObject.transform.position;
        }
        Perlin p = new Perlin() { Frequency = .04f };
        public float GetDistanceValue(Vector3 val)
        {
            //return (10 - (val - new Vector3(45, 45, 45) / 2f).magnitude);
            return 1 - 2 * (float)p.GetValue(val.x + (int)offset.x, val.y + (int)(offset.y), val.z + (int)(offset.z));//Mathf.Max((7 - (val + new Vector3(-8f, -8f, -8f)).magnitude),(7 - (val + new Vector3(-12f, -12f, -12f)).magnitude));
        }

        public byte GetMaterialValue(Vector3 val)
        {
            throw new System.NotImplementedException();
        }
    }
}
