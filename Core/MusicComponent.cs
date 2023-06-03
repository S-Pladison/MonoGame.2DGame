using Microsoft.Xna.Framework.Media;
using Pladi.Content;

namespace Pladi.Core
{
    public class MusicComponent : BaseComponent
    {
        public float Volume
        {
            get => MediaPlayer.Volume;
            set => MediaPlayer.Volume = value;
        }

        public override void Initialize()
        {
            MediaPlayer.Play(AudioAssets.Default);
            Volume = 0.75f;
        }
    }
}