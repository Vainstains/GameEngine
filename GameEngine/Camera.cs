using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VainEngine
{
    public static class Camera
    {
        public static bool orthographic;
        public static bool vr = false;
        public static float orthographicScale = 10;
        public static float nearClip = 0.01f;
        public static float farClip = 500f;
        public static float orthoNearClip = -1f;
        public static float orthoFarClip = 200f;
        public static Vector3 pos;
        public static Vector3 euler;
        public static Vector3 front;
        public static Vector3 up;
        public static float fov;
        public static Matrix4 view;
        public static Matrix4 projection;
    }
}
