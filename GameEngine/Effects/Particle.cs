using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VainEngine.Effects
{
    class Particle : GameObject
    {
        public Vector3 velocity = Vector3.Zero;
        public float lifetime;
        public float time;
        public Vector3 euler;
        public float seed;
        public void Update(float frameTime)
        {
            position += velocity * frameTime;
            time += frameTime;
        }
    }
}
