using System;
using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.InputWrapper;
using KingsOfSacredStar.Screen;
using KingsOfSacredStar.World.Unit;
using KingsOfSacredStar.World.Unit.Skills;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.World
{
    internal sealed class LoadAndSaveManager
    {
        private readonly Camera mCamera;
        private readonly GameScreen mGameScreen;
        private readonly int mGridSize;
        private readonly ContentManager mContent;
        public static LoadAndSaveManager Current { get; set; }

#if DEBUG
        private const string Path = "../../../";
#else
        private const string Path = "";
#endif
        public LoadAndSaveManager(int gridSize, Camera camera, GameScreen gameScreen, ContentManager content)
        {
            mGridSize = gridSize;
            mCamera = camera;
            mGameScreen = gameScreen;
            mContent = content;
        }


        public void SaveGame(string name)
        {
            var saveAllList = new List<string>
            {
                "map01 " + GameState.Current.Resources.Keys.Count,
                mCamera.Position.X + " " + mCamera.Position.Y + " " + mCamera.Position.Z
            };
            foreach (var player in GameState.Current.Resources.Keys)
            {
                saveAllList.Add((int)player + " " + GameState.Current.Resources[player][Resources.Gold] + " " + GameState.Current.Resources[player][Resources.Stone]);
            }

            foreach (var model in GameState.Current.UnitsByModel.Keys)
            {
                foreach (var unit in GameState.Current.UnitsByModel[model])
                {
                    saveAllList.Add(unit.Serialize());
                }
            }
            if (FileReaderWriter.FileExists(Path + "save/" + name))
            {
                FileReaderWriter.WriteFile(Path + "save/" + name, saveAllList);
            }
            else
            {
                FileReaderWriter.CreateParentDirectories(Path + "save/");
                FileReaderWriter.WriteFile(Path + "save/" + name, saveAllList);
            }
        }


        public void LoadMap(string name, bool newGame)
        {
            if (!FileReaderWriter.FileExists(Path + "map/" + name))
            {
                return;
            }

            GameState.Current?.SaveStatistics();
            GameState.Current = new GameState(mCamera, mGridSize, mContent);
            var lineNum = 0;
            var data = FileReaderWriter.GetFullFile(Path + "map/" + name);
            var objects = data[lineNum].Split(' ');
            lineNum++;
            GameState.Current.GenerateEmptyFreeMap(int.Parse(objects[0]), int.Parse(objects[1]));
            AddRockLayer(objects);
            mGameScreen.SetMapSize(int.Parse(objects[0]) * mGridSize, int.Parse(objects[1]) * mGridSize);
            if (newGame)
            {
                objects = data[lineNum].Split(' ');
                var playerNum = int.Parse(objects[3]);
                var pos = new Vector2(
                    float.Parse(objects[0]),
                    float.Parse(objects[1]));
                GameState.Current.AddBuilding(Players.Global,
                    new Vector2(pos.X, pos.Y),
                    UnitTypes.SacredStar,
                    Math.Abs(float.Parse(objects[2])));

                AddPlayer(data, playerNum);
                AddMines(data, playerNum + 2);

            }
        }

        private static void AddRockLayer(IReadOnlyList<string> objects)
        {
            var maxX = int.Parse(objects[0]);
            var maxY = int.Parse(objects[1]);
            for (var i = 0; i < maxX; i++)
            {
                var xPos = i - (maxX - 1) / 2;
                var yPos = (maxY - 1) / 2;
                GameState.Current.AddBuilding(Players.Global, new Vector2(xPos, yPos), UnitTypes.Rock);
                GameState.Current.AddBuilding(Players.Global, new Vector2(xPos, -yPos - 1), UnitTypes.Rock);
            }

            for (var i = 0; i < maxY; i++)
            {
                var xPos = (maxX - 1) / 2;
                var yPos = i - (maxY - 1) / 2;
                GameState.Current.AddBuilding(Players.Global, new Vector2(-xPos - 1, yPos), UnitTypes.Rock);
                GameState.Current.AddBuilding(Players.Global, new Vector2(xPos, yPos), UnitTypes.Rock);
            }
        }
        private void AddPlayer(IReadOnlyList<string> data, int playerNum)
        {
            for (var i = 2; i < playerNum + 2; i++)
            {
                var objects = data[i].Split(' ');
                Vector2 pos;
                pos.X = float.Parse(objects[0]);
                pos.Y = float.Parse(objects[1]);
                GameState.Current.AddUnit((Players)(i- 1), pos, UnitTypes.Hero);
                GameState.Current.AddBuilding((Players)(i - 1),
                    new Vector2(pos.X + 1, pos.Y),
                    UnitTypes.Village,
                    Math.Abs(float.Parse(objects[2])));
                if (!GameState.Current.Resources.ContainsKey((Players)(i - 1)))
                {
                    GameState.Current.Resources[(Players)(i - 1)] = new Dictionary<Resources, int>
                    {
                        [Resources.Gold] = 10,
                        [Resources.Stone] = 100
                    };
                }

                GameState.Current.CreateCollisionManger(mGridSize);
            }
        }
        private static void AddMines(IReadOnlyList<string> data, int lineNum)
        {
            for (var i = 0; i < 3; i++)
            {
                var objects = data[lineNum].Split(' ');
                var actualObj = UnitTypes.GoldMine;
                switch (objects[0])
                {
                    case "GoldMine":
                        actualObj = UnitTypes.GoldMine;
                        break;
                    case "Quarry":
                        actualObj = UnitTypes.Quarry;
                        break;
                    case "Rock":
                        actualObj = UnitTypes.Rock;
                        break;
                }

                var numOfType = int.Parse(objects[1]);
                lineNum++;
                for (var l = 0; l < numOfType; l++)
                {
                    objects = data[lineNum].Split(' ');
                    var pos = new Vector2(int.Parse(objects[0]), int.Parse(objects[1]));
                    GameState.Current.AddBuilding(Players.Global,
                        pos,
                        actualObj,
                        Math.Abs(float.Parse(objects[2])));
                    lineNum++;
                }
            }
        }
        public void LoadGame(string name)
        {
            if (!FileReaderWriter.FileExists(Path + "save/" + name))
            {
                return;
            }

            var data = FileReaderWriter.GetFullFile(Path + "save/" + name);
            var objects = data[0].Split(' ');
            LoadMap(objects[0], false);

            var playerNum = int.Parse(objects[1]);
            for (var i = 2; i < playerNum + 2; i++)
            {
                objects = data[i].Split(' ');
                GameState.Current.Resources.Add((Players) int.Parse(objects[0]), new Dictionary<Resources, int>());
                GameState.Current.Resources[(Players) int.Parse(objects[0])]
                    .Add(Resources.Gold, int.Parse(objects[1]));
                GameState.Current.Resources[(Players) int.Parse(objects[0])]
                    .Add(Resources.Stone, int.Parse(objects[2]));
            }

            for (var i = playerNum + 2; i < data.Length; i++)
            {
                objects = data[i].Split(' ');
                if ((UnitTypes) int.Parse(objects[1]) == UnitTypes.GoldMine || (UnitTypes) int.Parse(objects[1]) == UnitTypes.Quarry ||
                    (UnitTypes) int.Parse(objects[1]) == UnitTypes.SacredStar || (UnitTypes) int.Parse(objects[1]) == UnitTypes.Rock)
                {
                    GameState.Current.AddBuilding((Players) int.Parse(objects[0]),
                        new Vector2(float.Parse(objects[2]), float.Parse(objects[3])),
                        (UnitTypes) int.Parse(objects[1]),
                        float.Parse(objects[4]));
                }
                else if ((UnitTypes) int.Parse(objects[1]) == UnitTypes.Gate || (UnitTypes) int.Parse(objects[1]) == UnitTypes.Wall ||
                         (UnitTypes) int.Parse(objects[1]) == UnitTypes.Village)
                {
                    GameState.Current.AddBuilding((Players) int.Parse(objects[0]),
                        new Vector2(float.Parse(objects[2]), float.Parse(objects[3])),
                        (UnitTypes) int.Parse(objects[1]),
                        float.Parse(objects[4]),
                        int.Parse(objects[5]));
                }
                else if ((UnitTypes) int.Parse(objects[1]) == UnitTypes.Hero)
                {
                    LoadHero(objects);
                }
                else
                {
                    GameState.Current.AddUnit((Players) int.Parse(objects[0]),
                        new Vector2(float.Parse(objects[2]), float.Parse(objects[3])),
                        (UnitTypes) int.Parse(objects[1]),
                        float.Parse(objects[4]),
                        int.Parse(objects[5]));
                }
            }

        }

        private void LoadHero(IReadOnlyList<string> objects)
        {
            var levelManager = new LevelManager(int.Parse(objects[7]), int.Parse(objects[8]));
            var skillManager = new SkillManager((Players)int.Parse(objects[0]), mContent);
            for (var j = 0; j < int.Parse(objects[9]); j++)
            {
                skillManager.AddSkillPoint();
            }

            for (var j = 10; j < objects.Count - 1; j++)
            {
                var skill = objects[j].Split('/');
                skillManager.SetSkillLevel((Skills)int.Parse(skill[0]), int.Parse(skill[1]));
            }

            GameState.Current.AddHero((Players)int.Parse(objects[0]),
                new Vector2(float.Parse(objects[2]), float.Parse(objects[3])),
                float.Parse(objects[4]),
                int.Parse(objects[6]),
                int.Parse(objects[5]),
                levelManager,
                skillManager);
        }
    }
}
