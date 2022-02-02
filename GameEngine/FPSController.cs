using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK.Mathematics;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Text;
using VainEngine.Physics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VainEngine
{
    public class FPSController : GameObject
    {
        public bool grounded, wall;
        public float camImpact;
        public Vector3 velocity { get { return new Vector3(rb.LinearVelocity.X, rb.LinearVelocity.Y, rb.LinearVelocity.Z); } set { rb.LinearVelocity = new Jitter.LinearMath.JVector(value.X, value.Y, value.Z); } }
        public Vector3 pos { get { return new Vector3(rb.Position.X, rb.Position.Y, rb.Position.Z); } set { rb.Position = new Jitter.LinearMath.JVector(value.X, value.Y, value.Z); } }

        public Vector3 camOffsetFollow { get; private set; }
        public Vector3 camOffset { get; private set; }

        public bool jumping;

        public float height = 0;
        private JVector wallNormal = JVector.Zero, wallRunPos;
        private Shape shape;
        public RigidBody rb;

        public void AddAcceleration(Vector3 acc)
        {
            rb.AddForce(10*new Jitter.LinearMath.JVector(acc.X, acc.Y, acc.Z));
        }
        public void AddAccelerationImpulse(Vector3 acc, float impulse)
        {
            rb.ApplyImpulse(new Jitter.LinearMath.JVector(acc.X, acc.Y, acc.Z) * 0.01f * impulse);
        }
        GameObject follower;
        public FPSController(float height)
        {
            follower = new GameObject();
            follower.viewMesh = new Mesh(@"Resources\capsule.obj", Texture.LoadFromFile(@"Resources\grid.png"));
            follower.visible = true;
            follower.viewMesh.scale = 1;
            this.height = height;
            CapsuleShape body = new CapsuleShape(Math.Clamp((height)-6f, 0, 5), 3f);
            rb = new RigidBody(body);
            rb.Material.KineticFriction = -0.2f;
            rb.Material.StaticFriction = -0.2f;
            rb.Mass = 0.08f;
            rb.AllowDeactivation = false;
            Window.world.AddBody(rb);
            shape = body;
            Window.collision.CollisionDetected += CollisionDetected;
            //Window.collision.
        }
        public void DoPhysics(float ftime)
        {
            frametime = ftime;
            rb.Shape = new CapsuleShape(Math.Clamp((height) - 0.2f, 0, 5), 0.1f);
            rb.IsActive = true;
            rb.LinearVelocity += new JVector(0, -0.01f, 0);
            rb.Orientation = Jitter.LinearMath.JMatrix.Identity;
            rb.AngularVelocity = JVector.Zero;
            position = new Vector3(rb.Position.X, rb.Position.Y + (height / 2), rb.Position.Z);
            follower.position = position;
            camImpact = Lerp(camImpact, 0, frametime * 10);
            Console.WriteLine(velocity);
            if (rb.LinearVelocity.Y < -0.05)
            {
                grounded = false;
            }
            if (grounded)
            {
                rb.AddForce(new JVector(0, 0, 0));
            }
            if (pos.X == float.NaN || pos.Y == float.NaN || pos.Z == float.NaN)
                pos = Vector3.Zero;
            Camera.up = Vector3.Lerp(Camera.up, targetCamUp, 0.05f);
            targetCamUp = Vector3.Lerp(targetCamUp, -Vector3.UnitY, 0.05f);
            
        }
        Vector3 targetCamUp = Vector3.Zero;
        private float frametime = 1;
        public Vector3 gunPos;

        bool wasGrounded = false;
        float airMovement = 1.5f, gndMovement = 6f, jumpPower = 16f, wallJumpPower = 18;
        float gunKnockbackOffset = 0;
        bool wasCrouching;
        float shootCooldown = 0;

        public void CollisionDetected(RigidBody body1, RigidBody body2, JVector point1, JVector point2, JVector normal, float penetration)
        {
            if(body1 == rb && Vector3.Dot(new Vector3(normal.X, normal.Y, normal.Z), Vector3.UnitY) > 0.8f)
            {

                grounded = true;
                wall = false;
            }
            if (body1 == rb && Vector3.Dot(new Vector3(normal.X, normal.Y, normal.Z), Vector3.UnitY) > -0.01f && Vector3.Dot(new Vector3(normal.X, normal.Y, normal.Z), Vector3.UnitY) < 0.01f)
            {
                if(!wall && wallNormal != normal)
                {
                    wall = true;
                    rb.LinearVelocity = new JVector(rb.LinearVelocity.X, 17, rb.LinearVelocity.Z);
                }
                
                wallNormal = normal;
                //if (!wall) { }
                //wallRunPos = new JVector(pos.X, pos.Y, pos.Z);
                targetCamUp = (new Vector3(normal.X, -normal.Y, normal.Z)/2.5f) + -Vector3.UnitY;
                rb.LinearVelocity = new JVector(rb.LinearVelocity.X/(1+frametime), rb.LinearVelocity.Y / (1 + frametime), rb.LinearVelocity.Z / (1 + frametime));
                rb.AddForce(new JVector(-normal.X*8, -normal.Y*8, -normal.Z*8));
                rb.AddForce(new JVector(0,5,0));
                //rb.Position -= new JVector(normal.X/90, normal.Y / 90, normal.Z / 90);
                if (jumping)
                {
                    rb.LinearVelocity = new JVector(rb.LinearVelocity.X, 13, rb.LinearVelocity.Z) + new JVector(normal.X * wallJumpPower, normal.Y * wallJumpPower, normal.Z * wallJumpPower);
                    rb.Position += new JVector(normal.X * (penetration + 0.01f), normal.Y * (penetration + 0.01f), normal.Z * (penetration + 0.01f));
                    wall = false;
                }
                    
          
            }
        }
        public bool debugMode = false, debugAllowed = false;
        public void Movement(KeyboardState keyboard, MouseState mouse,float frameTime)
        {
            Camera.fov = debugMode?90:85;
            if (debugMode)
            {
                rb.LinearVelocity = JVector.Zero;
                grounded = true;
                wasGrounded = true;
            }

            if (keyboard.IsKeyDown(Keys.F) && !keyboard.WasKeyDown(Keys.F) && debugAllowed)
                debugMode = !debugMode;
            
            bool crouching = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.LeftShift);
            jumping = keyboard.IsKeyDown(Keys.Space);

            if (!debugMode)
            {
                if (velocity.X == float.NaN || velocity.Y == float.NaN || velocity.Z == float.NaN || position.X == float.NaN || position.Y == float.NaN || position.Z == float.NaN)
                    SceneManagement.SceneManager.Reload();
                

                if (crouching)
                    height = 0.3f;
                else
                    height = 1;
                if (!crouching && wasCrouching)
                {
                    position.Y += 0.8f;
                    gunPos.Y += 1f;
                    rb.Update();
                }
                if (crouching && !wasCrouching)
                {
                    position.Y -= 0.8f;
                    gunPos.Y -= 1f;
                    rb.Update();
                }
                //AddAcceleration(Vector3.UnitY / -100);

                if (grounded && !crouching)
                {
                    if (keyboard.IsKeyDown(Keys.W))
                        AddAcceleration(localfwd(gndMovement));
                    if (keyboard.IsKeyDown(Keys.S))
                        AddAcceleration(localfwd(-gndMovement));
                    if (keyboard.IsKeyDown(Keys.A))
                        AddAcceleration(localrht(-gndMovement));
                    if (keyboard.IsKeyDown(Keys.D))
                        AddAcceleration(localrht(gndMovement));
                    if (!wasGrounded)
                    {
                        camOffset = new Vector3(0, camImpact, 0);
                    }

                }
                else if (!grounded)
                {
                    Vector3 widhDir = Vector3.Zero;
                    if (keyboard.IsKeyDown(Keys.W))
                        widhDir += localfwd(1);
                    if (keyboard.IsKeyDown(Keys.S))
                        widhDir += localfwd(-1);
                    if (keyboard.IsKeyDown(Keys.A))
                        widhDir += localrht(-1);
                    if (keyboard.IsKeyDown(Keys.D))
                        widhDir += localrht(1);
                    widhDir.NormalizeFast();
                    Vector3 mov = velocity;
                    mov.Y = 0;
                    AddAcceleration((widhDir * (1f-(Vector3.Dot(widhDir, mov.Normalized())*0.9f)))+(widhDir/(0.5f+(mov.LengthFast))));
                }
                if (grounded)
                {
                    if (keyboard.IsKeyDown(Keys.Space))
                    {
                        velocity = new Vector3(velocity.X, jumpPower, velocity.Z);
                        grounded = false;
                    }
                }
                if (grounded && !wasGrounded)
                {
                    camOffset = new Vector3(0, camImpact, 0);
                }

                if (grounded)
                {
                    if (!crouching)
                    {
                        AddAcceleration((velocity * -0.2f));
                        if (!(keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.D)))
                            AddAcceleration((velocity * -0.4f));
                    }
                    else
                    {
                        //AddAcceleration(((velocity * 1.5f) * (-velocity.LengthFast / 4)) / 7);
                        AddAcceleration(new Vector3(0, -1.5f, 0));
                    }
                }
            }
            else
            {
                if (jumping)
                    rb.Position += new JVector(0, frameTime*7, 0);
                if(crouching)
                    rb.Position += new JVector(0, -frameTime*7, 0);
                if (keyboard.IsKeyDown(Keys.W))
                    rb.Position += Jlocalfwd(7 * frameTime);
                if (keyboard.IsKeyDown(Keys.S))
                    rb.Position -= Jlocalfwd(7 * frameTime);
                if (keyboard.IsKeyDown(Keys.A))
                    rb.Position -= Jlocalrht(7 * frameTime);
                if (keyboard.IsKeyDown(Keys.D))
                    rb.Position += Jlocalrht(7 * frameTime);
            }
            Camera.euler.X = Math.Clamp(Camera.euler.X, -85, 85);

            camOffsetFollow = Vector3.Lerp(camOffsetFollow, camOffset, frameTime * 8);
            camOffset = Vector3.Lerp(camOffset, new Vector3(0, -0.25f, 0), frameTime * 10);
            if (grounded && !wasGrounded)
                camImpact = -20;
                wasGrounded = grounded;
            wasCrouching = crouching;
            
        }
        Vector3 localfwd(float intensity)
        {
            return intensity * new Vector3((float)Math.Cos(MathHelper.DegreesToRadians(-Camera.euler.Y)), 0, (float)Math.Sin(MathHelper.DegreesToRadians(-Camera.euler.Y)));
        }
        Vector3 localrht(float intensity)
        {
            return intensity * new Vector3((float)Math.Cos(MathHelper.DegreesToRadians(-Camera.euler.Y - 90)), 0, (float)Math.Sin(MathHelper.DegreesToRadians(-Camera.euler.Y - 90)));
        }
        JVector Jlocalfwd(float intensity)
        {
            return new JVector(intensity * (float)Math.Cos(MathHelper.DegreesToRadians(-Camera.euler.Y)), 0, intensity * (float)Math.Sin(MathHelper.DegreesToRadians(-Camera.euler.Y)));
        }
        JVector Jlocalrht(float intensity)
        {
            return new JVector(intensity * (float)Math.Cos(MathHelper.DegreesToRadians(-Camera.euler.Y - 90)), 0, intensity * (float)Math.Sin(MathHelper.DegreesToRadians(-Camera.euler.Y - 90)));
        }
        float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }
    }
}
