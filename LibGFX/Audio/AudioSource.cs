using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Pyhsics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Audio
{
    public class AudioSource : IGameBehavior
    {
        public int SourceId { get; set; }

        private GameElement _gameElement;
        private IAudioDevice _audioDevice;


        public AudioSource(IAudioDevice audioDevice)
        {
            _audioDevice = audioDevice;
        }

        public void CreateAudioSource()
        {
            _audioDevice.LoadAudioSource(this);
            _audioDevice.SetAudioSourcePosition(this, _gameElement.Transform.Position);
        }

        public void SetAudioClip(AudioClip audioClip)
        {
            _audioDevice.LoadSourceClip(this, audioClip);
        }

        public GameElement GetElement()
        {
            return _gameElement;
        }

        public void SetElement(GameElement gameElement)
        {
            _gameElement = gameElement;
        }

        public void OnCollide(Collision collision)
        {
            
        }

        public void OnDispose(BaseScene scene, IRenderDevice renderer)
        {
            
        }

        public void OnInit(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            
        }

        public void OnRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            
        }

        public void OnUpdate(BaseScene scene)
        {
            
        }
    }
}
