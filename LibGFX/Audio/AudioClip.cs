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
    /// <summary>
    /// Represents the state of the audio clip
    /// </summary>
    public enum AudioClipState
    {
        None,
        Loaded,
        Initialized,
        Disposed
    }

    /// <summary>
    /// Represents an audio clip
    /// </summary>
    public class AudioClip
    {
        /// <summary>
        /// The name of the audio clip
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The buffer ID of the audio clip
        /// </summary>
        public int BufferId { get; set; }

        /// <summary>
        /// The bytes of the audio clip
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// The time of the audio clip in seconds
        /// </summary>
        public float Time { get; set; }

        /// <summary>
        /// The state of the audio clip
        /// </summary>
        public AudioClipState AudioClipState { get; set; }

        /// <summary>
        /// The wave format of the audio clip
        /// </summary>
        public WaveFormat WaveFormat { get; set; }

        /// <summary>
        /// Creates a new audio clip
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        public AudioClip(String name, String path)
        {
            this.Name = name;
            this.LoadAudio(path);
        }

        /// <summary>
        /// Loads the audio file into memory
        /// </summary>
        /// <param name="path"></param>
        private void LoadAudio(String path)
        {
            using var stream = File.OpenRead(path);
            var wave = new NAudio.Wave.WaveFileReader(stream);
            WaveFormat = wave.WaveFormat;
            var buffer = new byte[wave.Length];
            wave.Read(buffer, 0, (int)wave.Length);
            Bytes = buffer;

            this.Time = (float)wave.TotalTime.TotalSeconds;
            AudioClipState = AudioClipState.Loaded;
        }

    }
}
