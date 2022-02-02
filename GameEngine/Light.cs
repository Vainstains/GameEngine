using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VainEngine
{
    public class Light
    {
        public static List<Light> lights = new List<Light>();
        public Vector3 position;
        public Color4 color;
        public float specular;
        public float power;
        public float distance;

        public Light(Vector3 position, Color4 color, float specular, float power, float distance)
        {
            this.position = position;
            this.color = color;
            this.specular = specular;
            this.power = power;
            this.distance = distance;
            lights.Add(this);
        }
        public void Destroy()
        {
            lights.Remove(this);
        }
    }
}
