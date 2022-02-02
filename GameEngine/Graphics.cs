using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace VainEngine
{
    public static class Graphics
    {
        public static List<Vector3[]> lineQueue = new List<Vector3[]>();
        public static void DrawLine(Vector3 start, Vector3 end, Color4 color)
        {
            lineQueue.Add(new Vector3[]{new Vector3(color.R, color.G, color.B),start, end});
        }
    }
}
