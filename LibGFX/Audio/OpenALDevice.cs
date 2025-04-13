using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Audio
{
    public class OpenALDevice : IAudioDevice
    {
        private ALDevice _device;
        private ALContextAttributes _contextAttributes;
        private ALContext _context;

        public void InitializeAudioDevice()
        {
            _device = ALC.OpenDevice(null);
            _contextAttributes = ALC.GetContextAttributes(_device);
            _context = ALC.CreateContext(_device, _contextAttributes);
        }

        public void MakeCurrent()
        {
            ALC.MakeContextCurrent(_context);
        }

        public void LoadAudioClip(AudioClip clip)
        {
            var format = clip.WaveFormat.Channels switch
            {
                1 => clip.WaveFormat.BitsPerSample == 8 ? ALFormat.Mono8 : ALFormat.Mono16,
                2 => clip.WaveFormat.BitsPerSample == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16,
                _ => throw new NotSupportedException("Unsupported channel count")
            };

            int buffer = AL.GenBuffer();
            AL.BufferData<byte>(buffer, format, clip.Bytes, clip.WaveFormat.SampleRate);
            clip.BufferId = buffer;
            clip.AudioClipState = AudioClipState.Initialized;
        }

        public void LoadAudioSource(AudioSource source)
        {
            var element = source.GetElement();

            var sourceId = AL.GenSource();
            AL.Source(sourceId, ALSource3f.Position, element.Transform.Position.X, element.Transform.Position.Y, element.Transform.Position.Z);
            source.SourceId = sourceId;
        }

        public void LoadSourceClip(AudioSource source, AudioClip audioClip)
        {
            AL.Source(source.SourceId, ALSourcei.Buffer, audioClip.BufferId);
        }

        public void SetAudioListenerPosition(Vector3 position)
        {
            AL.Listener(ALListener3f.Position, position.X, position.Y, position.Z);
        }

        public void SetAudioListenerOrientation(Vector3 forward, Vector3 up)
        {
            AL.Listener(ALListenerfv.Orientation, new float[] { forward.X, forward.Y, forward.Z, up.X, up.Y, up.Z });
        }

        public void SetAudioListenerVelocity(Vector3 velocity)
        {
            AL.Listener(ALListener3f.Velocity, velocity.X, velocity.Y, velocity.Z);
        }

        public void SetAudioSourcePosition(AudioSource source, Vector3 position)
        {
            var element = source.GetElement();
            AL.Source(source.SourceId, ALSource3f.Position, position.X, position.Y, position.Z);
        }

        public void DisposeAudioClip(AudioClip clip)
        {
            if (clip.AudioClipState == AudioClipState.Initialized)
            {
                AL.DeleteBuffer(clip.BufferId);
                clip.AudioClipState = AudioClipState.Disposed;
            }
        }

        public void DisposeAudioSource(AudioSource source)
        {
            if (source.SourceId != 0)
            {
                AL.DeleteSource(source.SourceId);
                source.SourceId = 0;
            }
        }

        public void SetAudioSourceVelocity(AudioSource source, Vector3 velocity)
        {
            AL.Source(source.SourceId, ALSource3f.Velocity, velocity.X, velocity.Y, velocity.Z);
        }

        public void SetAudioSourcePitch(AudioSource source, float pitch)
        {
            AL.Source(source.SourceId, ALSourcef.Pitch, pitch);
        }

        public void SetAudioSourceGain(AudioSource source, float gain)
        {
            AL.Source(source.SourceId, ALSourcef.Gain, gain);
        }

        public void SetAudioSourceLooping(AudioSource source, bool looping)
        {
            AL.Source(source.SourceId, ALSourceb.Looping, looping);
        }

        public void PlayAudioSource(AudioSource source)
        {
            AL.SourcePlay(source.SourceId);
        }

        public void StopAudioSource(AudioSource source)
        {
            AL.SourceStop(source.SourceId);
        }

        public void PauseAudioSource(AudioSource source)
        {
            AL.SourcePause(source.SourceId);
        }

        public void SetAudioSourceRange(AudioSource source, float min, float max)
        {
            AL.Source(source.SourceId, ALSourcef.ReferenceDistance, min);
            AL.Source(source.SourceId, ALSourcef.MaxDistance, max);
        }

        public Vector3 GetAudioSourcePosition(AudioSource source)
        {
            AL.GetSource(source.SourceId, ALSource3f.Position, out float x, out float y, out float z);
            return new Vector3(x, y, z);
        }

        public void SetAudioSourceTime(AudioSource source, float time)
        {
            AL.Source(source.SourceId, ALSourcef.SecOffset, time);
        }

        public float GetAudioSourceTime(AudioSource source)
        {
            AL.GetSource(source.SourceId, ALSourcef.SecOffset, out float time);
            return time;
        }
    }
}
