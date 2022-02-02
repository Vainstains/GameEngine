using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VainEngine.SceneManagement;
using VainEngine.SceneManagement.Data;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using VainEngine.Physics;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;

namespace VainEngine
{
    public class Scene
    {
        public string sceneData, name;
        public Scene(string path)
        {
            StreamReader fs = new StreamReader(path);
            sceneData = fs.ReadToEnd();
            name = new FileInfo(path).Name;
        }
    }
    namespace SceneManagement.Data
    {
        public class PlayerSpawnPos
        {
            public string x { get; set; }
            public string y { get; set; }
            public string z { get; set; }
        }

        public class Box
        {
            public string x { get; set; }
            public string y { get; set; }
            public string z { get; set; }
            public string scale { get; set; }
            public string rot1 { get; set; }
            public string rot2 { get; set; }
            public string r { get; set; }
            public string g { get; set; }
            public string b { get; set; }
        }
        public class Light
        {
            public string x { get; set; }
            public string y { get; set; }
            public string z { get; set; }
            public string r { get; set; }
            public string g { get; set; }
            public string b { get; set; }
            public string specular { get; set; }
            public string power { get; set; }
            public string distance { get; set; }
        }

        public class sceneData
        {
            public PlayerSpawnPos PlayerSpawnPos { get; set; }
            public List<Box> Boxes { get; set; }
            public List<Light> Lights { get; set; }
            public string skyR { get; set; }
            public string skyG { get; set; }
            public string skyB { get; set; }
        }

        public class Root
        {
            public sceneData Scene { get; set; }
        }
    }
    namespace SceneManagement
    {
        public static class SceneManager
        {
            public static Scene current;
            public static void LoadScene(Scene scene)
            {
                {
                    List<GameObject> toDestroy = new List<GameObject>();
                    foreach (GameObject g in GameObject.objects)
                    {
                        if (!g.dontDestroyOnSceneLoad)
                        {
                            toDestroy.Add(g);
                        }
                    }
                    var d = toDestroy.ToArray();
                    for (int i = 0; i < d.Length; i++)
                    {
                        if (d[i].collider != null)
                        {
                            d[i].collider.Destroy();
                        }
                        d[i].Destroy();
                    }
                }//destroy gameObjects
                {
                    List<Light> toDestroy = new List<Light>();
                    foreach (Light l in Light.lights)
                    {
                        toDestroy.Add(l);
                    }
                    var d = toDestroy.ToArray();
                    for (int i = 0; i < d.Length; i++)
                    {
                        d[i].Destroy();
                    }
                }//destroy Lights
                Root sceneRoot = JsonConvert.DeserializeObject<Root>(scene.sceneData);
                foreach (Box b in sceneRoot.Scene.Boxes)
                {
                    GameObject box = GameObject.Primitives.Box(float.Parse(b.scale) * 0.25f);
                    box.scale = float.Parse(b.scale);
                    box.position = new Vector3(float.Parse(b.x), float.Parse(b.y), float.Parse(b.z));
                    box.eulerRot = new Vector3(float.Parse(b.rot1)+90, float.Parse(b.rot2), 0);
                    box.collider = new Collider(new BoxShape(new JVector(float.Parse(b.scale))), box, new Vector3(float.Parse(b.rot1), float.Parse(b.rot2), 0));
                    box.viewMesh.SetTexture(Texture.LoadFromFile(@"Resources\floor.jpg"));
                    box.viewMesh.color = new Color4(float.Parse(b.r)/255, float.Parse(b.g)/255, float.Parse(b.b)/255, 1);
                }
                foreach (Data.Light l in sceneRoot.Scene.Lights)
                {
                    Light.lights.Add(new Light(new Vector3(float.Parse(l.x), float.Parse(l.y), float.Parse(l.z)), new Color4(float.Parse(l.r), float.Parse(l.g), float.Parse(l.b), 1), float.Parse(l.specular), float.Parse(l.power)/10, float.Parse(l.distance)));
                    GameObject box = GameObject.Primitives.Sphere(1);
                    box.scale = 0.3f;
                    box.position = new Vector3(float.Parse(l.x), float.Parse(l.y), float.Parse(l.z));
                    box.eulerRot = new Vector3(0, 0, 0);
                    box.collider = new Collider(new BoxShape(new JVector(0.3f)), box, new Vector3(0, 0, 0));
                    box.viewMesh.SetTexture(Texture.LoadFromFile(@"Resources\white.png"));
                    box.viewMesh.color = new Color4(float.Parse(l.r), float.Parse(l.g), float.Parse(l.b), 0.5f);
                    box.viewMesh.emission = 100000* float.Parse(l.power);
                }
                Global.SkyColor(new Color4(float.Parse(sceneRoot.Scene.skyR) / 255, float.Parse(sceneRoot.Scene.skyG) / 255, float.Parse(sceneRoot.Scene.skyB) / 255,1));
                current = scene;
            }
            public static void Reload()
            {
                LoadScene(current);
            }
        }
    }
}