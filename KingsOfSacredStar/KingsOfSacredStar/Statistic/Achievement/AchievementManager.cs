using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.Screen;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Statistic.Achievement
{
    internal sealed class AchievementManager : Achievements
    {
        private readonly AchievementText[] mAchievements = new AchievementText[7];
        private readonly Statistics mStatistics;
        public AchievementManager(ContentManager content, GraphicsDeviceManager graphics, Camera camera) : base(content, graphics, camera, GetButtons(content))
        {
            mStatistics = new Statistics(content, graphics, camera);
            foreach (var entry in mAchievementsText.Select((entry, index) =>
                new { Achievement = entry.Key, Text = entry.Value, Index = index }))
            {
                SetAchievementAchieved(entry.Achievement, entry.Text, content, entry.Index);
            }
        }

        private static AButton[] GetButtons(ContentManager content)
        {
            return new AButton[]
            {
                new MenuButton(content, 1400, 750, 150, 100, new[] {Properties.AchievementName.Back}, new List<Screen.Screen> {Screen.Screen.AchievementsMenu}, new List<Screen.Screen> {Screen.Screen.AchievementManager})
            };
        }

        private void SetAchievementAchieved(string achievement, string text, ContentManager content, int i)
        {
            // Define achievement Text and set if it is achieved or not
            var achieved = AchievementAchieved(achievement);
            mAchievements[i] = new AchievementText(content, 100, (i + 1) * 100, 1000, 50, achievement + ": " + text, achieved);
        }

        private bool AchievementAchieved(string achievement)
        {
            var statistics = mStatistics.GetFullStatistics();

            var requirements = mAchievementRequirements[achievement];

            return requirements.Amount <= statistics[requirements.Key];
        }
        public override void DrawHud(SpriteBatch spriteBatch)
        {
            base.DrawHud(spriteBatch);
            foreach (var achievement in mAchievements)
            {
                achievement.Draw(spriteBatch, 1);
            }
        }
    }
}
