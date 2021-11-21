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
using ImGuiNET;

namespace VainEngine
{
    public class Window : GameWindow
    {
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
        protected override void OnLoad()
        {
            titleText = Title;
            //Window.c
            world = new World(collision);
            world.Gravity = new Jitter.LinearMath.JVector(0, -150f, 0);
            Mesh.Meshes.Add(new Mesh(@"Resources\sphere.obj", Texture.LoadFromFile(@"Resources\shootgun.png")));
            gameLogic.mouse = MouseState;
            gameLogic.keyboard = KeyboardState;
            gameLogic.Size = Size;
            //call update
            gameLogic.Start();
            gameLogic.STARTED = true;
            base.OnLoad();
            GL.ClearColor(0.56f, 0.7f, 1.0f, 1.0f);

            // We enable depth testing here. If you try to draw something more complex than one plane without this,
            // you'll notice that polygons further in the background will occasionally be drawn over the top of the ones in the foreground.
            // Obviously, we don't want this, so we enable depth testing. We also clear the depth buffer in GL.Clear over in OnRenderFrame.
            GL.Enable(EnableCap.DepthTest);

            
            // For the view, we don't do too much here. Next tutorial will be all about a Camera class that will make it much easier to manipulate the view.
            // For now, we move it backwards three units on the Z axis.
            Camera.view = Matrix4.CreateTranslation(1.0f, -1f, -3.0f);

            // For the matrix, we use a few parameters.
            //   Field of view. This determines how much the viewport can see at once. 45 is considered the most "realistic" setting, but most video games nowadays use 90
            //   Aspect ratio. This should be set to Width / Height.
            //   Near-clipping. Any vertices closer to the camera than this value will be clipped.
            //   Far-clipping. Any vertices farther away from the camera than this value will be clipped.
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
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
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

            for(int i = 0; i < GameObject.objects.Count; i++)
            {
                if (GameObject.objects[i].viewMesh != null && GameObject.objects[i].visible)
                {
                    GameObject.objects[i].viewMesh.position = GameObject.objects[i].position;
                    GameObject.objects[i].viewMesh.scale = GameObject.objects[i].scale;
                    GameObject.objects[i].viewMesh.euler = GameObject.objects[i].eulerRot;
                    GameObject.objects[i].viewMesh.Draw();
                }
            }
            for (int i = 0; i < UIText.texts.Count; i++)
            {
                UIText.texts[i].Draw();
            }

            SwapBuffers();
        }
        Vector3 front;
        string titleText;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
           
            Title = titleText + $" ({(1 / e.Time).ToString()} fps)";
            ScaleX = Size.X;
            ScaleY = Size.Y;
            world.Step((float)e.Time * (timeScale/4), true);
            
            gameLogic.time = (float)_time;
            gameLogic.frameTime = (float)e.Time * timeScale;
            gameLogic.mouse = MouseState;
            gameLogic.keyboard = KeyboardState;
            gameLogic.Size = Size;
            //call update
            if(gameLogic.STARTED)
                gameLogic.Update();
            //post-update processing
            front.X = (float)Math.Cos(MathHelper.DegreesToRadians(Camera.euler.X)) * (float)Math.Cos(MathHelper.DegreesToRadians(-Camera.euler.Y));
            front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(Camera.euler.X));
            front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(Camera.euler.X)) * (float)Math.Sin(MathHelper.DegreesToRadians(-Camera.euler.Y));
            Camera.front = front;
            Camera.view = Matrix4.LookAt(new Vector3(Camera.pos.X, -Camera.pos.Y, Camera.pos.Z), new Vector3(Camera.pos.X, -Camera.pos.Y, Camera.pos.Z) + front, Camera.up);
            Camera.projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.fov), Size.X / (float)Size.Y, 0.02f, 500.0f);
        }
        public static bool cursorLock;
        public static float ScaleX = 10, ScaleY = 10;
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            
            GL.Viewport(0, 0, Size.X, Size.Y);
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
