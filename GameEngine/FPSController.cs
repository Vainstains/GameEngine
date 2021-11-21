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
        public FPSController(float height)
        {
            this.height = height;
            CapsuleShape body = new CapsuleShape(Math.Clamp((height)-2f, 0, 5), 1f);
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
            camImpact = Lerp(camImpact, rb.LinearVelocity.Y, frametime * 2);
            Console.WriteLine(velocity);
            if (rb.LinearVelocity.Y < 0)
            {
                grounded = false;
            }
            if (grounded)
            {
                rb.AddForce(new JVector(0, -5, 0));
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
        float airMovement = 0.8f, gndMovement = 7f, jumpPower = 15f, wallJumpPower = 12;
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
                rb.AddForce(new JVector(normal.X*2, normal.Y*2, normal.Z*2));
                rb.AddForce(new JVector(0,5,0));
                if (jumping)
                {
                    rb.LinearVelocity = new JVector(rb.LinearVelocity.X, 13, rb.LinearVelocity.Z) + new JVector(normal.X * wallJumpPower, normal.Y * wallJumpPower, normal.Z * wallJumpPower);
                    wall = false;
                }
                    
          
            }
        }
        public bool debugMode = false;
        public void Movement(KeyboardState keyboard, MouseState mouse,float frameTime)
        {
            Camera.fov = debugMode?90:75;
            if (debugMode)
                rb.LinearVelocity = JVector.Zero;
            if (keyboard.IsKeyDown(Keys.F1) && !keyboard.WasKeyDown(Keys.F1))
                debugMode = !debugMode;
            
            bool crouching = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.LeftShift);
            jumping = keyboard.IsKeyDown(Keys.Space);

            if (!debugMode)
            {


                

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
                    if (keyboard.IsKeyDown(Keys.W))
                        AddAcceleration(localfwd(airMovement) * -(-1.2f + Vector3.Dot(localfwd(airMovement).Normalized(), velocity.Normalized())) / (0.6f + (localfwd(airMovement).Xz.LengthFast) / 2));
                    if (keyboard.IsKeyDown(Keys.S))
                        AddAcceleration(localfwd(-airMovement) * -(-1.2f + Vector3.Dot(localfwd(-airMovement).Normalized(), velocity.Normalized())) / (0.6f + (localfwd(airMovement).Xz.LengthFast) / 2));
                    if (keyboard.IsKeyDown(Keys.A))
                        AddAcceleration(localrht(-airMovement) * -(-1.2f + Vector3.Dot(localrht(-airMovement).Normalized(), velocity.Normalized())) / (0.6f + (localfwd(airMovement).Xz.LengthFast) / 2));
                    if (keyboard.IsKeyDown(Keys.D))
                        AddAcceleration(localrht(airMovement) * -(-1.2f + Vector3.Dot(localrht(airMovement).Normalized(), velocity.Normalized())) / (0.6f + (localfwd(airMovement).Xz.LengthFast) / 2));
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
                    rb.Position += new JVector(0, frameTime*3, 0);
                if(crouching)
                    rb.Position += new JVector(0, -frameTime*3, 0);
                if (keyboard.IsKeyDown(Keys.W))
                    rb.Position += Jlocalfwd(10 * frameTime);
                if (keyboard.IsKeyDown(Keys.S))
                    rb.Position -= Jlocalfwd(10 * frameTime);
                if (keyboard.IsKeyDown(Keys.A))
                    rb.Position -= Jlocalrht(10 * frameTime);
                if (keyboard.IsKeyDown(Keys.D))
                    rb.Position += Jlocalrht(10 * frameTime);
            }
            Camera.euler.X = Math.Clamp(Camera.euler.X, -85, 85);

            camOffsetFollow = Vector3.Lerp(camOffsetFollow, camOffset, frameTime * 8);
            camOffset = Vector3.Lerp(camOffset, new Vector3(0, -0.25f, 0), frameTime * 10);

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
