using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using ImGuiNET;
using System.Diagnostics;
using Jitter;
using Jitter.Collision;
using System.Drawing;
using System.ComponentModel;

namespace VainEngine
{
    public static class Global
    {
        public static Color4 ambient { get; private set; }
        public static void SkyColor(Color4 c)
        {
            GL.ClearColor(c);
            ambient = new Color4(c.R*2f, c.G * 2f, c.B * 2f, 1);
        }
        public static double totalTime;
    }
    public struct Eye
    {
        public Matrix4 ProjectionMatrix;
        public Matrix4 ViewMatrix;
        public int HiddenAreaMeshVAO;
        public int HiddenAreaMeshNumElements;
        public int FrameBufferHandle;
        public int FrameBufferColorHandle;
    }
    public class Window : GameWindow
    {
        public Bitmap GrabScreenshot()
        {
            Bitmap bmp = new Bitmap((int)ScaleX, (int)ScaleY);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(new Rectangle(0, 0, (int)ScaleX, (int)ScaleY), System.Drawing.Imaging.ImageLockMode.WriteOnly,
                             System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, (int)ScaleX, (int)ScaleY, PixelFormat.Bgr, PixelType.UnsignedByte,
                     data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

        public static CollisionSystem collision = new CollisionSystemSAP();
        public static World world;
        public static float timeScale = 0.8f;
        public const float FogStart = 80, FogEnd = 100;
        public static Vector3 sunPos = new Vector3(1f, 1f, 1.0f);
        Mesh _mesh;
        private double _time;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }
        #region constructors
        public static Window Create()
        {
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "VainEngine",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
                NumberOfSamples = 4
            };
            return new Window(GameWindowSettings.Default, nativeWindowSettings);
        }
        public static Window Create(int x, int y)
        {
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(x, y),
                Title = "VainEngine",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
                NumberOfSamples = 4
            };
            return new Window(GameWindowSettings.Default, nativeWindowSettings);
        }
        public static Window Create(int x, int y, string title)
        {
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(x, y),
                Title = title,
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
                NumberOfSamples = 4
            };
            return new Window(GameWindowSettings.Default, nativeWindowSettings);
        }
        #endregion
        public void GoFullscreen()
        {
            WindowState = WindowState.Fullscreen;
        }
        public void GoWindowed()
        {
            WindowState = WindowState.Normal;
        }
        public void GrabMouse()
        {
            CursorVisible = false;
            CursorGrabbed = true;
        }
        public void ReleaseMouse()
        {
            CursorVisible = true;
            CursorGrabbed = false;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            for (int i = 0; i < GameObject.objects.Count; i++)
            {
                for (int i1 = 0; i1 < GameObject.objects[i].components.Count; i1++)
                {
                    Component c = GameObject.objects[i].components[i1];
                    c.End();
                }
            }
        }
        protected override void OnLoad()
        {
            titleText = Title;
            //Window.c
            world = new World(collision);
            world.Gravity = new Jitter.LinearMath.JVector(0, -150f, 0);
            Mesh.Meshes.Add(new Mesh(@"Resources\sphere.obj", Texture.LoadFromFile(@"Resources\grid.png")));
            gameLogic.mouse = MouseState;
            gameLogic.keyboard = KeyboardState;
            gameLogic.Size = Size;
            //call update
            gameLogic.Start();
            gameLogic.STARTED = true;
            base.OnLoad();
            GL.Enable(EnableCap.DepthTest);

           
            Camera.view = Matrix4.CreateTranslation(1.0f, -1f, -3.0f);

            Camera.projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75f), Size.X / (float)Size.Y, 0.02f, 500.0f);

            Camera.pos = Vector3.Zero;
            Camera.euler = Vector3.Zero;
            Camera.fov = 75;
            

            collision.EnableSpeculativeContacts = true;
            CursorVisible = false;
            CursorGrabbed = true;
            //WindowState = WindowState.Fullscreen;
        }
        float mouseX, mouseY;
        public static Vector2 size;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //int bufferFBO = GL.GenFramebuffer();
            //GL.BindFramebuffer(FramebufferTarget.Framebuffer,bufferFBO);
            //int buffer = GL.GenTexture();
            //GL.BindTexture(TextureTarget.Texture2D,buffer);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, Size.X, Size.Y, 0,PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);



            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            //GL.CullFace(CullFaceMode.Back);
            //GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //if (KeyboardState.IsKeyDown(Keys.F) && !KeyboardState.WasKeyDown(Keys.F))
            //{
            //    if (WindowState == WindowState.Fullscreen)
            //        WindowState = WindowState.Normal;
            //    else if (WindowState == WindowState.Normal)
            //        WindowState = WindowState.Fullscreen;
            //}
            base.OnRenderFrame(e);

            // We add the time elapsed since last frame, times 4.0 to speed up animation, to the total amount of time passed.
            _time += e.Time;

            // We clear the depth buffer in addition to the color buffer.
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            if (Camera.vr)
            {
                Camera.projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.fov*1.04f), (Size.X) / (float)Size.Y, Camera.nearClip, Camera.farClip);
                Vector3 cross = Vector3.Cross(Camera.front, Camera.up) * 0.18f;
                Camera.view = Matrix4.LookAt(new Vector3(Camera.pos.X, -Camera.pos.Y, Camera.pos.Z) + (cross * -0.1f), new Vector3(Camera.pos.X, -Camera.pos.Y, Camera.pos.Z) + Camera.front + (cross * -0.1f), Camera.up);

                GL.Viewport(0, 0, Size.X / 2, Size.Y);
                GL.Enable(EnableCap.DepthTest);
                RenderScene();
                GL.Disable(EnableCap.DepthTest);
                RenderUI();

                Camera.view = Matrix4.LookAt(new Vector3(Camera.pos.X, -Camera.pos.Y, Camera.pos.Z) + (cross * 0.1f), new Vector3(Camera.pos.X, -Camera.pos.Y, Camera.pos.Z) + Camera.front + (cross * 0.1f), Camera.up);

                GL.Viewport(Size.X / 2, 0, Size.X / 2, Size.Y);
                GL.Enable(EnableCap.DepthTest);
                RenderScene();
                GL.Disable(EnableCap.DepthTest);
                RenderUI();
            }
            else
            {
                Camera.view = Matrix4.LookAt(new Vector3(Camera.pos.X, -Camera.pos.Y, Camera.pos.Z), new Vector3(Camera.pos.X, -Camera.pos.Y, Camera.pos.Z) + Camera.front, Camera.up);

                GL.Viewport(0, 0, Size.X, Size.Y);
                GL.Enable(EnableCap.DepthTest);
                RenderScene();
                GL.Disable(EnableCap.DepthTest);
                RenderUI();
            }
            Graphics.lineQueue.Clear();
            SwapBuffers();

        }
        Vector3 front;
        string titleText;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Global.totalTime = _time;
            fTime = (float)e.Time;
            size = new Vector2(ScaleX, ScaleY);
            ScaleX = Size.X;
            ScaleY = Size.Y;
            world.Step((float)e.Time * (timeScale/4), true);
            
            gameLogic.time = (float)_time;
            gameLogic.frameTime = (float)e.Time * timeScale;
            gameLogic.mouse = MouseState;
            gameLogic.keyboard = KeyboardState;
            gameLogic.Size = Size;

            for (int i = 0; i < GameObject.objects.Count; i++)
            {
                foreach(var c in GameObject.objects[i].components)
                {
                    if (c._INIT)
                        c.Start();
                    c.Update();
                }
            }
            if (KeyboardState.IsKeyPressed(Keys.B))
                Debugger.Break();
            //call update
            if (gameLogic.STARTED)
                gameLogic.Update();
            //post-update processing
            front.X = (float)Math.Cos(MathHelper.DegreesToRadians(Camera.euler.X)) * (float)Math.Cos(MathHelper.DegreesToRadians(-Camera.euler.Y));
            front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(Camera.euler.X));
            front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(Camera.euler.X)) * (float)Math.Sin(MathHelper.DegreesToRadians(-Camera.euler.Y));
            Camera.front = front;
            Camera.view = Matrix4.LookAt(new Vector3(Camera.pos.X, -Camera.pos.Y, Camera.pos.Z), new Vector3(Camera.pos.X, -Camera.pos.Y, Camera.pos.Z) + Camera.front, Camera.up);
            if (Camera.orthographic)
            {
                Camera.projection = Matrix4.CreateOrthographic(Camera.orthographicScale * (Size.X / (float)Size.Y), Camera.orthographicScale, Camera.orthoNearClip, Camera.orthoFarClip);
            }
            else
            {
                Camera.projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.fov), Size.X / (float)Size.Y, Camera.nearClip, Camera.farClip);
            }
        }
        public static bool cursorLock;
        public static float ScaleX = 10, ScaleY = 10;
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        void RenderScene()
        {
            for (int i = 0; i < GameObject.objects.Count; i++)
            {
                if (GameObject.objects[i].viewMesh != null && GameObject.objects[i].visible)
                {
                    GameObject.objects[i].viewMesh.position = GameObject.objects[i].position;
                    GameObject.objects[i].viewMesh.scale = GameObject.objects[i].scale;
                    GameObject.objects[i].viewMesh.euler = GameObject.objects[i].eulerRot;
                    GameObject.objects[i].viewMesh.rotMatrix = GameObject.objects[i].rotation;
                    GameObject.objects[i].viewMesh.useRotMatrix = GameObject.objects[i].useMatrixInsteadOfEuler;
                    GameObject.objects[i].viewMesh.Draw();
                }
            }
            Shader shader = new Shader("Shaders/line.vert", "Shaders/line.frag");
            GL.Disable(EnableCap.CullFace);
            foreach(Vector3[] lineset in Graphics.lineQueue)
            {
                
                var cross1 = Vector3.Cross(Vector3.UnitY, (lineset[1] - lineset[2]).Normalized()).Normalized();
                var cross2 = Vector3.Cross(Vector3.UnitY, (lineset[1] - lineset[2]).Normalized()).Normalized();
                var a = Vector3.Cross(Vector3.UnitY, cross1).Normalized();
                var b = Vector3.Cross(Vector3.UnitY, cross2).Normalized();
                var cross11 = Vector3.Cross((lineset[1] - lineset[2]).Normalized(), a).Normalized();
                var cross21 = Vector3.Cross((lineset[1] - lineset[2]).Normalized(), b).Normalized();

                var v0 = lineset[1] + (cross1 * 0.005f);
                var v1 = lineset[1] - (cross1 * 0.005f);
                var v01 = lineset[2] + (cross2 * 0.005f);
                var v11 = lineset[2] - (cross2 * 0.005f);

                var v001 = lineset[1] + (cross11 * 0.005f);
                var v101 = lineset[1] - (cross11 * 0.005f);
                var v011 = lineset[2] + (cross21 * 0.005f);
                var v111 = lineset[2] - (cross21 * 0.005f);


                float[] vertices = new float[] { 
                    v0.X, v0.Y, v0.Z,
                    v1.X, v1.Y, v1.Z,
                    v01.X, v01.Y, v01.Z,
                    v1.X, v1.Y, v1.Z,
                    v11.X, v11.Y, v11.Z,
                    v01.X, v01.Y, v01.Z,

                    v001.X, v001.Y, v001.Z,
                    v101.X, v101.Y, v101.Z,
                    v011.X, v011.Y, v011.Z,
                    v101.X, v101.Y, v101.Z,
                    v111.X, v111.Y, v111.Z,
                    v011.X, v011.Y, v011.Z,
                };

                int vao, vbo;

                vao = GL.GenVertexArray();
                GL.BindVertexArray(vao);

                vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                var vertexLocation = shader.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

                shader.SetMatrix4("view", Camera.view);
                shader.SetMatrix4("projection", Camera.projection);
                shader.SetVector3("color", lineset[0]);

                shader.Use();
                GL.BindVertexArray(vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
            
        }
        public static float fTime;
        void RenderUI()
        {
            for (int i = 0; i < UIBox.boxes.Count; i++)
            {
                UIBox.boxes[i].Draw();
            }
            for (int i = 0; i < UIText.texts.Count; i++)
            {
                UIText.texts[i].Draw();
            }
        }
        //protected override void OnUnload()
        //{
        //    // Unbind all the resources by binding the targets to 0/null.
        //    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        //    GL.BindVertexArray(0);
        //    GL.UseProgram(0);
        //
        //    // Delete all the resources.
        //    GL.DeleteBuffer(_vertexBufferObject);
        //    GL.DeleteVertexArray(_vertexArrayObject);
        //
        //    GL.DeleteProgram(_shader.Handle);
        //
        //    base.OnUnload();
        //}
        public void SetCamPos(float x, float y, float z)
        {
            Camera.view = Matrix4.CreateTranslation(x, -y, -z);
        }
        private GameLogic gameLogic;
        public void Setup(GameLogic instance)
        {
            gameLogic = instance;
        }

    }
    public class GameLogic
    {
        public bool STARTED = false;
        public Vector2i Size;
        public MouseState mouse;
        public KeyboardState keyboard;
        public float time, frameTime;
        public virtual void Update() { }
        public virtual void Start() { }
    }
}
