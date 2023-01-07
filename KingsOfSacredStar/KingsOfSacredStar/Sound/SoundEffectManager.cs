using System.Globalization;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Sound
{
    internal sealed class SoundEffectManager : SoundManager
    {
        private readonly SoundEffectInstance[] mSoundEffects;

        public SoundEffectManager(ContentManager content, params string[] paths)
        {
            mSoundEffects = new SoundEffectInstance[paths.Length];
            for (var x = 0; x < paths.Length; x++)
            {
                mSoundEffects[x] = content.Load<SoundEffect>(paths[x]).CreateInstance();
            }

            Volume = float.Parse(Settings.Dict["EffectVolume"], CultureInfo.InvariantCulture);
        }


        public override void RefreshVolume()
        {
            foreach (var soundEffectInstance in mSoundEffects)
            {
                soundEffectInstance.Volume = MasterVolume * Volume;
            }
        }

        public void Play(int sound)
        {
            mSoundEffects[sound].Play();
        }
    }
}