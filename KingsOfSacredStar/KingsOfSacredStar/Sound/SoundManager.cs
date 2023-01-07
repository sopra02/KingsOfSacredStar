using System.Globalization;

namespace KingsOfSacredStar.Sound
{
    internal abstract class SoundManager
    {
        public static float MasterVolume { get; set; } = 0.5f;

        private float mVolume = 0.5f;
        public float Volume
        {
            get => mVolume;
            set
            {
                mVolume = value;
                RefreshVolume();
            }
        }

        public static void Init()
        {
            MasterVolume = float.Parse(Settings.Dict["MasterVolume"], CultureInfo.InvariantCulture);

        }

        public abstract void RefreshVolume();
    }
}