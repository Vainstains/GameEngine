using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VainEngine.Effects
{
    public class ParticleSystem
    {
        public static List<ParticleSystem> PSes;
        private List<Particle> particles = new List<Particle>();
        public float lifetime = 1, speed = 0.5f, jitter = 0, scale = 0.1f, gravity = 0, gravityJitter = 0;
        public int amount;
        public Texture tex;
        public int particlesAlive { get; private set; }
        public void Start(Vector3 pos, Vector3 dir)
        {
            if(particles != null)
            {
                foreach(Particle p in particles)
                {
                    p.Destroy();
                }
            }
            particles = new List<Particle>();
            for(int i = 0; i < amount; i++)
            {
                particles.Add(new Particle()
                {
                    position = pos,
                    velocity = (dir + (new Vector3(r.Next(-100, 100), r.Next(-100, 100), r.Next(-100, 100)).Normalized() * jitter)) * speed * (float)r.NextDouble(),
                    lifetime = lifetime,
                    viewMesh = Mesh.Meshes[0].SetTextureMesh(tex),
                    scale = scale * (float)r.NextDouble(),
                    euler = new Vector3(r.Next(-100, 100), r.Next(-100, 100), r.Next(-100, 100)).Normalized() * r.Next(0, 5),
                    seed = (float)r.NextDouble()
                });
                particlesAlive++;
            }
        }
        Random r = new Random();
        public float sizeChange;

        public void Update(float frameTime)
        {
            for(int i = 0; i < particles.Count; i++)
            {
                particles[i].Update(frameTime);
                particles[i].velocity /= 1f + (frameTime * 3);
                particles[i].velocity += Vector3.UnitY * gravity * frameTime * particles[i].seed;
                particles[i].scale /= 1f + (frameTime * 3f * ((float)((r.NextDouble() * 2) + 1) * (1 + particles[i].seed))/(10/sizeChange));
                particles[i].eulerRot += particles[i].euler * frameTime * 100;
                if (particles[i].time > particles[i].lifetime * (1 - particles[i].seed))
                {
                    particles[i].Destroy();
                    particlesAlive--;
                }
            }
        }
    }
}
