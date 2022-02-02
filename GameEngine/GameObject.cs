using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics;
using System.Drawing;
using DocumentFormat.OpenXml.Drawing;
using VainEngine.Physics;

namespace VainEngine
{

    public class GameObject
    {
        public static List<GameObject> objects = new List<GameObject>();
        public Vector3 position = Vector3.Zero, eulerRot = Vector3.Zero;
        public float scale = 1;
        public string tag;
        private int index;
        public Mesh viewMesh;
        public Collider collider;
        public bool dontDestroyOnSceneLoad = false;
        public bool visible = true, useMatrixInsteadOfEuler = false;
        public Matrix4 rotation = Matrix4.Identity;
        public List<Component> components { get; private set; }

        public GameObject()
        {
            tag = "";
            position = Vector3.Zero;
            objects.Add(this);
            index = objects.IndexOf(this);
            components = new List<Component>();
        }
        public GameObject(string tag)
        {
            this.tag = tag;
            position = Vector3.Zero;
            objects.Add(this);
            index = objects.IndexOf(this);
            components = new List<Component>();
        }
        public static GameObject FindWithTag(string tag)
        {
            foreach (GameObject g in objects)
            {
                if (g.tag == tag)
                {
                    return g;
                }
            }
            return null;
        }
        public void Destroy()
        {
            objects.Remove(this);
        }
        public static class Primitives
        {
            private static Texture primitiveTexture = Texture.LoadFromFile(@"Resources\grid.png");
            public static GameObject Plane()
            {
                GameObject g = new GameObject();
                g.viewMesh = new Mesh(@"Resources\plane.obj", primitiveTexture, 1);
                return g;
            }
            public static GameObject Box()
            {
                GameObject g = new GameObject();
                g.viewMesh = new Mesh(@"Resources\box.obj", primitiveTexture, 1);
                return g;
            }
            public static GameObject Sphere()
            {
                GameObject g = new GameObject();
                g.viewMesh = new Mesh(@"Resources\sphere.obj", primitiveTexture, 1);
                return g;
            }
            public static GameObject Capsule()
            {
                GameObject g = new GameObject();
                g.viewMesh = new Mesh(@"Resources\capsule.obj", primitiveTexture, 1);
                return g;
            }
            public static GameObject Plane(float s)
            {
                GameObject g = new GameObject();
                g.viewMesh = new Mesh(@"Resources\plane.obj", primitiveTexture, s);
                return g;
            }
            public static GameObject Box(float s)
            {
                GameObject g = new GameObject();
                g.viewMesh = new Mesh(@"Resources\box.obj", primitiveTexture, s);
                return g;
            }
            public static GameObject Sphere(float s)
            {
                GameObject g = new GameObject();
                g.viewMesh = new Mesh(@"Resources\sphere.obj", primitiveTexture, s);
                return g;
            }
            public static GameObject Capsule(float s)
            {
                GameObject g = new GameObject();
                g.viewMesh = new Mesh(@"Resources\capsule.obj", primitiveTexture, s);
                return g;
            }
        }

        public Component AddComponent(Component c)
        {
            components.Add(c);
            c.obj = this;
            return c;
        }
        public void RemoveComponent<type>()
        {
            List<Component> toDestroy = new List<Component>();
            foreach (Component c in components)
            {
                if (c.GetType() == typeof(type))
                    toDestroy.Add(c);
            }
            foreach (Component c in toDestroy)
            {
                components.Remove((Component)c);
            }
        }
        public Component GetComponent<type>()
        {
            foreach (Component c in components)
            {
                if(c.GetType() == typeof(type))
                    return (Component)c;
            }
            return null;
        }
    }
}
