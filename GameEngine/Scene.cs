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
        public string sceneData;
        public Scene(string path)
        {
            StreamReader fs = new StreamReader(path);
            sceneData = fs.ReadToEnd();
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

        public class sceneData
        {
            public PlayerSpawnPos PlayerSpawnPos { get; set; }
            public List<Box> Boxes { get; set; }
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
            public static void LoadScene(Scene scene)
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
                    if(d[i].collider != null)
                    {
                        d[i].collider.Destroy();
                    }
                    d[i].Destroy();
                }
                Root sceneRoot = JsonConvert.DeserializeObject<Root>(scene.sceneData);
                foreach (Box b in sceneRoot.Scene.Boxes)
                {
                    GameObject box = GameObject.Primitives.Box(float.Parse(b.scale) * 2);
                    box.scale = float.Parse(b.scale);
                    box.position = new Vector3(float.Parse(b.x), float.Parse(b.y), float.Parse(b.z));
                    box.eulerRot = new Vector3(float.Parse(b.rot1), float.Parse(b.rot2), 0);
                    box.collider = new Collider(new BoxShape(new JVector(float.Parse(b.scale))), box, new Vector3(float.Parse(b.rot1), float.Parse(b.rot2), 0));
                    box.viewMesh.SetTexture(Texture.LoadFromFile(@"Resources\tile.png"));
                    box.viewMesh.color = new Color4(float.Parse(b.r)/255, float.Parse(b.g)/255, float.Parse(b.b)/255, 1);
                }
            }
        }
    }
}