using System;
using System.Collections.Generic;
using KingsOfSacredStar.Screen;
using KingsOfSacredStar.Screen.MainMenu;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Statistic.Achievement
{
    internal abstract class Achievements : AMainMenuPage
    {
        protected readonly Dictionary<string, string> mAchievementsText = new Dictionary<string, string>();
        protected  readonly Dictionary<string, Requirements> mAchievementRequirements = new Dictionary<string, Requirements>();

        protected Achievements(ContentManager content, GraphicsDeviceManager graphics, Camera camera, AButton[] buttons) : base(content, graphics, camera, buttons)
        {
            AddAchievements();
        }

        private void AddAchievements()
        {
            var achievements = new Dictionary<string, Tuple<string, Requirements>>
            {
                { Properties.AchievementName.FirstWin, new Tuple<string, Requirements>(Properties.AchievementText.FirstWin, new Requirements("Won", 1)) },
                { Properties.AchievementName.MaybeNextTime, new Tuple<string, Requirements>(Properties.AchievementText.MaybeNextTime, new Requirements("Lost", 1)) },
                { Properties.AchievementName.GettingRich, new Tuple<string, Requirements>(Properties.AchievementText.GettingRich, new Requirements("Gold", 100)) },
                { Properties.AchievementName.LordOfTheWar, new Tuple<string, Requirements>(Properties.AchievementText.LordOfTheWar, new Requirements("KilledTroops", 100)) },
                { Properties.AchievementName.DonNotDie, new Tuple<string, Requirements>(Properties.AchievementText.DoNotDie, new Requirements("LostTroops", 100)) },
                { Properties.AchievementName.StartTheProduction, new Tuple<string, Requirements>(Properties.AchievementText.StartTheProduction, new Requirements("CreateTroops", 10)) },
                { Properties.AchievementName.Gatherer, new Tuple<string, Requirements>(Properties.AchievementText.Gatherer, new Requirements("Stone", 1000)) }
            };

            foreach (var achievement in achievements)
            {
                mAchievementsText.Add(achievement.Key, achievement.Value.Item1);
                mAchievementRequirements.Add(achievement.Key, achievement.Value.Item2);
            }
        }

        protected sealed class Requirements
        {
            public string Key { get; }
            public int Amount { get; }

            public Requirements(string key, int amount)
            {
                Key = key;
                Amount = amount;
            }
        }

        public override void Update(GameTime gameTime, Action<List<Screen.Screen>> addScreens, Action<List<Screen.Screen>> removeScreens)
        {
            base.Update(gameTime, addScreens, removeScreens);
            PanToPosition(new Vector3(3, 0, 7) * GameScreen.GridSize + new Vector3(GameScreen.GridSize / 2f, 0, 0), gameTime);
        }
    }
}
