using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Physics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Audio
{
    /// <summary>
    /// Represents the state of the audio source
    /// </summary>
    public enum AudioSourceState
    {
        None,
        Initialized,
        Playing,
        Paused,
        Stopped,
        Disposed
    }

    /// <summary>
    /// Represents the play mode of the audio source
    /// </summary>
    public enum PlayMode
    {
        Once = 0,
        Loop = 1
    }

    /// <summary>
    /// Represents an audio source
    /// </summary>
    public class AudioSource : IGameBehavior
    {
        /// <summary>
        /// The state of the audio source
        /// </summary>
        public AudioSourceState State { get; set; } = AudioSourceState.None;

        /// <summary>
        /// The ID of the audio source
        /// </summary>
        public int SourceId { get; set; }

        /// <summary>
        /// The audio clip of the audio source
        /// </summary>
        public AudioClip AudioClip { get => _audioClip; set => SetAudioClip(value); }

        /// <summary>
        /// The play mode of the audio source
        /// </summary>
        public PlayMode PlayMode { get => _playMode; set => SetPlayMode(value); }

        /// <summary>
        /// The gain (volume) of the audio source
        /// </summary>
        public float Gain { get => _gain; set => SetGain(value); }

        /// <summary>
        /// The range of the audio source
        /// </summary>
        public Vector3 Range { get => _range; set => SetRange(value); }

        private GameElement _gameElement;
        private IAudioDevice _audioDevice;
        private AudioClip _audioClip;
        private PlayMode _playMode = PlayMode.Once;
        private float _gain = 1.0f;
        private Vector3 _range = new Vector3(1.0f, float.PositiveInfinity, 1.0f);

        /// <summary>
        /// Creates a new audio source
        /// </summary>
        /// <param name="audioDevice"></param>
        /// <param name="playmode"></param>
        public AudioSource(IAudioDevice audioDevice, PlayMode playmode = PlayMode.Loop)
        {
            _audioDevice = audioDevice;
            _playMode = playmode;
        }

        /// <summary>
        /// Sets the audio clip for the audio source
        /// </summary>
        /// <param name="audioClip"></param>
        public void SetAudioClip(AudioClip audioClip)
        {
            _audioClip = audioClip;
            if(this.State != AudioSourceState.None || this.State != AudioSourceState.Disposed)
            {
                _audioDevice.LoadSourceClip(this, audioClip);
            }
        }

        /// <summary>
        /// Sets the play mode for the audio source
        /// </summary>
        /// <param name="playMode"></param>
        public void SetPlayMode(PlayMode playMode)
        {
            _playMode = playMode;
            if (this.State != AudioSourceState.None || this.State != AudioSourceState.Disposed)
            {
                switch (_playMode)
                {
                    case PlayMode.Once:
                        _audioDevice.SetAudioSourceLooping(this, false);
                        break;
                    case PlayMode.Loop:
                        _audioDevice.SetAudioSourceLooping(this, true);
                        break;
                    default:
                        _audioDevice.SetAudioSourceLooping(this, false);
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the range of the audio source
        /// </summary>
        /// <param name="value"></param>
        public void SetRange(Vector3 value)
        {
            _range = value;
            if (this.State != AudioSourceState.None || this.State != AudioSourceState.Disposed)
            {
                _audioDevice.SetAudioSourceRange(this, value.X, value.Y, value.Z);
            }
        }

        /// <summary>
        /// Sets the gain (volume) of the audio source
        /// </summary>
        /// <param name="value"></param>
        public void SetGain(float value)
        {
            if (this.State != AudioSourceState.None || this.State != AudioSourceState.Disposed)
            {
                _audioDevice.SetAudioSourceGain(this, value);
            }
        }

        /// <summary>
        /// Plays the audio source
        /// </summary>
        public void Play()
        {
            if (this.State == AudioSourceState.Stopped || this.State == AudioSourceState.Paused || this.State == AudioSourceState.Initialized)
            {
                _audioDevice.PlayAudioSource(this);
                this.State = AudioSourceState.Playing;
            }
        }

        /// <summary>
        /// Stops the audio source
        /// </summary>
        public void Stop()
        {
            if (this.State == AudioSourceState.Playing)
            {
                _audioDevice.StopAudioSource(this);
                this.State = AudioSourceState.Stopped;
            }
        }

        /// <summary>
        /// Pauses the audio source
        /// </summary>
        public void Pause()
        {
            if (this.State == AudioSourceState.Playing)
            {
                _audioDevice.PauseAudioSource(this);
                this.State = AudioSourceState.Paused;
            }
        }

        /// <summary>
        /// Returns the game element associated with the audio source
        /// </summary>
        /// <returns></returns>
        public GameElement GetElement()
        {
            return _gameElement;
        }

        /// <summary>
        /// Sets the game element associated with the audio source
        /// </summary>
        /// <param name="gameElement"></param>
        public void SetElement(GameElement gameElement)
        {
            _gameElement = gameElement;
        }

        /// <summary>
        /// Handles the collision event (not used in this implementation)
        /// </summary>
        /// <param name="collision"></param>
        public void OnCollide(Collision collision)
        {
            
        }

        /// <summary>
        /// Disposes the audio source
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public void OnDispose(BaseScene scene, IRenderDevice renderer)
        {
            _audioDevice.StopAudioSource(this);
            Debug.WriteLine($"Disposing audio source {this.SourceId}");
            _audioDevice.DisposeAudioSource(this);  
            this.State = AudioSourceState.Disposed;
            Debug.WriteLine($"Audio source {this.SourceId} disposed");
        }

        /// <summary>
        /// Initializes the audio source and sets the start parameters
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public void OnInit(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            Debug.WriteLine($"Initializing audio source {this.SourceId}");
            // Loads the audio source and sets the position in the world
            _audioDevice.LoadAudioSource(this);
            _audioDevice.SetAudioSourcePosition(this, _gameElement.Transform.Position);
            _audioDevice.SetAudioSourceGain(this, _gain);
            _audioDevice.SetAudioSourceRange(this, _range.X, _range.Y, _range.Z);

            // Sets the audio clip for the audio source if it is not null
            if (_audioClip != null)
            {
                _audioDevice.LoadSourceClip(this, _audioClip);
            }

            // Initializes the audio play mode
            switch (_playMode)
            {
                case PlayMode.Once:
                    _audioDevice.SetAudioSourceLooping(this, false);
                    break;
                case PlayMode.Loop:
                    _audioDevice.SetAudioSourceLooping(this, true);
                    break;
                default:
                    _audioDevice.SetAudioSourceLooping(this, false);
                    break;
            }

            this.State = AudioSourceState.Initialized;
            Debug.WriteLine($"Audio source {this.SourceId} initialized");
        }

        /// <summary>
        /// Handles the shadow pass rendering (not used in this implementation)
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public void OnShadowPass(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {

        }

        /// <summary>
        /// Handles the rendering of the audio source (not used in this implementation)
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public void OnRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            
        }

        /// <summary>
        /// Handles the update of the audio source
        /// </summary>
        /// <param name="scene"></param>
        public void OnUpdate(BaseScene scene, float dt)
        {
            if (this.State != AudioSourceState.None || this.State != AudioSourceState.Disposed)
            {
                var sourcePosition = _audioDevice.GetAudioSourcePosition(this);
                if(sourcePosition != _gameElement.Transform.Position)
                {
                    // Updates the position of the audio source
                    _audioDevice.SetAudioSourcePosition(this, _gameElement.Transform.Position);
                }
            }
        }

        /// <summary>
        /// Returns a clone of the audio source with the same parameters
        /// </summary>
        /// <returns></returns>
        public IGameBehavior Clone()
        {
            var clone = new AudioSource(_audioDevice, _playMode);
            clone.SetAudioClip(_audioClip);
            clone.SetGain(_gain);
            clone.SetRange(_range);
            return clone;
        }
    }
}
