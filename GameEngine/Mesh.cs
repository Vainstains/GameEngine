using System;
using System.Collections.Generic;
using System.Text;
using ObjParser;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace VainEngine
{
    public class Mesh
    {
        public static List<Mesh> Meshes = new List<Mesh>();

        public float[] _vertices;

        public uint[] _indices;

        private int _elementBufferObject;

        private int _vertexBufferObject;

        private int _vao;

        private Shader _lightingShader, _lampShader;

        private Texture _texture, _emissionMap;
        public Vector3 position, euler;
        public float scale;
        public static Vector3 _lightPos = new Vector3(1.2f, -1.0f, 2.0f);
        public Color4 color = Color4.White;
        public enum DataMapType
        {
            Emission,
            Roughness,
            Specular,
            Combined
        }
        public void SetTexture(Texture t)
        {
            _texture = t;
        }
        public void SetMap(DataMapType m, Texture t)
        {
            switch(m)
            {
                case DataMapType.Emission:
                    {
                        _emissionMap = t;
                        _emissionMap.Use(TextureUnit.Texture1);
                        break;
                    }
            }
            
        }
        public Mesh SetTextureMesh(Texture t)
        {
            _texture = t;
            return this;
        }
        public Mesh(string path, Texture texture, float texScale = 1, bool useWorldPosAsTexCoord = false)
        {
            Obj obj = new Obj();
            obj.LoadObj(path);
            List<float> vertices = new List<float>();
            List<uint> indices = new List<uint>();
            for (int i = 0; i < obj.FaceList.Count; i++)
            {
                var faces = obj.FaceList;
                var verts = obj.VertexList;
                var tex = obj.TextureList;
                var norms = obj.NormalList;
                //var v1 = verts[faces[i].VertexIndexList[0] - 1];
                //var v2 = verts[faces[i].VertexIndexList[1] - 1];
                //var v3 = verts[faces[i].VertexIndexList[2] - 1];
                //var e1 = new Vector3((float)(v1.X - v2.X), (float)(v1.Y - v2.Y), (float)(v1.Z - v2.Z));
                //var e2 = new Vector3((float)(v1.X - v3.X), (float)(v1.Y - v3.Y), (float)(v1.Z - v3.Z));
                //var normal = Vector3.Cross(e1, e2);
                float Nx = ((float)norms[faces[i].VertexNormalList[0] - 1].X + (float)norms[faces[i].VertexNormalList[1] - 1].X + (float)norms[faces[i].VertexNormalList[2] - 1].X) / 3;
                float Ny = ((float)norms[faces[i].VertexNormalList[0] - 1].Y + (float)norms[faces[i].VertexNormalList[1] - 1].Y + (float)norms[faces[i].VertexNormalList[2] - 1].Y) / 3;
                float Nz = ((float)norms[faces[i].VertexNormalList[0] - 1].Z + (float)norms[faces[i].VertexNormalList[1] - 1].Z + (float)norms[faces[i].VertexNormalList[2] - 1].Z) / 3;
                for (int i1 = 0; i1 < 3; i1++)
                {
                    float x = (float)verts[faces[i].VertexIndexList[i1] - 1].X;
                    float y = (float)verts[faces[i].VertexIndexList[i1] - 1].Y;
                    float z = (float)verts[faces[i].VertexIndexList[i1] - 1].Z;
                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(z);
                    if (useWorldPosAsTexCoord)
                    {
                        vertices.Add((x) * texScale);
                        vertices.Add((z + y) * texScale);
                    }
                    else
                    {
                        vertices.Add((float)tex[faces[i].TextureVertexIndexList[i1] - 1].X * texScale);
                        vertices.Add((float)tex[faces[i].TextureVertexIndexList[i1] - 1].Y * texScale);
                    }
                    

                    vertices.Add((float)norms[faces[i].VertexNormalList[i1] - 1].X);
                    vertices.Add((float)norms[faces[i].VertexNormalList[i1] - 1].Y);
                    vertices.Add((float)norms[faces[i].VertexNormalList[i1] - 1].Z);
                    //Console.WriteLine($"{(float)verts[faces[i].VertexIndexList[i1] - 1].X}, {(float)verts[faces[i].VertexIndexList[i1] - 1].Y}, {(float)verts[faces[i].VertexIndexList[i1] - 1].Z} [{(float)tex[faces[i].TextureVertexIndexList[i1] - 1].X}, {(float)tex[faces[i].TextureVertexIndexList[i1] - 1].Y}]");
                }

                indices.Add((uint)((i * 3)));
                indices.Add((uint)((i * 3) + 1));
                indices.Add((uint)((i * 3) + 2));
            }
            _vertices = vertices.ToArray();
            _indices = indices.ToArray();
            _texture = texture;
            Bind();
        }
        public Mesh(string path, Texture texture, Vector3 scale, float texScale = 1, bool useWorldPosAsTexCoord = false)
        {
            Obj obj = new Obj();
            obj.LoadObj(path);
            List<float> vertices = new List<float>();
            List<uint> indices = new List<uint>();
            for (int i = 0; i < obj.FaceList.Count; i++)
            {
                var faces = obj.FaceList;
                var verts = obj.VertexList;
                var tex = obj.TextureList;
                var norms = obj.NormalList;
                //var v1 = verts[faces[i].VertexIndexList[0] - 1];
                //var v2 = verts[faces[i].VertexIndexList[1] - 1];
                //var v3 = verts[faces[i].VertexIndexList[2] - 1];
                //var e1 = new Vector3((float)(v1.X - v2.X), (float)(v1.Y - v2.Y), (float)(v1.Z - v2.Z));
                //var e2 = new Vector3((float)(v1.X - v3.X), (float)(v1.Y - v3.Y), (float)(v1.Z - v3.Z));
                //var normal = Vector3.Cross(e1, e2);
                float Nx = ((float)norms[faces[i].VertexNormalList[0] - 1].X + (float)norms[faces[i].VertexNormalList[1] - 1].X + (float)norms[faces[i].VertexNormalList[2] - 1].X) / 3;
                float Ny = ((float)norms[faces[i].VertexNormalList[0] - 1].Y + (float)norms[faces[i].VertexNormalList[1] - 1].Y + (float)norms[faces[i].VertexNormalList[2] - 1].Y) / 3;
                float Nz = ((float)norms[faces[i].VertexNormalList[0] - 1].Z + (float)norms[faces[i].VertexNormalList[1] - 1].Z + (float)norms[faces[i].VertexNormalList[2] - 1].Z) / 3;
                for (int i1 = 0; i1 < 3; i1++)
                {
                    float x = (float)verts[faces[i].VertexIndexList[i1] - 1].X;
                    float y = (float)verts[faces[i].VertexIndexList[i1] - 1].Y;
                    float z = (float)verts[faces[i].VertexIndexList[i1] - 1].Z;
                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(z);
                    if (useWorldPosAsTexCoord)
                    {
                        vertices.Add((x) * texScale);
                        vertices.Add((z + y) * texScale);
                    }
                    else
                    {
                        vertices.Add((float)tex[faces[i].TextureVertexIndexList[i1] - 1].X * texScale);
                        vertices.Add((float)tex[faces[i].TextureVertexIndexList[i1] - 1].Y * texScale);
                    }

                    vertices.Add((float)norms[faces[i].VertexNormalList[i1] - 1].X);
                    vertices.Add((float)norms[faces[i].VertexNormalList[i1] - 1].Y);
                    vertices.Add((float)norms[faces[i].VertexNormalList[i1] - 1].Z);
                    //Console.WriteLine($"{(float)verts[faces[i].VertexIndexList[i1] - 1].X}, {(float)verts[faces[i].VertexIndexList[i1] - 1].Y}, {(float)verts[faces[i].VertexIndexList[i1] - 1].Z} [{(float)tex[faces[i].TextureVertexIndexList[i1] - 1].X}, {(float)tex[faces[i].TextureVertexIndexList[i1] - 1].Y}]");
                }

                indices.Add((uint)((i * 3)));
                indices.Add((uint)((i * 3) + 1));
                indices.Add((uint)((i * 3) + 2));
            }
             _vertices = vertices.ToArray();
            _indices = indices.ToArray();
            _texture = texture;
            Bind();
        }
        private void Bind()
        {
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);




            // shader.vert has been modified. Take a look at it after the explanation in OnRenderFrame.
            _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            var vertexLocation = _lightingShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            var texCoordLocation = _lightingShader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var normalLocation = _lightingShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));

            _texture.Use(TextureUnit.Texture0);

            _lightingShader.SetInt("texture0", 0);
            _emissionMap = Texture.Blank();
            _emissionMap.Use(TextureUnit.Texture1);

            _lightingShader.SetInt("emissionMap", 1);
        }
        Vector3 coordFix = new Vector3(1, -1, 1);
        public float emission = 0.2f;
        public void Draw()
        {
            GL.BindVertexArray(_vao);

            _texture.Use(TextureUnit.Texture0);
            _emissionMap.Use(TextureUnit.Texture1);


            // Finally, we have the model matrix. This determines the position of the model.
            var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(180 - euler.X)) * Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(euler.Z)) * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(euler.Y)) * Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(position);

            _lightingShader.SetMatrix4("model", model);
            _lightingShader.SetMatrix4("view", Camera.view);
            _lightingShader.SetMatrix4("projection", Camera.projection);
            _lightingShader.SetVector3("lightColor", new Vector3(1f, 1f, 1.0f));
            _lightingShader.SetVector3("ambientColor", new Vector3(1.4f, 1.6f, 2.0f));
            _lightingShader.SetFloat("lightPower", 1.4f);
            _lightingShader.SetFloat("ambientPower", 1f);
            _lightingShader.SetFloat("fogStart", Window.FogStart);
            _lightingShader.SetFloat("fogEnd", Window.FogEnd);
            _lightingShader.SetFloat("shininess", 2f);
            _lightingShader.SetFloat("emission", emission);
            _lightingShader.SetVector3("lightPos", Window.sunPos*coordFix);
            _lightingShader.SetVector3("viewPos", Camera.pos * coordFix);
            _lightingShader.SetVector3("color", new Vector3(color.R/ 10, color.G / 10, color.B / 10));

            Matrix4 lampMatrix = Matrix4.CreateScale(0.2f);
            lampMatrix = lampMatrix * Matrix4.CreateTranslation(_lightPos);
            _lampShader.SetMatrix4("model", lampMatrix);
            _lampShader.SetMatrix4("view", Camera.view);
            _lampShader.SetMatrix4("projection", Camera.projection);
            _lampShader.SetVector3("color", new Vector3(color.R / 10, color.G / 10, color.B / 10));
            _lightingShader.Use();

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
