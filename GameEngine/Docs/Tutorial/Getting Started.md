# Getting Started

To start, put a using block at the top of your main code file (Program.cs in my case).
```
using VainEngine;
```
then, in the entry point method, where you want the game to start, put the following lines:
```
//Create the window. Syntax is width, height, then window title
var w = VainEngine.Window.Create(1000, 720,  "[name of your game or project]");

//Setup with a new of your main class (must not be static!!!), [Program] in my case
w.Setup(new Program());

//Start the game
w.Run();
```
A window for the game should appear.