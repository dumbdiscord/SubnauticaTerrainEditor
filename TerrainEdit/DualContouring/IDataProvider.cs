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
        Vector3 h = new Vector3(80, 80, 80);
        public PerlinNoiseProvider(Component c)
        {
            offset = c.gameObject.transform.position;
        }
        Perlin p = new Perlin() { Frequency = .08f };
        public float GetDistanceValue(Vector3 val)
        {
            //return ;
            return (1 - 2 * (get(val)));//Mathf.Max((7 - (val + new Vector3(-8f, -8f, -8f)).magnitude),(7 - (val + new Vector3(-12f, -12f, -12f)).magnitude));
        }
        // A bunch of inefficient stuff used for testing
        public float Adder(Vector3 val)
        {
            var radius = 35;

            return (Mathf.Clamp(((radius - ((val - (new Vector3(45, 45, 45) - offset)).magnitude))*10)/radius,-.05f,1));
        }

        public float Multiplier(Vector3 val)
        {
            var radius = 35;
            var dist = ((val - (new Vector3(45, 45, 45) - offset)).magnitude);
            if(dist<radius) return 1f;
            if(dist>radius*1.1f) return 0f;
            return (dist-radius)/(radius*.1f);
        }
        public float Multiplier2(Vector3 val)   
        {
            var radius = 35;
            var dist = ((val - (new Vector3(45, 45, 45) - offset)).magnitude);
            if (dist < radius*.9) return 0f;
            return ((radius-dist) / (radius * .1f));
        }
        public float get(Vector3 val)
        {
            return (float)p.GetValue(val.x + (int)offset.x + h.x, val.y + h.y + (int)(offset.y), val.z + h.z + (int)(offset.z));
        }
        public byte GetMaterialValue(Vector3 val)
        {
            throw new System.NotImplementedException();
        }
    }
}
