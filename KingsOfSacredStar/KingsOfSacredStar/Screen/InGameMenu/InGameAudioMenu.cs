using System.Collections.Generic;
using KingsOfSacredStar.Properties;
using KingsOfSacredStar.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.InGameMenu
{
    internal sealed class InGameAudioMenu : AAudioMenu
    {
        public InGameAudioMenu(ContentManager content) : base(null, null, null, GetButtons(content)) {}


        private static AButton[] GetButtons(ContentManager content)
        {
            return new AButton[]
            {
                new MenuButton(content, 310, 270, 380, 60, new []{AudioMenu.MasterVolume}),
                new ActionButton(content, 330, 348, 340, 3, new []{""}, () => {}, Color.Black),
                new ActionButton(content, 310, 340, 20, 20, new []{"-"}, LowerMasterVolume, Color.Yellow,new List<Screen> {Screen.InGameAudioMenu}, new List<Screen> {Screen.InGameAudioMenu}),
                new ActionButton(content, 670, 340, 20, 20, new []{"+"}, HigherMasterVolume, Color.Yellow,new List<Screen> {Screen.InGameAudioMenu}, new List<Screen> {Screen.InGameAudioMenu}),
                new ActionButton(content, (int)(330 + 320 * SoundManager.MasterVolume), 340, 20, 20, new string[0], () => {}, Color.Blue),

                new MenuButton(content, 310, 470, 380, 60, new []{AudioMenu.Effects}),
                new ActionButton(content, 330, 548, 340, 3, new []{""}, () => {}, Color.Black),
                new ActionButton(content, 310, 540, 20, 20, new []{"-"}, LowerSoundVolume, Color.Yellow, new List<Screen> {Screen.InGameAudioMenu}, new List<Screen> {Screen.InGameAudioMenu}),
                new ActionButton(content, 670, 540, 20, 20, new []{"+"}, HigherSoundVolume, Color.Yellow, new List<Screen> {Screen.InGameAudioMenu}, new List<Screen> {Screen.InGameAudioMenu}),
                new ActionButton(content, (int)(330 + 320 * EffectVolume()), 540, 20, 20, new string[0], () => {}, Color.Blue),

                new MenuButton(content, 880, 270, 380, 60, new []{AudioMenu.Music}),
                new ActionButton(content, 900, 348, 340, 3, new []{""}, () => {}, Color.Black),
                new ActionButton(content, 880, 340, 20, 20, new []{"-"}, LowerMusicVolume, Color.Yellow, new List<Screen> {Screen.InGameAudioMenu}, new List<Screen> {Screen.InGameAudioMenu}),
                new ActionButton(content, 1240, 340, 20, 20, new []{"+"}, HigherMusicVolume, Color.Yellow, new List<Screen> {Screen.InGameAudioMenu}, new List<Screen> {Screen.InGameAudioMenu}),
                new ActionButton(content, (int)(900 + 320 * MusicVolume()), 340, 20, 20, new string[0], () => {}, Color.Blue),

                new MenuButton(content, 900, 500, 400, 100, new[] {AudioMenu.Back}, new List<Screen> {Screen.InGameMenu}, new List<Screen> {Screen.InGameAudioMenu})

            };
        }
    }
}