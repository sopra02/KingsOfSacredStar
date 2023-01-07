using System.Diagnostics;
using KingsOfSacredStar.Screen.MainMenu;
using KingsOfSacredStar.Sound;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen
{
    internal abstract class AAudioMenu : AMainMenuPage
    {
        private const float DialAmount = 1f / 16;
        private static SoundEffectManager sSoundEffectManager;
        private static MusicManager sMusicManager;

        public static void Init(SoundEffectManager soundEffectManager, MusicManager musicManager)
        {
            Debug.Assert(sSoundEffectManager == null);
            Debug.Assert(sMusicManager == null);
            sSoundEffectManager = soundEffectManager;
            sMusicManager = musicManager;
        }

        protected AAudioMenu(ContentManager content, GraphicsDeviceManager graphics, Camera camera, AButton[] buttons) : base(content, graphics, camera, buttons) { }

        protected static float EffectVolume() => sSoundEffectManager.Volume;
        protected static float MusicVolume() => sMusicManager.Volume;

        protected static void HigherMasterVolume()
        {
            var masterVolume = SoundManager.MasterVolume;
            if (masterVolume > 1 - DialAmount) return;
            SoundManager.MasterVolume += DialAmount;
            sSoundEffectManager.RefreshVolume();
            sMusicManager.RefreshVolume();
        }

        protected static void LowerMasterVolume()
        {
            var masterVolume = SoundManager.MasterVolume;
            if (masterVolume < DialAmount) return;
            SoundManager.MasterVolume -= DialAmount;
            sSoundEffectManager.RefreshVolume();
            sMusicManager.RefreshVolume();
        }

        protected static void HigherSoundVolume()
        {
            if (sSoundEffectManager.Volume > 1 - DialAmount) return;
            sSoundEffectManager.Volume += DialAmount;
        }

        protected static void LowerSoundVolume()
        {
            if (sSoundEffectManager.Volume < DialAmount) return;
            sSoundEffectManager.Volume -= DialAmount;
        }

        protected static void HigherMusicVolume()
        {
            if (sMusicManager.Volume > 1 - DialAmount) return;
            sMusicManager.Volume += DialAmount;
        }

        protected static void LowerMusicVolume()
        {
            if (sMusicManager.Volume < DialAmount) return;
            sMusicManager.Volume -= DialAmount;
        }
    }
}
