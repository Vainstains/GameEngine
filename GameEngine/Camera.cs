using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VainEngine
{
    public static class Camera
    {
        public static Vector3 pos;
        public static Vector3 euler;
        public static Vector3 front;
        public static Vector3 up;
        public static float fov;
        public static Matrix4 view, projection;
    }
}
