using VainEngine;
using OpenTK.Input;
using NAudio.Vorbis;
using System.Threading.Tasks;
using System;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GridBeatz
{
    public class Conductor : Component
    {
        public float time = 0;
        public BeatMapData.Root mapData;
        public float BPM;
        public string musicPath;
        int noteCap = 0;
        //UIBox judgementLine = new UIBox(0.4f, 0.01f, Texture.LoadFromFile(@"Resources\white.png"));
        public override void Start()
        {
            base.Start();
            
            Camera.euler = new OpenTK.Mathematics.Vector3(0, -90, 0);
            Camera.pos = new OpenTK.Mathematics.Vector3(0, 1, -5);
            Camera.fov = 55;
            scoreText.position.X = -1;
            scoreText.position.Y = 1;
            //judgementLine.position.X = 0.7f;
            //judgementLine.RebuildMesh();
            PlayAudio();
            Program.w.ReleaseMouse();
            scoreText.size = 4;
            scoreText.SetText(score.ToString());
        }
        DateTime start;
        Keys[][] keyGrid = 
        new Keys[3][]{ 
            new Keys[4]{ Keys.Z, Keys.X, Keys.C, Keys.V },
            new Keys[4]{ Keys.A, Keys.S, Keys.D, Keys.F },
            new Keys[4]{ Keys.Q, Keys.W, Keys.E, Keys.R } 
        };
        public override void Update()
        {
            if (true)
            {
                base.Update();
                for (int i = noteCap; i < mapData._notes.Count; i++)
                {
                    var note = mapData._notes[i];
                    if (time > (((note._time - 8f) * 60) / BPM))
                    {
                        noteCap++;
                        if (note._type == 3)
                            continue;
                        GameObject n = new GameObject();
                        n.position = new OpenTK.Mathematics.Vector3(note._lineIndex - 1.5f, -note._lineLayer, 0);
                        
                        var noteComponent = (Note)n.AddComponent(new Note());
                        noteComponent.assignedKey = keyGrid[note._lineLayer][note._lineIndex];
                        noteComponent.targetTime = (float)note._time;
                        noteComponent.conductor = this;
                        n.viewMesh = new Mesh("Resources/sphere.obj", Texture.LoadFromFile("Resources/white.png"));
                        n.scale = 0.3f;
                        n.viewMesh.emission = 1;
                    }
                }
            }
            else
            {
                start = DateTime.Now;
            }
            
        }
        public void UpdateScore()
        {
            scoreText.SetText(score.ToString());
        }
        bool audioPlaying = false;
        bool audioCancelling = false;
        public int score = 0;
        UIText scoreText = new UIText();
        public override void End()
        {
            base.End();
            audioCancelling = true;
        }
        public float offset = -0.25f;
        void PlayAudio()
        {
            Task.Run(() =>
            {
                using (var vorbisStream = new NAudio.Vorbis.VorbisWaveReader(musicPath))
                using (var waveOut = new NAudio.Wave.WasapiOut())
                {
                    
                    waveOut.Init(vorbisStream);
                    waveOut.Play();
                    audioPlaying = waveOut.PlaybackState == NAudio.Wave.PlaybackState.Playing;
                    while (waveOut.PlaybackState == NAudio.Wave.PlaybackState.Playing) 
                    { 
                        time = (float)vorbisStream.CurrentTime.TotalSeconds + offset; 
                        if (audioCancelling == true) 
                            waveOut.Stop(); 
                    }
                }
            });
        }
    }
}
