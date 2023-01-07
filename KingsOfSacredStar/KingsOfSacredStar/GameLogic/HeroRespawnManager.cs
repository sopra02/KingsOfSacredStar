using System.Collections.Generic;
using KingsOfSacredStar.World;

namespace KingsOfSacredStar.GameLogic
{
    internal sealed class HeroRespawnManager
    {
        private readonly Dictionary<Players, int> mPlayerRespawnCount = new Dictionary<Players, int>();
        private readonly Dictionary<Players, int> mPlayerTickMulti = new Dictionary<Players, int>();
        private const int TicksToWait = 500;

        public void Update()
        {
            foreach (var hero in GameState.Current.HeroesByPlayer.Values)
            {
                if (hero.Health > 0) continue;
                if (!mPlayerRespawnCount.ContainsKey(hero.Owner))
                {
                    mPlayerRespawnCount.Add(hero.Owner, 0);
                    mPlayerTickMulti.Add(hero.Owner, 1);
                }

                if (mPlayerRespawnCount[hero.Owner] >= TicksToWait * mPlayerTickMulti[hero.Owner])
                {
                    mPlayerTickMulti[hero.Owner]++;
                    hero.Respawn(GameState.Current.VillagePosOffset(hero.Owner));
                    GameState.Current.UnitsByPlayer[hero.Owner].Add(hero);
                    mPlayerRespawnCount[hero.Owner] = 0;
                }
                else
                {
                    if (mPlayerRespawnCount[hero.Owner] == 0)
                    {
                        GameState.Current.UnitsByPlayer[hero.Owner].Remove(hero);
                    }
                    mPlayerRespawnCount[hero.Owner]++;
                }
            }
        }

        public int? GetRespawnSecondsLeft(Players player)
        {
            int? secondsLeft = null;
            if (mPlayerRespawnCount.ContainsKey(player) && mPlayerRespawnCount[player] > 0)
            {
                secondsLeft = (TicksToWait * mPlayerTickMulti[player] - mPlayerRespawnCount[player]) / 60;
            }

            return secondsLeft;
        }
    }
}
