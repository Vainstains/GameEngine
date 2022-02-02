using System;
using System.Collections.Generic;
using VainEngine;
using Newtonsoft.Json;
using System.IO;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GridBeatz
{
    class Program : GameLogic
    {
        public static string levelPath;
        public static VainEngine.Window w;
        static void Main(string[] args)
        {
            //Console.WriteLine("Please drag in any beat saber 'Info.dat'");

            //levelPath = Console.ReadLine().Trim('"');
            w = VainEngine.Window.Create(1250, 800, "Grid Beatz");
            w.Setup(new Program());
            
            w.Run();

        }
        public override void Start()
        {
            base.Start();
            w.ReleaseMouse();
            Global.SkyColor(new OpenTK.Mathematics.Color4(40, 40, 45, 255));
            GameObject plane = GameObject.Primitives.Plane(0);
            Camera.up = -OpenTK.Mathematics.Vector3.UnitY;
            
            BeatMapPlayer.PlayBeatmap(BeatMap(@"C:\Users\dbasp\OneDrive\Desktop\YeetSaberLevelPlaylist\1fc8f (THE MUZZLE FACING - Timbo)\EasyStandard.dat"), 213, @"C:\Users\dbasp\OneDrive\Desktop\YeetSaberLevelPlaylist\1fc8f (THE MUZZLE FACING - Timbo)\FurryUWU.egg");

        }
        public override void Update()
        {
            base.Update();
            Graphics.DrawLine(Camera.pos + Camera.front, OpenTK.Mathematics.Vector3.Zero, OpenTK.Mathematics.Color4.Red);
        }

        public static BeatInfoData.Root MapInfo(string path)
        {
            string data = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<BeatInfoData.Root>(data);
        }
        public static BeatMapData.Root BeatMap(string path)
        {
            string data = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<BeatMapData.Root>(data);
        }
    }

    namespace BeatMapData
    {
        public class Bookmark
        {
            public double _time { get; set; }
            public string _name { get; set; }
        }

        public class CustomData
        {
            public int _time { get; set; }
            public List<object> _BPMChanges { get; set; }
            public List<Bookmark> _bookmarks { get; set; }
        }

        public class Event
        {
            public double _time { get; set; }
            public int _type { get; set; }
            public int _value { get; set; }
        }

        public class Note
        {
            public double _time { get; set; }
            public int _lineIndex { get; set; }
            public int _lineLayer { get; set; }
            public int _type { get; set; }
            public int _cutDirection { get; set; }
        }

        public class Obstacle
        {
            public double _time { get; set; }
            public int _lineIndex { get; set; }
            public int _type { get; set; }
            public double _duration { get; set; }
            public int _width { get; set; }
        }

        public class Root
        {
            public string _version { get; set; }
            public CustomData _customData { get; set; }
            public List<Event> _events { get; set; }
            public List<Note> _notes { get; set; }
            public List<Obstacle> _obstacles { get; set; }
        }
    }
    namespace BeatInfoData
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Contributor
        {
            public string _role { get; set; }
            public string _name { get; set; }
            public string _iconPath { get; set; }
        }

        public class MMA2
        {
            public string version { get; set; }
        }

        public class ChroMapper
        {
            public string version { get; set; }
        }

        public class Editors
        {
            public string _lastEditedBy { get; set; }
            public MMA2 MMA2 { get; set; }
            public ChroMapper ChroMapper { get; set; }
        }

        public class CustomData
        {
            public List<Contributor> _contributors { get; set; }
            public Editors _editors { get; set; }
            public string _difficultyLabel { get; set; }
            public int _editorOffset { get; set; }
            public int _editorOldOffset { get; set; }
            public List<object> _warnings { get; set; }
            public List<object> _information { get; set; }
            public List<object> _suggestions { get; set; }
            public List<object> _requirements { get; set; }
        }

        public class DifficultyBeatmap
        {
            public string _difficulty { get; set; }
            public int _difficultyRank { get; set; }
            public string _beatmapFilename { get; set; }
            public double _noteJumpMovementSpeed { get; set; }
            public double _noteJumpStartBeatOffset { get; set; }
            public CustomData _customData { get; set; }
        }

        public class DifficultyBeatmapSet
        {
            public string _beatmapCharacteristicName { get; set; }
            public List<DifficultyBeatmap> _difficultyBeatmaps { get; set; }
        }

        public class Root
        {
            public string _version { get; set; }
            public string _songName { get; set; }
            public string _songSubName { get; set; }
            public string _songAuthorName { get; set; }
            public string _levelAuthorName { get; set; }
            public int _beatsPerMinute { get; set; }
            public int _shuffle { get; set; }
            public double _shufflePeriod { get; set; }
            public double _previewStartTime { get; set; }
            public double _previewDuration { get; set; }
            public string _songFilename { get; set; }
            public string _coverImageFilename { get; set; }
            public string _environmentName { get; set; }
            public int _songTimeOffset { get; set; }
            public CustomData _customData { get; set; }
            public List<DifficultyBeatmapSet> _difficultyBeatmapSets { get; set; }
        }


    }
}
