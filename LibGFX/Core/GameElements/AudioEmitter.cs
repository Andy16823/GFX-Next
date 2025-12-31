using LibGFX.Audio;
using LibGFX.Graphics;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core.GameElements
{
    /// <summary>
    /// Represents an audio emitter in 3D space
    /// </summary>
    public class AudioEmitter : GameElement
    {
        /// <summary>
        /// The audio source for this emitter
        /// </summary>
        private AudioSource _source;

        /// <summary>
        /// Gets the audio source for this emitter
        /// </summary>
        public AudioSource Source { get => _source; }

        /// <summary>
        /// Gets a value indicating whether the image contains any transparent pixels.
        /// </summary>
        public override bool HasTransparency => false;

        /// <summary>
        /// Creates a new audio emitter
        /// </summary>
        /// <param name="audioDevice"></param>
        /// <param name="playMode"></param>
        public AudioEmitter(IAudioDevice audioDevice, PlayMode playMode = PlayMode.Loop)
        {
            _source = new AudioSource(audioDevice);
            _source.SetRange(new Vector3(10, 10, 10));
        }

        /// <summary>
        /// Creates a new audio emitter with a clip
        /// </summary>
        /// <param name="audioDevice"></param>
        /// <param name="clip"></param>
        /// <param name="playMode"></param>
        public AudioEmitter(IAudioDevice audioDevice, AudioClip clip, PlayMode playMode = PlayMode.Loop) 
        {
            _source = new AudioSource(audioDevice);
            _source.SetAudioClip(clip);
        }

        /// <summary>
        /// Initializes the audio emitter
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            this.AddBehavior<AudioSource>(_source);
            base.Init(scene, viewport, renderer);
        }

        /// <summary>
        /// Sets the audio clip for this emitter
        /// </summary>
        /// <param name="clip"></param>
        public void SetAudioClip(AudioClip clip)
        {
            _source.SetAudioClip(clip);
        }

        /// <summary>
        /// Plays the audio source
        /// </summary>
        public void Play()
        {
            _source.Play();
        }

        /// <summary>
        /// Stops the audio source
        /// </summary>
        public void Stop()
        {
            _source.Stop();
        }

        /// <summary>
        /// Pauses the audio source
        /// </summary>
        public void Pause()
        {
            _source.Pause();
        }

        /// <summary>
        /// Sets the play mode for this emitter
        /// </summary>
        /// <param name="playMode"></param>
        public void SetPlayMode(PlayMode playMode)
        {
            _source.SetPlayMode(playMode);
        }

        /// <summary>
        /// Sets the range for this emitter
        /// </summary>
        /// <param name="value"></param>
        public void SetRange(Vector3 value)
        {
            _source.SetRange(value);
        }

        /// <summary>
        /// Computes the axis-aligned bounding box (AABB) for this audio emitter.
        /// </summary>
        override public void ComputeAABB()
        {
            var min = this.Transform.Position - (_source.Range / 2);
            var max = this.Transform.Position + (_source.Range / 2);

            this.AABB = new AABB(min, max);
        }
    }
}
