using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Audio
{
    public interface IAudioDevice
    {
        void InitializeAudioDevice();
        void MakeCurrent();
        void LoadAudioClip(AudioClip clip);
        void DisposeAudioClip(AudioClip clip);
        void LoadAudioSource(AudioSource source);
        void DisposeAudioSource(AudioSource source);
        void LoadSourceClip(AudioSource source, AudioClip audioClip);
        void SetAudioListenerPosition(Vector3 position);
        void SetAudioListenerOrientation(Vector3 forward, Vector3 up);
        void SetAudioListenerVelocity(Vector3 velocity);
        void SetAudioSourcePosition(AudioSource source, Vector3 position);
        void SetAudioSourceVelocity(AudioSource source, Vector3 velocity);
        void SetAudioSourcePitch(AudioSource source, float pitch);
        void SetAudioSourceGain(AudioSource source, float gain);
        void SetAudioSourceLooping(AudioSource source, bool looping);
        void PlayAudioSource(AudioSource source);
        void StopAudioSource(AudioSource source);
        void PauseAudioSource(AudioSource source);
    }
}
