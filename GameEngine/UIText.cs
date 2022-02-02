using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace VainEngine
{
    public class UIText
    {
        public static List<UIText> texts = new List<UIText>();
        public Vector2 position;
        public Color4 color = Color4.White;
        public string text { get; private set; }
        private static readonly Atlas atlas = new Atlas(2048, 2048, 128, 128, Texture.LoadFromFile(@"Resources\TextAtlas.png"));
        private static string CharSheet = " ☺︎☻︎♥︎♦︎♣︎♠︎•︎◘︎○︎◙︎♂︎♀︎♪︎♫︎☼︎►︎◄︎↕︎‼︎¶︎§︎▬︎↨︎↑︎↓︎→︎←︎∟︎↔︎▲︎▼︎ ︎!︎\"#︎$︎%︎&︎'︎(︎)︎*︎+︎,︎-.︎/︎0︎1︎2︎3︎4︎5︎6︎7︎8︎9︎:︎;︎<︎=︎>︎?︎@︎A︎B︎C︎D︎E︎F︎G︎H︎I︎J︎K︎L︎M︎N︎O︎P︎Q︎R︎S︎T︎U︎V︎W︎X︎Y︎Z︎[︎\\]︎^︎_︎`︎a︎b︎c︎d︎e︎f︎g︎h︎i︎j︎k︎l︎m︎n︎o︎p︎q︎r︎s︎t︎u︎v︎w︎x︎y︎z︎{︎¦︎}︎~︎⌂︎Ç︎ü︎é︎â︎ä︎à︎å︎ç︎ê︎ë︎è︎ï︎î︎ì︎Ä︎Å︎É︎æ︎Æ︎ô︎ö︎ò︎û︎ù︎ÿ︎Ö︎Ü︎¢︎£︎¥︎₧︎ƒ︎á︎í︎ó︎ú︎ñ︎Ñ︎ª︎º︎¿︎⌐︎¬︎½︎¼︎¡︎«︎»︎░︎▒︎▓︎│︎┤︎╡︎╢︎╖︎╕︎╣︎║︎╗︎╝︎╜︎╛︎┐︎└︎┴︎┬︎├︎─︎┼︎╞︎╟︎╚︎╔︎╩︎╦︎╠︎═︎╬︎╧︎╨︎╤︎╥︎╙︎╘︎╒︎╓︎╫︎╪︎┘︎┌︎█︎▄︎▌︎▐︎▀︎α︎ß︎Γ︎π︎Σ︎σ︎µ︎τ︎Φ︎Θ︎Ω︎δ︎∞︎φ︎ε︎∩︎≡︎±︎≥︎≤︎⌠︎⌡︎÷︎≈︎°︎∙︎·︎√︎ⁿ︎²︎■︎□︎";
        private Shader shader;
        int vbo, vao;
        public float size = 1;
        public void SetText(string s)
        {
            text = s;
            float[] vertices = new float[s.Length * 30];
            for(int i = 0; i < text.Length*30; i+=30)
            {
                Atlas.TexRect rect = atlas.GetCell((int)text[i / 30]);
                float offset = (((float)i) / 30) / 34;

                //tri 1

                vertices[i] = position.X+(offset * size);
                vertices[i + 1] = position.Y;
                vertices[i + 2] = 0;
                vertices[i + 3] = rect.topLeft.X;
                vertices[i + 4] = rect.topLeft.Y;

                vertices[i + 5] = position.X+(offset * size);
                vertices[i + 6] = position.Y-(0.05f * size);
                vertices[i + 7] = 0;
                vertices[i + 8] = rect.bottomLeft.X;
                vertices[i + 9] = rect.bottomLeft.Y;

                vertices[i + 10] = position.X + (offset * size)+ (0.05f * size);
                vertices[i + 11] = position.Y - (0.05f * size);
                vertices[i + 12] = 0;
                vertices[i + 13] = rect.bottomRight.X;
                vertices[i + 14] = rect.bottomRight.Y;

                //tri 2

                vertices[i + 15] = position.X + (offset * size) + (0.05f * size);
                vertices[i + 16] = position.Y - (0.05f * size);
                vertices[i + 17] = 0;
                vertices[i + 18] = rect.bottomRight.X;
                vertices[i + 19] = rect.bottomRight.Y;

                vertices[i + 20] = position.X + (offset * size) + (0.05f * size);
                vertices[i + 21] = position.Y;
                vertices[i + 22] = 0;
                vertices[i + 23] = rect.topRight.X;
                vertices[i + 24] = rect.topRight.Y;

                vertices[i + 25] = position.X + (offset * size);
                vertices[i + 26] = position.Y;
                vertices[i + 27] = 0;
                vertices[i + 28] = rect.topLeft.X;
                vertices[i + 29] = rect.topLeft.Y;
            }

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
            atlas.texture.Use(TextureUnit.Texture0);
        }
        public UIText()
        {
            texts.Add(this);
            shader = new Shader("Shaders/text.vert", "Shaders/text.frag");
        }
        public void Destroy()
        {
            texts.Remove(this);
        }
        public void Draw()
        {
            atlas.texture.Use(TextureUnit.Texture0);

            if (Camera.vr)
                shader.SetFloat("sx", Window.ScaleX / 2);
            else
                shader.SetFloat("sx", Window.ScaleX / 2);
            shader.SetFloat("sy", Window.ScaleY/2);

            shader.SetVector3("color", new Vector3(color.R / 1, color.G / 1, color.B / 1));
            shader.SetFloat("alpha", color.A);

            shader.Use();
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, text.Length*6);
        }
    }
}
