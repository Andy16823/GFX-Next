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
        /// <summary>
        /// Initializes the audio device
        /// </summary>
        void InitializeAudioDevice();

        /// <summary>
        /// Makes the audio device current
        /// </summary>
        void MakeCurrent();

        /// <summary>
        /// Loads an audio clip into memory
        /// </summary>
        /// <param name="clip"></param>
        void LoadAudioClip(AudioClip clip);

        /// <summary>
        /// Disposes of an audio clip
        /// </summary>
        /// <param name="clip"></param>
        void DisposeAudioClip(AudioClip clip);

        /// <summary>
        /// Loads an audio source into memory
        /// </summary>
        /// <param name="source"></param>
        void LoadAudioSource(AudioSource source);

        /// <summary>
        /// Disposes of an audio source
        /// </summary>
        /// <param name="source"></param>
        void DisposeAudioSource(AudioSource source);

        /// <summary>
        /// Loads an audio clip into an audio source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="audioClip"></param>
        void LoadSourceClip(AudioSource source, AudioClip audioClip);

        /// <summary>
        /// Sets the position of the audio listener
        /// </summary>
        /// <param name="position"></param>
        void SetAudioListenerPosition(Vector3 position);

        /// <summary>
        /// Sets the orientation of the audio listener
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        void SetAudioListenerOrientation(Vector3 forward, Vector3 up);

        /// <summary>
        /// Sets the velocity of the audio listener
        /// </summary>
        /// <param name="velocity"></param>
        void SetAudioListenerVelocity(Vector3 velocity);

        /// <summary>
        /// Sets the position of the audio source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="position"></param>
        void SetAudioSourcePosition(AudioSource source, Vector3 position);

        /// <summary>
        /// Gets the position of the audio source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        Vector3 GetAudioSourcePosition(AudioSource source);

        /// <summary>
        /// Sets the velocity of the audio source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="velocity"></param>
        void SetAudioSourceVelocity(AudioSource source, Vector3 velocity);

        /// <summary>
        /// Sets the pitch of the audio source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pitch"></param>
        void SetAudioSourcePitch(AudioSource source, float pitch);

        /// <summary>
        /// Sets the gain of the audio source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="gain"></param>
        void SetAudioSourceGain(AudioSource source, float gain);

        /// <summary>
        /// Sets the range of the audio source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        void SetAudioSourceRange(AudioSource source, float min, float max);

        /// <summary>
        /// Sets the looping state of the audio source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="looping"></param>
        void SetAudioSourceLooping(AudioSource source, bool looping);

        /// <summary>
        /// Plays the audio source
        /// </summary>
        /// <param name="source"></param>
        void PlayAudioSource(AudioSource source);

        /// <summary>
        /// Stops the audio source and sets the time to 0
        /// </summary>
        /// <param name="source"></param>
        void StopAudioSource(AudioSource source);

        /// <summary>
        /// Pauses the audio source
        /// </summary>
        /// <param name="source"></param>
        void PauseAudioSource(AudioSource source);

        /// <summary>
        /// Sets the time of the audio source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="time"></param>
        void SetAudioSourceTime(AudioSource source, float time);

        /// <summary>
        /// Gets the time of the audio source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        float GetAudioSourceTime(AudioSource source);
    }
}
