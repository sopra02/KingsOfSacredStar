using System.Globalization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace KingsOfSacredStar.Sound
{
    internal sealed class MusicManager : SoundManager
    {
        private readonly Song[] mSongs;

        public MusicManager(ContentManager content, params string[] paths)
        {
            Volume = float.Parse(Settings.Dict["MusicVolume"], CultureInfo.InvariantCulture);
            MediaPlayer.Volume = Volume;
            mSongs = new Song[paths.Length];
            for (var x = 0; x < paths.Length; x++)
            {
                mSongs[x] = content.Load<Song>(paths[x]);
            }
            MediaPlayer.IsRepeating = true;
        }
        public override void RefreshVolume()
        {
            MediaPlayer.Volume = Volume * MasterVolume;
        }

        public void Play(int song)
        {
            MediaPlayer.Stop();
            MediaPlayer.Play(mSongs[song]);
        }
    }
}
