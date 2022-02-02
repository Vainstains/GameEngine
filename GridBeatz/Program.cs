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
        public static string levelPath, soundPath;
        public static VainEngine.Window w;
        public static BeatMapData.Root map;
        public static float BPM;
        public static float offset;
        static void Main(string[] args)
        {
            Console.WriteLine("Please drag in any beat saber 'Info.dat'");

            levelPath = Console.ReadLine().Trim('"');
            BeatInfoData.Root mapInfo = MapInfo(levelPath);
            List<BeatInfoData.DifficultyBeatmapSet> diffsets = mapInfo._difficultyBeatmapSets;
            List<BeatInfoData.DifficultyBeatmap> difficultyBeatmaps = mapInfo._difficultyBeatmapSets[0]._difficultyBeatmaps;

            Console.WriteLine("Found the following difficulty maps:");
            Console.WriteLine();

            for (int i = 0; i < difficultyBeatmaps.Count; i++)
            {
                BeatInfoData.DifficultyBeatmap d1 = difficultyBeatmaps[i];
                if(d1._customData != null)
                    Console.WriteLine($"{i}: {d1._customData._difficultyLabel} ({d1._difficulty})");
                else
                    Console.WriteLine($"{i}: {d1._difficulty}");
            }
            Console.WriteLine();
            Console.WriteLine("Please type the number next to the difficulty you would like to play.");
            int.TryParse(Console.ReadLine(), out int selected);
            offset = (float)difficultyBeatmaps[selected]._noteJumpStartBeatOffset;
            string fileName = difficultyBeatmaps[selected]._beatmapFilename;
            FileInfo d = new FileInfo(levelPath);
            var parent = d.Directory.FullName + "\\" + fileName;
            map = BeatMap(parent);
            BPM = (float)mapInfo._beatsPerMinute;
            soundPath = d.Directory.FullName + "\\" + mapInfo._songFilename;


            w = VainEngine.Window.Create(1000,720,  "Grid Beatz (I bought the entire keyboard, ima use the entire keyboard)");
            w.Setup(new Program());
            
            w.Run();
        }
        public override void Start()
        {
            base.Start();
            w.ReleaseMouse();
            Global.SkyColor(new OpenTK.Mathematics.Color4(15, 15, 23, 255));
            Camera.up = -OpenTK.Mathematics.Vector3.UnitY;
            //fps = new UIText();
            BeatMapPlayer.PlayBeatmap(map, BPM, soundPath);

        }
        UIText fps;
        public override void Update()
        {
            base.Update();
            //fps.size = 1.5f;
            //fps.position = new OpenTK.Mathematics.Vector2(-1, 0.8f);
            //fps.SetText("fps:"+(1 / frameTime).ToString());
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
            public double _time { get; set; }
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
            public double _editorOffset { get; set; }
            public double _editorOldOffset { get; set; }
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
            public double _beatsPerMinute { get; set; }
            public double _shuffle { get; set; }
            public double _shufflePeriod { get; set; }
            public double _previewStartTime { get; set; }
            public double _previewDuration { get; set; }
            public string _songFilename { get; set; }
            public string _coverImageFilename { get; set; }
            public string _environmentName { get; set; }
            public double _songTimeOffset { get; set; }
            public CustomData _customData { get; set; }
            public List<DifficultyBeatmapSet> _difficultyBeatmapSets { get; set; }
        }


    }
}
