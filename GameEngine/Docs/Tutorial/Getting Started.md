# Getting Started

To start, put a using block at the top of your main code file (Program.cs in my case).
```C#
using VainEngine;
```
then, make your main class inherit from GameLogic. this enables you to write code that runs in the engine.
example:
```C#
class Program : GameLogic
{
    //class code here...
}
```
finally, in the entry point method, where you want the game to start, put the following lines:
```C#
//Create the window. Syntax is width, height, then window title
Window w = Window.Create(1000, 720,  "[name of your game or project]");

//Setup with a new of your main class (must not be static!!!), [Program] in my case
w.Setup(new Program());

//Start the game
w.Run();
```
A window for the game should appear.

## Camera
To display things in VainEngine, you must configure the camera. VainEngine is a 3D engine, but you can emulate 2D by setting the camera to orthographic mode and using rotated planes as sprites.(you COULD use the UIBox class, but it is slightly more expensive and you have no control over the draw order.)

The camera uses a "position, euler, up" system. First the position is set, then the euler angles are converted to a direction vector reperesenting the camera's forward direction. then it uses the up direction combined with the forward direction to set the orientation.

__IMPORTANT: VainEngine uses a vertically flipped coordinate system. This means that (1,2,3) in Unity, would be (1,-2,3) in VainEngine. this is mainly because early in development i screwed up some stuff and forgot to add proper coordinate systems, so this means that if you also want to use a physics library, you could flip the y-coordinate when setting a meshe's position or just set gravity to negative.__

To make the camera look forward from (0,0,0), just type the following in any *game logic method\**:
```C#
Camera.up = new OpenTK.Mathematics.Vector3(0,-1,0);
Camera.pos = new OpenTK.Mathematics.Vector3(0,0,0);
Camera.euler = new OpenTK.Mathematics.Vector3(0,0,0);
```
*\*game logic method means any method who's execution is dictated by VainEngine, and not your code. for example, the Update() method in the main program(discussed later)

## Ceating GameObjects
Similar to Unity, VainEngine uses a GameObject-Component system.
Creating GameObjects is easy.
```C#
GameObject g = new GameObject();
```
or
```C#
GameObject g = new GameObject("[optional string as tag]");
```
This creates a blank GameObject. To see it, assign a mesh. To assign a mesh, you must have a folder in your project called "Resources", containing a .obj file you wish to use. this folder, when built will automatically contain fundamentals, like primitives, and a blank white texture. items in this folder must have "Copy Always" set.

To use a mesh and assign it, type:
```C#
Mesh m = new Mesh(@"Resources\yourObj.obj", Texture.LoadFromFile(@"Resources\yourTexture.whatever"))
g.viewMesh = m;
```
if you are creating many meshes on the fly, such as shaped particles, you may want to cache the mesh and texture using a global object and a list or something.
