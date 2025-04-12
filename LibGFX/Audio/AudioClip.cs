using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using OpenTK.Audio.OpenAL;

namespace LibGFX.Audio
{
    public enum AudioClipState
    {
        None,
        Loaded,
        Initialized,
        Disposed
    }

    public class AudioClip
    {
        public String Name { get; set; }
        public int BufferId { get; set; }
        public byte[] Bytes { get; set; }
        public AudioClipState AudioClipState { get; set; }
        public WaveFormat WaveFormat { get; set; }

        public AudioClip(String name, String path)
        {
            this.Name = name;
            this.LoadAudio(path);
        }

        private void LoadAudio(String path)
        {
            using var stream = File.OpenRead(path);
            var wave = new NAudio.Wave.WaveFileReader(stream);
            WaveFormat = wave.WaveFormat;
            var buffer = new byte[wave.Length];
            wave.Read(buffer, 0, (int)wave.Length);
            Bytes = buffer;
            
            // Load the audio file into memory
            // Set the AudioClipState to Loaded
            AudioClipState = AudioClipState.Loaded;
        }

    }
}
