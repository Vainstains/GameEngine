using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VainEngine.Physics
{
    public class Collider
    {
        public static List<Collider> colliders = new List<Collider>();
        public Shape shape;
        public RigidBody colliderbody;
        public Collider(Shape s, GameObject connectedBody)
        {
            shape = s;
            colliderbody = new RigidBody(shape);
            colliderbody.AffectedByGravity = false;
            colliderbody.IsStatic = true;
            colliderbody.Position = new JVector(connectedBody.position.X, -connectedBody.position.Y, connectedBody.position.Z);
            Window.world.AddBody(colliderbody);
            body = connectedBody;
        }
        public Collider(Shape s, GameObject connectedBody, Vector3 rot)
        {
            shape = s;
            colliderbody = new RigidBody(shape);
            colliderbody.AffectedByGravity = false;
            colliderbody.IsStatic = true;
            colliderbody.Position = new JVector(connectedBody.position.X, -connectedBody.position.Y, connectedBody.position.Z);
            Window.world.AddBody(colliderbody);
            body = connectedBody;
            colliderbody.Orientation = JMatrix.CreateRotationX(0.01745329f * rot.X) * JMatrix.CreateRotationY(0.01745329f * rot.Y) * JMatrix.CreateRotationZ(-0.01745329f * rot.Z);
        }
        public void Destroy()
        {
            colliders.Remove(this);
            Window.world.RemoveBody(colliderbody);
            colliderbody = null;
            shape = null;
        }
        public GameObject body;
        public Vector3 normal;
    }
}