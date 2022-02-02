using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace VainEngine
{
    public class UIBox
    {
        public static List<UIBox> boxes = new List<UIBox>();
        public Vector2 position;
        public Color4 color = Color4.White;
        private Shader shader;
        private Texture texture;
        public Vector2 scale;
        int vbo, vao;
        public UIBox(float scaleX, float scaleY, Texture t)
        {
            scale = new Vector2(scaleX, scaleY);
            float[] vertices = new float[30];
            vertices[0] = position.X - (scaleX / 2);
            vertices[1] = position.Y + (scaleY / 2);
            vertices[2] = 0;
            vertices[3] = 0;
            vertices[4] = 0;

            vertices[5] = position.X - (scaleX / 2);
            vertices[6] = position.Y - (scaleY / 2);
            vertices[7] = 0;
            vertices[8] = 0;
            vertices[9] = 1;

            vertices[10] = position.X + (scaleX / 2);
            vertices[11] = position.Y - (scaleY / 2);
            vertices[12] = 0;
            vertices[13] = 1;
            vertices[14] = 1;

            //tri 2

            vertices[15] = position.X + (scaleX / 2);
            vertices[16] = position.Y - (scaleY / 2);
            vertices[17] = 0;
            vertices[18] = 1;
            vertices[19] = 1;

            vertices[20] = position.X + (scaleX / 2);
            vertices[21] = position.Y + (scaleY / 2);
            vertices[22] = 0;
            vertices[23] = 1;
            vertices[24] = 0;

            vertices[25] = position.X - (scaleX / 2);
            vertices[26] = position.Y + (scaleY / 2);
            vertices[27] = 0;
            vertices[28] = 0;
            vertices[29] = 0;

            shader = new Shader("Shaders/text.vert", "Shaders/text.frag");

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            var vertexLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            texture = t;

            texture.Use(TextureUnit.Texture0);

            boxes.Add(this);
            
        }
        public void Destroy()
        {
            boxes.Remove(this);
        }
        public void RebuildMesh()
        {
            float[] vertices = new float[30];
            vertices[0] = position.X - (scale.X / 2);
            vertices[1] = position.Y + (scale.Y / 2);
            vertices[2] = 0;
            vertices[3] = 0;
            vertices[4] = 0;

            vertices[5] = position.X - (scale.X / 2);
            vertices[6] = position.Y - (scale.Y / 2);
            vertices[7] = 0;
            vertices[8] = 0;
            vertices[9] = 1;

            vertices[10] = position.X + (scale.X / 2);
            vertices[11] = position.Y - (scale.Y / 2);
            vertices[12] = 0;
            vertices[13] = 1;
            vertices[14] = 1;

            //tri 2

            vertices[15] = position.X + (scale.X / 2);
            vertices[16] = position.Y - (scale.Y / 2);
            vertices[17] = 0;
            vertices[18] = 1;
            vertices[19] = 1;

            vertices[20] = position.X + (scale.X / 2);
            vertices[21] = position.Y + (scale.Y / 2);
            vertices[22] = 0;
            vertices[23] = 1;
            vertices[24] = 0;

            vertices[25] = position.X - (scale.X / 2);
            vertices[26] = position.Y + (scale.Y / 2);
            vertices[27] = 0;
            vertices[28] = 0;
            vertices[29] = 0;

            shader = new Shader("Shaders/text.vert", "Shaders/text.frag");

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            var vertexLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            texture.Use(TextureUnit.Texture0);
        }
        public void Draw()
        {
            texture.Use(TextureUnit.Texture0);
            if (Camera.vr)
                shader.SetFloat("sx", Window.ScaleX/2);
            else
                shader.SetFloat("sx", Window.ScaleX / 2);
            shader.SetFloat("sy", Window.ScaleY / 2);

            shader.SetVector3("color", new Vector3(color.R / 1, color.G / 1, color.B / 1));
            shader.SetFloat("alpha", color.A);

            shader.Use();
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
    }
}
