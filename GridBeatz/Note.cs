using VainEngine;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace GridBeatz
{
    public class Note : Component
    {
        public Conductor conductor;
        public float targetTime;
        //UIBox noteReperesentation = new UIBox(0.2f, 0.01f, Texture.LoadFromFile(@"Resources\white.png"));
        public Keys assignedKey;
        public override void Start()
        {
            //noteReperesentation.position.X = 0.7f;
            base.Start();
        }
        public override void Update()
        {
            float timeUntilHit = (((targetTime*60)/conductor.BPM)*10)-(conductor.time * 10);
            //noteReperesentation.position.Y = (timeUntilHit / 10);
            timeUntilHit += VMath.Power(timeUntilHit / 10, 30);
            obj.position.Z = timeUntilHit*3;
            obj.viewMesh.emission = 4- VMath.Clamp(VMath.Power(0.3f+timeUntilHit,4), -20, 4);
            if (obj.position.Z < -0.7)
            {
                obj.viewMesh.color = Color4.Red;
                obj.scale = 0.2f;
            }
            if (VMath.Absolute(timeUntilHit) < 0.7 && Program.w.IsKeyDown(assignedKey))
            {
                obj.Destroy();
                conductor.score += 100 - (int)(VMath.Absolute(timeUntilHit)*50);
                conductor.UpdateScore();
                return;
            }
            if (timeUntilHit < -2f)
            {
                //noteReperesentation.Destroy();
                //noteReperesentation = null;
                obj.Destroy();
                return;
            }

            //noteReperesentation.RebuildMesh();
        }
    }
}
