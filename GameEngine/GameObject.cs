using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics;
using System.Drawing;
using DocumentFormat.OpenXml.Drawing;
using VainEngine.Physics;

namespace VainEngine
{
    
    public class GameObject
    {
        public static List<GameObject> objects = new List<GameObject>();
        public Vector3 position = Vector3.Zero, eulerRot = Vector3.Zero;
        public float scale = 1;
        public string tag;
        private int index;
        public Mesh viewMesh;
        public Collider collider;
        public bool dontDestroyOnSceneLoad = false;
        public bool visible = true;

        public GameObject()
        {
            tag = "";
            position = Vector3.Zero;
            objects.Add(this);
            index = objects.IndexOf(this);
        }
        public GameObject(string tag)
        {
            this.tag = tag;
            position = Vector3.Zero;
            objects.Add(this);
            index = objects.IndexOf(this);
        }
        public static GameObject FindWithTag(string tag)
        {
            foreach(GameObject g in objects)
            {
                if(g.tag == tag)
                {
                    return g;
                }
            }
            return null;
        }
        public void Destroy()
        {
            objects.Remove(this);
        }
        public static class Primitives
        {
            private static Texture primitiveTexture = Texture.LoadFromFile(@"Resources\grid.png");
            public static GameObject Plane()
            {
                GameObject g = new GameObject();
                g.viewMesh = new Mesh(@"Resources\plane.obj", primitiveTexture, 1);
                return g;
            }
            public static GameObject Box()
            {
                GameObject g = new GameObject();
                g.viewMesh = new Mesh(@"Resources\box.obj", primitiveTexture, 1);
                return g;
            }
            public static GameObject Plane(float s)
            {
                GameObject g = new GameObject();
                g.viewMesh = new Mesh(@"Resources\plane.obj", primitiveTexture, s);
                return g;
            }
            public static GameObject Box(float s)
            {
                GameObject g = new GameObject();
                g.viewMesh = new Mesh(@"Resources\box.obj", primitiveTexture, s);
                return g;
            }
        }
    }
    //public class UIText
    //{
    //    public static List<UIText> texts = new List<UIText>();
    //    public Vector2 position;
    //    public string text;
    //    private int index;
    //    private static readonly Atlas atlas = new Atlas(512, 512, 16, 16, Texture.LoadFromFile(@"C:\Users\dbasp\source\repos\GameEngine\GameEngine\Resources\TextAtlas.png"));
    //    private static string CharSheet = " ︎☺︎☻︎♥︎♦︎♣︎♠︎•︎◘︎○︎◙︎♂︎♀︎♪︎♫︎☼︎►︎◄︎↕︎‼︎¶︎§︎▬︎↨︎↑︎↓︎→︎←︎∟︎↔︎▲︎▼︎ ︎!︎\"︎#︎$︎%︎&︎'︎(︎)︎*︎+︎,︎-︎.︎/︎0︎1︎2︎3︎4︎5︎6︎7︎8︎9︎:︎;︎<︎=︎>︎?︎@︎A︎B︎C︎D︎E︎F︎G︎H︎I︎J︎K︎L︎M︎N︎O︎P︎Q︎R︎S︎T︎U︎V︎W︎X︎Y︎Z︎[︎\\︎]︎^︎_︎`︎a︎b︎c︎d︎e︎f︎g︎h︎i︎j︎k︎l︎m︎n︎o︎p︎q︎r︎s︎t︎u︎v︎w︎x︎y︎z︎{︎¦︎}︎~︎⌂︎Ç︎ü︎é︎â︎ä︎à︎å︎ç︎ê︎ë︎è︎ï︎î︎ì︎Ä︎Å︎É︎æ︎Æ︎ô︎ö︎ò︎û︎ù︎ÿ︎Ö︎Ü︎¢︎£︎¥︎₧︎ƒ︎á︎í︎ó︎ú︎ñ︎Ñ︎ª︎º︎¿︎⌐︎¬︎½︎¼︎¡︎«︎»︎░︎▒︎▓︎│︎┤︎╡︎╢︎╖︎╕︎╣︎║︎╗︎╝︎╜︎╛︎┐︎└︎┴︎┬︎├︎─︎┼︎╞︎╟︎╚︎╔︎╩︎╦︎╠︎═︎╬︎╧︎╨︎╤︎╥︎╙︎╘︎╒︎╓︎╫︎╪︎┘︎┌︎█︎▄︎▌︎▐︎▀︎α︎ß︎Γ︎π︎Σ︎σ︎µ︎τ︎Φ︎Θ︎Ω︎δ︎∞︎φ︎ε︎∩︎≡︎±︎≥︎≤︎⌠︎⌡︎÷︎≈︎°︎∙︎·︎√︎ⁿ︎²︎■︎□︎";
    //    private Shader shader;
    //    int VAO, VBO;
    //    public UIText()
    //    {
    //        position = Vector2.Zero;
    //        texts.Add(this);
    //        index = texts.IndexOf(this);
    //        
    //        VAO = GL.GenVertexArray();
    //        GL.BindVertexArray(VAO);
    //        VBO = GL.GenBuffer();
    //        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
    //        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4,new float[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, BufferUsageHint.DynamicDraw);
    //
    //        shader = new Shader("Shaders/text.vert", "Shaders/text.frag");
    //
    //        var vertexLocation = shader.GetAttribLocation("aPosition");
    //        GL.EnableVertexAttribArray(vertexLocation);
    //        GL.VertexAttribPointer(vertexLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
    //
    //        var texCoordLocation = shader.GetAttribLocation("aTexCoord");
    //        GL.EnableVertexAttribArray(texCoordLocation);
    //        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
    //
    //        atlas.texture.Use(TextureUnit.Texture0);
    //
    //        shader.SetInt("texture0", 0);
    //    }
    //    public void Destroy()
    //    {
    //        texts.Remove(this);
    //    }
    //    public void Draw()
    //    {
    //        char[] data = text.ToCharArray();
    //        GL.BindVertexArray(VAO);
    //
    //            atlas.texture.Use(TextureUnit.Texture0);
    //        for(int i = 0; i < data.Length; i++)
    //        {
    //            
    //            int cellID = CharSheet.IndexOf(data[i]);
    //            Atlas.TexRect rect= atlas.GetCell(cellID);
    //            Vector2 _1, _2, _3, _4;
    //            _1 = new Vector2(position.X + (((i * atlas.cellWidth) + rect.topLeft.X) / Window.ScaleX), position.Y + ((rect.topLeft.Y) / Window.ScaleY));
    //            _2 = new Vector2(position.X + (((i * atlas.cellWidth) + rect.topRight.X) / Window.ScaleX), position.Y + ((rect.topRight.Y) / Window.ScaleY));
    //            _3 = new Vector2(position.X + (((i * atlas.cellWidth) + rect.bottomLeft.X) / Window.ScaleX), position.Y + ((rect.bottomLeft.Y) / Window.ScaleY));
    //            _4 = new Vector2(position.X + (((i * atlas.cellWidth) + rect.bottomRight.X) / Window.ScaleX), position.Y + ((rect.bottomRight.Y) / Window.ScaleY));
    //            float[] vertices = new float[24]
    //            {
    //                 _1.X,_1.Y,rect.topLeft.X,rect.topLeft.Y,
    //                 _2.X,_2.Y,rect.topRight.X,rect.topRight.Y,
    //                 _3.X,_3.Y,rect.bottomLeft.X,rect.bottomLeft.Y,
    //
    //                 _2.X,_2.Y,rect.topRight.X,rect.topRight.Y,
    //                 _4.X,_4.Y,rect.bottomRight.X,rect.bottomRight.Y,
    //                 _3.X,_3.Y,rect.bottomLeft.X,rect.bottomLeft.Y
    //            };
    //            
    //            // render glyph texture over quad
    //            GL.BindTexture(TextureTarget.Texture2D, 0);
    //            // update content of VBO memory
    //            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
    //            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float)*24, vertices);
    //            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    //            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
    //        }
    //    }
    //}
    //
    public class UIText
    {
        public static List<UIText> texts = new List<UIText>();
        public Vector2 position;
        public string text;
        private static readonly Atlas atlas = new Atlas(512, 512, 16, 16, Texture.LoadFromFile(@"Resources\TextAtlas.png"));
        private static string CharSheet = " ︎☺︎☻︎♥︎♦︎♣︎♠︎•︎◘︎○︎◙︎♂︎♀︎♪︎♫︎☼︎►︎◄︎↕︎‼︎¶︎§︎▬︎↨︎↑︎↓︎→︎←︎∟︎↔︎▲︎▼︎ ︎!︎\"︎#︎$︎%︎&︎'︎(︎)︎*︎+︎,︎-︎.︎/︎0︎1︎2︎3︎4︎5︎6︎7︎8︎9︎:︎;︎<︎=︎>︎?︎@︎A︎B︎C︎D︎E︎F︎G︎H︎I︎J︎K︎L︎M︎N︎O︎P︎Q︎R︎S︎T︎U︎V︎W︎X︎Y︎Z︎[︎\\︎]︎^︎_︎`︎a︎b︎c︎d︎e︎f︎g︎h︎i︎j︎k︎l︎m︎n︎o︎p︎q︎r︎s︎t︎u︎v︎w︎x︎y︎z︎{︎¦︎}︎~︎⌂︎Ç︎ü︎é︎â︎ä︎à︎å︎ç︎ê︎ë︎è︎ï︎î︎ì︎Ä︎Å︎É︎æ︎Æ︎ô︎ö︎ò︎û︎ù︎ÿ︎Ö︎Ü︎¢︎£︎¥︎₧︎ƒ︎á︎í︎ó︎ú︎ñ︎Ñ︎ª︎º︎¿︎⌐︎¬︎½︎¼︎¡︎«︎»︎░︎▒︎▓︎│︎┤︎╡︎╢︎╖︎╕︎╣︎║︎╗︎╝︎╜︎╛︎┐︎└︎┴︎┬︎├︎─︎┼︎╞︎╟︎╚︎╔︎╩︎╦︎╠︎═︎╬︎╧︎╨︎╤︎╥︎╙︎╘︎╒︎╓︎╫︎╪︎┘︎┌︎█︎▄︎▌︎▐︎▀︎α︎ß︎Γ︎π︎Σ︎σ︎µ︎τ︎Φ︎Θ︎Ω︎δ︎∞︎φ︎ε︎∩︎≡︎±︎≥︎≤︎⌠︎⌡︎÷︎≈︎°︎∙︎·︎√︎ⁿ︎²︎■︎□︎";
        private Shader shader;
        public UIText()
        {
            texts.Add(this);
            shader = new Shader("Shaders/text.vert", "Shaders/text.frag");
        }
        public void Draw()
        {
            
            //for (int i = 0; i < text.Length; i++)
            //{
            //    Atlas.TexRect t = atlas.GetCell(CharSheet.IndexOf(text[i]));
            //    int _vao = GL.GenVertexArray();
            //    GL.BindVertexArray(_vao);
            //    float[] v = new float[]
            //    {
            //        position.X + (i / 25), position.Y, 0, t.topRight.X, t.topRight.Y, 
            //        position.X + (i / 25), position.Y - 0.04f, 0, t.bottomRight.X, t.bottomRight.Y, 
            //        position.X + (i / 25) - 0.04f, position.Y - 0.04f, 0, t.bottomLeft.X, t.bottomLeft.X,
            //        position.X + (i / 25) - 0.04f, position.Y, 0, t.topLeft.X, t.topLeft.Y
            //    };
            //    int _vbo = GL.GenBuffer();
            //    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            //    GL.BufferData(BufferTarget.ArrayBuffer, v.Length * sizeof(float), v, BufferUsageHint.StaticDraw);
            //
            //    Shader tempShader = shader;
            //    var vertexLocation = tempShader.GetAttribLocation("aPosition");
            //    GL.EnableVertexAttribArray(vertexLocation);
            //    GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            //
            //    var texCoordLocation = tempShader.GetAttribLocation("aTexCoord");
            //
            //    GL.EnableVertexAttribArray(texCoordLocation);
            //    GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            //
            //    atlas.texture.Use(TextureUnit.Texture0);
            //
            //    tempShader.SetInt("texture0", 0);
            //
            //    GL.BindVertexArray(_vao);
            //
            //    atlas.texture.Use(TextureUnit.Texture0);
            //
            //    GL.DrawElements(PrimitiveType.Quads, 4, DrawElementsType.UnsignedInt, 0);
            //}
        }
    }
}
