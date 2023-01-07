using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KingsOfSacredStar.Screen;
using KingsOfSacredStar.Screen.MainMenu;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Statistic
{
    internal sealed class Statistics : AMainMenuPage
    {
        private readonly Dictionary<string, int> mStatisticsOld = new Dictionary<string, int>();
        public readonly Dictionary<string, int> mStatisticsNew = new Dictionary<string, int>();
        private const string FilePath = @"KossStatistics.ini";
        private StatisticsText mStatisticGold;
        private StatisticsText mStatisticStone;
        private StatisticsText mStatisticCreatedTroops;
        private StatisticsText mStatisticLostTroops;
        private StatisticsText mStatisticKilledTroops;
        private StatisticsText mStatisticWon;
        private StatisticsText mStatisticLost;
        private readonly ContentManager mContent;

        public Statistics(ContentManager content, GraphicsDeviceManager graphics, Camera camera) : base(content, graphics, camera, GetButtons(content))
        {
            if (File.Exists(FilePath))
            {
                ReadStatistics();
            }
            else
            {
                InitStatistic();
                WriteStatistics();
            }

            foreach (var text in mStatisticsOld)
            {
                SetStatistics(text.Key, text.Value.ToString(), content);
            }

            InitStatistic();
            mContent = content;

        }

        private static AButton[] GetButtons(ContentManager content)
        {
            return new AButton[]
            {
                new MenuButton(content, 1400, 750, 150, 100, new[] { Properties.StatisticsName.Back },new List<Screen.Screen> {Screen.Screen.AchievementsMenu},new List<Screen.Screen> {Screen.Screen.Statistics})
            };
        }

        public void WriteStatistics()
        {
            MergeStatistics();
            var keyValue = mStatisticsOld.Select(i => string.Concat(i.Key, " ", i.Value));

            File.WriteAllLines(FilePath, keyValue);
            UpdateStatistics();
        }

        private void ReadStatistics()
        {
            foreach (var line in File.ReadLines(FilePath))
            {
                var items = line.Split(' ');
                if (items.Length != 2) continue;

                mStatisticsOld[items[0]] = int.Parse(items[1]);
            }
        }

        // initializes a dictionary for the statistics if needed
        private void InitStatistic()
        {
            mStatisticsNew["Gold"] = 0;
            mStatisticsNew["Stone"] = 0;
            mStatisticsNew["KilledTroops"] = 0;
            mStatisticsNew["LostTroops"] = 0;
            mStatisticsNew["CreateTroops"] = 0;
            mStatisticsNew["Won"] = 0;
            mStatisticsNew["Lost"] = 0;
            mStatisticsNew["LastGamePlaytime"] = 0;
        }

        // get the right information for any specific statistic
        private void SetStatistics(string statistic, string text, ContentManager content)
        {
            switch (statistic)
            {
                case "Gold":
                    mStatisticGold = new StatisticsText(content, 100, 100, 1000, 50, Properties.StatisticsName.Gold + ": " + text);
                    break;
                case "Stone":
                    mStatisticStone = new StatisticsText(content, 100, 200, 1000, 50, Properties.StatisticsName.Stone + ": " + text);
                    break;
                case "CreateTroops":
                    mStatisticCreatedTroops = new StatisticsText(content, 100, 300, 1000, 50, Properties.StatisticsName.CreateTroops + ": " + text);
                    break;
                case "KilledTroops":
                    mStatisticKilledTroops = new StatisticsText(content, 100, 400, 1000, 50, Properties.StatisticsName.KilledTroops + ": " + text);
                    break;
                case "LostTroops":
                    mStatisticLostTroops = new StatisticsText(content, 100, 500, 1000, 50, Properties.StatisticsName.LostTroops + ": " + text);
                    break;
                case "Won":
                    mStatisticWon = new StatisticsText(content, 100, 600, 1000, 50, Properties.StatisticsName.Won + ": " + text);
                    break;
                case "Lost":
                    mStatisticLost = new StatisticsText(content, 100, 700, 1000, 50, Properties.StatisticsName.Lost + ": " + text);
                    break;
                default:
                    Console.WriteLine(@"Invalid String {0}", statistic);
                    break;
            }
        }

        public override void DrawHud(SpriteBatch spriteBatch)
        {
            base.DrawHud(spriteBatch);
            mStatisticGold.Draw(spriteBatch, 1);
            mStatisticStone.Draw(spriteBatch, 1);
            mStatisticCreatedTroops.Draw(spriteBatch, 1);
            mStatisticKilledTroops.Draw(spriteBatch, 1);
            mStatisticLostTroops.Draw(spriteBatch, 1);
            mStatisticWon.Draw(spriteBatch, 1);
            mStatisticLost.Draw(spriteBatch, 1);
        }

        // looks for some action on the back button and get back to the AchievementsMenu if pressed

        // returns the full statistic dictionary
        public Dictionary<string, int> GetFullStatistics()
        {
            return mStatisticsOld;
        }

        private void MergeStatistics()
        {
            foreach (var item in mStatisticsNew.Keys)
            {
                mStatisticsOld[item] += mStatisticsNew[item];
            }
        }

        private void UpdateStatistics()
        {
            ReadStatistics();
            foreach (var text in mStatisticsOld)
            {
                SetStatistics(text.Key, text.Value.ToString(), mContent);
            }
        }

        public override void Update(GameTime gameTime, Action<List<Screen.Screen>> addScreens, Action<List<Screen.Screen>> removeScreens)
        {
            base.Update(gameTime, addScreens, removeScreens);
            PanToPosition(new Vector3(3, 0, -14) * GameScreen.GridSize, gameTime);
        }
    }
}