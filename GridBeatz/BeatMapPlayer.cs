using System;
using System.Collections.Generic;
using System.Text;
using VainEngine;
namespace GridBeatz
{
    public static class BeatMapPlayer
    {
        public static void PlayBeatmap(BeatMapData.Root Map, float BPM, string audioPath)
        {
            GameObject conductor = new GameObject("conductor");
            var c = (Conductor)conductor.AddComponent(new Conductor());
            c.mapData = Map;
            c.BPM = BPM;
            c.musicPath = audioPath;
        }
    }
}
