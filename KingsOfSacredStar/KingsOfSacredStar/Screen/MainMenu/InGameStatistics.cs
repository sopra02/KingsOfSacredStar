using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal sealed class InGameStatistics : AMainMenuPage
    {
        public InGameStatistics(ContentManager content, GraphicsDeviceManager graphics, Camera camera) : base(content, graphics, camera, GetButtons(content)) {}

        private static AButton[] GetButtons(ContentManager content)
        {
            return new AButton[]
            {
                // Statistics x: 1600 / y: 900
                new MenuButton(content, 700, 80, 200, 50, new[] {Properties.StatisticsName.Statistics}),
                // Statistics for the player
                new MenuButton(content, 100, 160, 600, 50, new[] { Properties.StatisticsName.Player }),
                new MenuButton(content, 100, 240, 600, 50, new[] { Properties.StatisticsName.Gold + ": " + GameState.Current.StatisticsByPlayer[Players.Player]["Gold"]}),
                new MenuButton(content, 100, 320, 600, 50, new[] { Properties.StatisticsName.Stone + ": " + GameState.Current.StatisticsByPlayer[Players.Player]["Stone"]}),
                new MenuButton(content, 100, 400, 600, 50, new[] { Properties.StatisticsName.KilledTroops + ": " + GameState.Current.StatisticsByPlayer[Players.Player]["KilledTroops"]}),
                new MenuButton(content, 100, 480, 600, 50, new[] { Properties.StatisticsName.LostTroops + ": " + GameState.Current.StatisticsByPlayer[Players.Player]["LostTroops"]}),
                new MenuButton(content, 100, 560, 600, 50, new[] { Properties.StatisticsName.CreateTroops + ": " + GameState.Current.StatisticsByPlayer[Players.Player]["CreateTroops"]}),
                new MenuButton(content, 100, 640, 600, 50, new[] { Properties.StatisticsName.Won + ": " + GameState.Current.StatisticsByPlayer[Players.Player]["Won"]}),
                new MenuButton(content, 100, 720, 600, 50, new[] { Properties.StatisticsName.Lost + ": " + GameState.Current.StatisticsByPlayer[Players.Player]["Lost"]}),
                new MenuButton(content, 100, 800, 600, 50, new[] { Properties.StatisticsName.TimePlayed + ": " + GameState.Current.StatisticsByPlayer[Players.Player]["LastGamePlaytime"] + "s"}),
                // Statistics for the Ai
                new MenuButton(content, 900, 160, 600, 50, new[] { Properties.StatisticsName.AI }),
                new MenuButton(content, 900, 240, 600, 50, new[] { Properties.StatisticsName.Gold + ": " + GameState.Current.StatisticsByPlayer[Players.Ai]["Gold"]}),
                new MenuButton(content, 900, 320, 600, 50, new[] { Properties.StatisticsName.Stone + ": " + GameState.Current.StatisticsByPlayer[Players.Ai]["Stone"]}),
                new MenuButton(content, 900, 400, 600, 50, new[] { Properties.StatisticsName.KilledTroops + ": " + GameState.Current.StatisticsByPlayer[Players.Ai]["KilledTroops"]}),
                new MenuButton(content, 900, 480, 600, 50, new[] { Properties.StatisticsName.LostTroops + ": " + GameState.Current.StatisticsByPlayer[Players.Ai]["LostTroops"]}),
                new MenuButton(content, 900, 560, 600, 50, new[] { Properties.StatisticsName.CreateTroops + ": " + GameState.Current.StatisticsByPlayer[Players.Ai]["CreateTroops"]}),
                new MenuButton(content, 900, 640, 600, 50, new[] { Properties.StatisticsName.Won + ": " + GameState.Current.StatisticsByPlayer[Players.Ai]["Won"]}),
                new MenuButton(content, 900, 720, 600, 50, new[] { Properties.StatisticsName.Lost + ": " + GameState.Current.StatisticsByPlayer[Players.Ai]["Lost"]}),
                new MenuButton(content, 900, 800, 600, 50, new[] { Properties.StatisticsName.TimePlayed + ": " + GameState.Current.StatisticsByPlayer[Players.Ai]["LastGamePlaytime"] + "s"}),
                // Leave Statistics
                new MenuButton(content, 1300, 80, 250, 50, new[] { Properties.StatisticsName.LeaveStatistics }, new List<Screen> {Screen.MainMenu}, new List<Screen> {Screen.InGameStatistics})
            };
        }
    }
}