using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.KI;
using KingsOfSacredStar.Screen;
using KingsOfSacredStar.Sound;
using KingsOfSacredStar.Statistic;
using KingsOfSacredStar.World.Unit;
using KingsOfSacredStar.World.Unit.Buildings;
using KingsOfSacredStar.World.Unit.PathFinding;
using KingsOfSacredStar.World.Unit.Skills;
using KingsOfSacredStar.World.Unit.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.World
{

    /// <summary>
    /// This class contains all elements of a game needed to save/load it and synchronise it in a network.
    /// Currently it only acts as a container for the game.
    /// </summary>
    internal sealed class GameState
    {
        private TileStates[,] mMap;
        public PathFindingZones mPathZones;
        public readonly Dictionary<Players, float> mDamageFactor;
        public Point MapSize => new Point(mMap.GetLength(0) * mGridSize / 2, mMap.GetLength(1) * mGridSize / 2);

        public Trapeze MapPoints => new Trapeze(new Vector2(-mMap.GetLength(0) * mGridSize / 2f, -mMap.GetLength(1) * mGridSize / 2f),
            new Vector2(mMap.GetLength(0) * mGridSize / 2f, -mMap.GetLength(1) * mGridSize / 2f),
            new Vector2(-mMap.GetLength(0) * mGridSize / 2f, mMap.GetLength(1) * mGridSize / 2f),
            new Vector2(mMap.GetLength(0) * mGridSize / 2f, mMap.GetLength(1) * mGridSize / 2f));
        private readonly AKi mEnemy;
        private readonly int mGridSize;
        private readonly Camera mCamera;
        private CollisionHandler mCollision;
        public HeroRespawnManager HeroRespawnManager { get; }
        public readonly Queue<IUnit> mUnitsToAddNextTick = new Queue<IUnit>();
        public Dictionary<Players, List<IUnit>> UnitsByPlayer { get; }
        public Dictionary<Players, Village> VillagesByPlayer { get; }
        public SpatialStructuredUnits SpatialUnitsByPlayer { get; }
        public Dictionary<Players, List<IUnit>> BuildingsByPlayer { get; }
        public Dictionary<Players, Hero> HeroesByPlayer { get; }
        public Dictionary<ModelManager.Model, List<IUnit>> UnitsByModel { get; }
        public Dictionary<Players, Dictionary<Resources, int>> Resources { get; }
        public static GameState Current { get; set; }
        internal Dictionary<Players, Vector2> VillagePos { get; }
        public Vector2 VillagePosOffset(Players player) {
            return new Vector2((float)(VillagePos[player].X - 1.5 * Math.Sign(VillagePos[player].X)), VillagePos[player].Y);
        }

        public HashSet<IUnit> SelectedEntities { get; } = new HashSet<IUnit>();
        public bool IsPaused { get; set; }
        private readonly SoundEffectManager mBlockedSound;
        private readonly Statistics mStatistics;
        public Dictionary<Players, Dictionary<string, int>> StatisticsByPlayer { get; }
        private int mLastGamePlaytime;
        private int mGameStartTime;
        private bool mFirstFewSec;
        private readonly ThreadLocal<PathFinder> mPathFinderInstances;
        public PathFinder PathFinder => mPathFinderInstances.Value;
        private readonly ContentManager mContent;
        private static SoundEffectManager sSoundEffectManager;

        public static void Init(SoundEffectManager soundEffectManager)
        {
            sSoundEffectManager = soundEffectManager;
        }

        public GameState(Camera camera, int gridSize, ContentManager content)
        {
            mGridSize = gridSize;
            mCamera = camera;
            UnitsByPlayer = new Dictionary<Players, List<IUnit>>();
            SpatialUnitsByPlayer = new SpatialStructuredUnits(mGridSize);
            BuildingsByPlayer = new Dictionary<Players, List<IUnit>>();
            HeroesByPlayer = new Dictionary<Players, Hero>();
            UnitsByModel = new Dictionary<ModelManager.Model, List<IUnit>>();
            Resources = new Dictionary<Players, Dictionary<Resources, int>>();
            VillagePos = new Dictionary<Players, Vector2>();
            VillagesByPlayer = new Dictionary<Players, Village>();
            mMap = new TileStates[0, 0];
            mPathZones = new PathFindingZones(0, 0, mGridSize);
            IsPaused = true;
            mCollision = new CollisionHandler(gridSize, UnitsByPlayer, IsObstructed);
            mEnemy = new DummyKi();
            HeroRespawnManager = new HeroRespawnManager();
            mDamageFactor = new Dictionary<Players, float>();
            foreach (var player in PlayerConstants.sPlayers)
            {
                mDamageFactor.Add(player, 1f);
            }
            mBlockedSound = new SoundEffectManager(content, "sounds/Logo_miss");
            mStatistics = new Statistics(content, null, null);
            StatisticsByPlayer = new Dictionary<Players, Dictionary<string, int>>();
            InitStatisticsByPlayer(Players.Player);
            InitStatisticsByPlayer(Players.Ai);
            mPathFinderInstances = new ThreadLocal<PathFinder>(() => new PathFinder(IsObstructed, gridSize));
            mContent = content;
        }

        public bool IsSelected(IUnit unit)
        {
            return SelectedEntities.Contains(unit);
        }

        /// <summary>
        /// For path-finding and collision
        /// </summary>
        private bool IsObstructed(Vector2 position)
        {
            var x = (int) (Math.Floor(position.X / mGridSize) + 1 + (mMap.GetLength(0) / 2f));
            var y = (int) (Math.Floor(position.Y / mGridSize) + 1 + (mMap.GetLength(1) / 2f));

            var fieldState = GetGameFieldState(x, y);

            return fieldState == TileStates.Blocked;
        }

        private void Remove(IUnit unit, Players playerId)
        {
            if (unit is ABuilding building)
            {
                ChangeFieldState(TileStates.Free, building.BlockedFields());
                BuildingsByPlayer[playerId].Remove(unit);
            }
            else
            {
                foreach (var hero in HeroesByPlayer.Values)
                {
                    if (hero.AttackedUnits.Contains(unit))
                    {
                        hero.KilledAUnit();
                        hero.AttackedUnits.Remove(unit as IDamageableUnit);
                    }
                }

                UnitsByPlayer[playerId].Remove(unit);
            }

            SelectedEntities.Remove(unit);
            UnitsByModel[unit.ModelType].Remove(unit);
        }

        private void AddQueuedUnits()
        {
            while (mUnitsToAddNextTick.Count != 0)
            {
                var unit = mUnitsToAddNextTick.Dequeue();
                UnitsByPlayer[unit.Owner].Add(unit);
                if (!UnitsByModel.ContainsKey(unit.ModelType))
                {
                    UnitsByModel[unit.ModelType] = new List<IUnit>();
                }

                UnitsByModel[unit.ModelType].Add(unit);
            }
        }

        private void AttackNearbyEnemyUnits(Players player, Players otherPlayer)
        {
            foreach (var attackingUnit in UnitsByPlayer[player].OfType<IAttackingUnit>())
            {
                if (attackingUnit.HasTarget())
                {
                    continue;
                }

                var target = SpatialUnitsByPlayer.NearestUnit(otherPlayer,
                    attackingUnit.Position,
                    attackingUnit.GetRange(),
                    unit => Math.Abs(AttackModifiers.GetModifier(attackingUnit.UnitType, unit.UnitType)) >= float.Epsilon);
                if (target != null && target is IDamageableUnit attackedUnit)
                {
                    attackingUnit.SetTarget(attackedUnit, false);
                }
            }
        }

        private void TriggerAutoDefense(GameTime gameTime)
        {
            // Don't check every tick for enemy units
            if (gameTime.TotalGameTime.Ticks % 5 != 0) return;
            // units attack nearest enemy if they don't already have a target
            foreach (var player in UnitsByPlayer.Keys)
            {
                foreach (var otherPlayer in UnitsByPlayer.Keys)
                {
                    if (otherPlayer == Players.Global || otherPlayer == player)
                    {
                        continue;
                    }

                    AttackNearbyEnemyUnits(player, otherPlayer);
                }
            }
        }

        private bool QueueForRemovalIfRequired(IUnit unit, ICollection<IUnit> unitsToRemove)
        {
            if (unit is IDespawningUnit despawningUnit && despawningUnit.ForRemoval)
            {
                switch (unit.UnitType)
                {
                    case UnitTypes.BatteringRam: sSoundEffectManager.Play(0); break;
                    case UnitTypes.Bowman: sSoundEffectManager.Play(1); break;
                    case UnitTypes.Cavalry: sSoundEffectManager.Play(2); break;
                    case UnitTypes.Swordsman: sSoundEffectManager.Play(3); break;
                    case UnitTypes.Hero: sSoundEffectManager.Play(4); break;
                }
                unitsToRemove.Add(unit);
                if (unit.UnitType == UnitTypes.Hero || unit.UnitType == UnitTypes.Swordsman ||
                    unit.UnitType == UnitTypes.Cavalry || unit.UnitType == UnitTypes.BatteringRam
                    || unit.UnitType == UnitTypes.Bowman)
                {
                    if (unit.Owner == Players.Player)
                    {
                        StatisticsByPlayer[unit.Owner]["LostTroops"] += 1;
                        StatisticsByPlayer[Players.Ai]["KilledTroops"] += 1;

                    }
                    else
                    {
                        StatisticsByPlayer[unit.Owner]["LostTroops"] += 1;
                        StatisticsByPlayer[Players.Player]["KilledTroops"] += 1;
                    }
                }

                return true;
            }

            return false;
        }

        private static void QueueOwnershipChange(IUnit unit, Players player, IDictionary<Players, List<IUnit>> unitsToChangePlayer)
        {
            if (unit.Owner != player)
            {
                if (!unitsToChangePlayer.ContainsKey(player))
                {
                    unitsToChangePlayer.Add(player, new List<IUnit>());
                }

                unitsToChangePlayer[player].Add(unit);

            }
        }

        private void RunUnitLifecycle(GameTime time)
        {
            var unitsToChangePlayer = new Dictionary<Players, List<IUnit>>();
            var unitsToRemove = new List<IUnit>();
            foreach (var player in PlayerConstants.sPlayers)
            {
                if (!UnitsByPlayer.TryGetValue(player, out var nonBuildings))
                {
                    nonBuildings = new List<IUnit>(0);
                }
                foreach (var unit in nonBuildings.Concat(BuildingsByPlayer[player]))
                {

                    if (QueueForRemovalIfRequired(unit, unitsToRemove))
                    {
                        continue;
                    }

                    unit.Update(time);

                    QueueOwnershipChange(unit, player, unitsToChangePlayer);
                }

                UpdateResources(player);
            }

            ApplyOwnershipChange(unitsToChangePlayer);

            mCollision.Update();

            RemoveQueuedUnits(unitsToRemove);
        }

        private void RemoveQueuedUnits(IEnumerable<IUnit> unitsToRemove)
        {
            foreach (var unit in unitsToRemove)
            {
                foreach (var player in UnitsByPlayer.Keys)
                {
                    Remove(unit, player);
                }
            }
        }

        private void ApplyOwnershipChange(Dictionary<Players, List<IUnit>> unitsToChangePlayer)
        {
            foreach (var oldOwner in unitsToChangePlayer.Keys)
            {
                foreach (var unit in unitsToChangePlayer[oldOwner])
                {
                    if (oldOwner != Players.Global)
                    {
                        GetListForOwner(unit, oldOwner).Remove(unit);
                    }

                    if (unit.Owner != Players.Global)
                    {
                        var list = GetListForOwner(unit, unit.Owner);
                        if (!list.Contains(unit))
                        {
                            list.Add(unit);
                        }
                    }
                }
            }
        }

        private List<IUnit> GetListForOwner(IUnit unit, Players owner)
        {
            return unit is ABuilding ? BuildingsByPlayer[owner] : UnitsByPlayer[owner];
        }

        private double mLastTimeResources;

        public bool Update(GameTime time)
        {
            if (IsPaused)
            {
                mLastTimeResources = time.TotalGameTime.TotalMilliseconds - (time.TotalGameTime.TotalMilliseconds - mLastTimeResources);
                foreach (var player in BuildingsByPlayer.Keys)
                {
                    foreach (var unit in BuildingsByPlayer[player])
                    {
                        if (unit.UnitType == UnitTypes.Quarry || unit.UnitType == UnitTypes.GoldMine)
                        {
                            unit.IsPaused(time);
                        }
                    }
                }
                return false;
            }

            if (mLastTimeResources + 5000 < time.TotalGameTime.TotalMilliseconds)
            {
                foreach (var player in Resources.Keys)
                {
                    Resources[player][Unit.Resources.Gold]++;
                    Resources[player][Unit.Resources.Stone]++;
                }

                mLastTimeResources = time.TotalGameTime.TotalMilliseconds;
            }

            AddQueuedUnits();

            SpatialUnitsByPlayer.Update(UnitsByPlayer);

            TriggerAutoDefense(time);

            RunUnitLifecycle(time);


            if(CheckVictory())
                return true;
            HeroRespawnManager.Update();
            mEnemy.Update(time);

            InGameTime(time);
            InGameStatistics();
            return false;
        }

        private bool IsFieldOnGameField(int x, int y)
        {
            return x >= 0 &&
                   y >= 0 &&
                   x < mMap.GetLength(0) &&
                   y < mMap.GetLength(1);
        }

        private TileStates GetGameFieldState(int x, int y)
        {
            return !IsFieldOnGameField(x, y) ? TileStates.Blocked : mMap[x, y];
        }

        private void SetGameFieldState(int x, int y, TileStates state)
        {
            if (!IsFieldOnGameField(x, y))
            {
                return;
            }

            mMap[x, y] = state;

            switch (state)
            {
                case TileStates.Blocked:
                    mPathZones.Block(x, y);
                    break;
                case TileStates.Free:
                    mPathZones.Unblock(x, y);
                    break;
            }
        }

        public void SelectEntities(Trapeze select, Players playerId, UnitTypes type = UnitTypes.Wall)
        {
            SelectedEntities.Clear();
            foreach (var unit in UnitsByPlayer[playerId])
            {
                if (select.Contains(unit.Position) && (unit.UnitType == type || type == UnitTypes.Wall))
                {
                    SelectedEntities.Add(unit);
                }
            }
        }

        public void SelectEntity(Point entityGameFieldPosition, Players playerId)
        {
            SelectedEntities.Clear();
            var unit = GetIntersectingUnits(entityGameFieldPosition, playerId).LastOrDefault();
            if (unit != null)
            {
                SelectedEntities.Add(unit);
            }
        }

        public IDamageableUnit GetDamageableUnitAtPosition(Point entityGameFieldPosition, Players playerId)
        {
            return GetIntersectingUnits(entityGameFieldPosition, playerId).OfType<IDamageableUnit>().FirstOrDefault();
        }

        public bool AttackUnitsAtPosition(Point entityGameFieldPosition)
        {
            IDamageableUnit damageableUnit = null;
            foreach (var key in UnitsByPlayer)
            {
                if (key.Key != Players.Global && key.Key != Players.Player)
                {
                    damageableUnit = GetDamageableUnitAtPosition(entityGameFieldPosition, key.Key);
                }
            }

            if (damageableUnit == null) return false;
            var isTarget = false;
            foreach (var entity in SelectedEntities.OfType<IAttackingUnit>())
            {
                entity.SetTarget(damageableUnit, true);
                isTarget = true;
            }

            return isTarget;

        }

        private IEnumerable<IUnit> GetIntersectingUnits(Point entityGameFieldPosition, Players playerId)
        {
            var startPoint = mCamera.Position + new Vector3(0, -1, 0);
            var endPoint = new Vector3(entityGameFieldPosition.X, 0, entityGameFieldPosition.Y);
            var pickingRay = new Ray(startPoint, endPoint - startPoint);
            var minIntersectionDistance = float.MaxValue;
            foreach (var unit in UnitsByPlayer[playerId].Concat(BuildingsByPlayer[playerId]))
            {
                var intersectionDistance = unit.ModelType.PickingRayIntersection(pickingRay, unit.RenderPosition);
                if (intersectionDistance == null || intersectionDistance >= minIntersectionDistance) continue;
                if (!unit.ModelType.PickingRayHittingMeshes(pickingRay, unit.Position)) continue;
                minIntersectionDistance = intersectionDistance.Value;
                yield return unit;
            }
        }

        public void MoveSelectedUnits(float x, float y, Predicate<IMovableUnit> predicate = null)
        {
            foreach (var unit in SelectedEntities)
            {
                if (unit is IMovableUnit movableUnit && (predicate == null || predicate(movableUnit)))
                {
                    movableUnit.SetMovingTarget(new Vector2(x, y)).ContinueWith(task =>
                    {
                        if (!task.Result)
                        {
                            mBlockedSound.Play(0);
                        }
                    });
                }
            }
        }

        public void AddHero(Players playerId, Vector2 pos, float rot, int mana, int health, LevelManager levelManager, SkillManager skillManager)
        {
            var hero = new Hero(playerId, pos, rot, mGridSize, mana, health, levelManager, skillManager, mContent);
            HeroesByPlayer[playerId] = hero;
            if (!UnitsByPlayer.ContainsKey(playerId))
            {
                UnitsByPlayer.Add(playerId, new List<IUnit>());
            }

            UnitsByPlayer[playerId].Add(hero);
            StatisticsByPlayer[playerId]["CreateTroops"] += 1;
            if (!UnitsByModel.ContainsKey(hero.ModelType))
            {
                UnitsByModel.Add(hero.ModelType, new List<IUnit>());
            }

            UnitsByModel[hero.ModelType].Add(hero);
        }

        public void AddUnit(Players playerId, Vector2 position, UnitTypes name, float rot = 0f, int health = 0)
        {
            IUnit unit;
            switch (name)
            {
                case UnitTypes.Arrow:
                    unit = new Arrow(playerId, position, rot, mGridSize, position);
                    break;
                case UnitTypes.Swordsman:
                    unit = new Swordsman(playerId, position, rot, mGridSize);
                    break;
                case UnitTypes.Cavalry:
                    unit = new Cavalry(playerId, position, rot, mGridSize);
                    break;
                case UnitTypes.Hero:
                    var hero = new Hero(playerId, position, rot, mGridSize, mContent);
                    unit = hero;
                    HeroesByPlayer[playerId] = hero;
                    break;
                case UnitTypes.Bowman:
                    unit = new Bowman(playerId, position, rot, mGridSize);
                    break;
                case UnitTypes.BatteringRam:
                    unit = new BatteringRam(playerId, position, rot, mGridSize);
                    break;
                default:
                    return;
            }

            if (unit is IDamageableUnit damageableUnit && health != 0)
            {
                damageableUnit.Health = health;
            }

            if (!UnitsByPlayer.ContainsKey(playerId))
            {
                UnitsByPlayer.Add(playerId, new List<IUnit>());
            }

            UnitsByPlayer[playerId].Add(unit);
            if (unit.UnitType == UnitTypes.Hero || unit.UnitType == UnitTypes.Swordsman || unit.UnitType == UnitTypes.Cavalry
                || unit.UnitType == UnitTypes.BatteringRam || unit.UnitType == UnitTypes.Bowman)
            {
                StatisticsByPlayer[playerId]["CreateTroops"] += 1;
            }
            if (!UnitsByModel.ContainsKey(unit.ModelType))
            {
                UnitsByModel.Add(unit.ModelType, new List<IUnit>());
            }

            UnitsByModel[unit.ModelType].Add(unit);
        }

        private bool AreFieldsOccupied(params Point[][] points)
        {
            var halfGameFieldWidth = mMap.GetLength(0) / 2;
            var halfGameFieldHeight = mMap.GetLength(1) / 2;

            var isOccupied = false;
            foreach (var pointGroup in points)
            {
                foreach (var offset in pointGroup)
                {
                    var x = halfGameFieldWidth + offset.X;
                    var y = halfGameFieldHeight + offset.Y;
                    isOccupied |= GetGameFieldState(x, y) != TileStates.Free;
                    if (isOccupied)
                    {
                        break;
                    }
                }
            }

            return isOccupied;
        }

        public void ChangeFieldState(TileStates newState, params Point[] offsets)
        {
            var halfGameFieldWidth = mMap.GetLength(0) / 2;
            var halfGameFieldHeight = mMap.GetLength(1) / 2;
            foreach (var offset in offsets)
            {
                var x = halfGameFieldWidth + offset.X;
                var y = halfGameFieldHeight + offset.Y;
                SetGameFieldState(x, y, newState);
            }
        }

        public bool IsValidBuildingPlace(Players playerId, Vector2 position, UnitTypes name, float rotation)
        {
            ABuilding unit;
            switch (name)
            {
                case UnitTypes.Gate:
                    unit = new Gate(playerId, position, rotation, mGridSize);
                    break;
                case UnitTypes.Wall:
                    unit = new Wall(playerId, position, rotation, mGridSize);
                    break;
                default:
                    return false;
            }

            var blockedFields = unit.BlockedFields();
            var passableFields = unit.PassableFields();

            var validBuildPlace = !AreFieldsOccupied(blockedFields, passableFields);

            const int buildingRangeToEnemy = 5 * GameScreen.GridSize;

            foreach (var otherPlayer in UnitsByPlayer.Keys)
            {
                if (otherPlayer == Players.Global || otherPlayer == playerId)
                {
                    continue;
                }
                if (!validBuildPlace)
                {
                    break;
                }

                validBuildPlace &= SpatialUnitsByPlayer.UnitsInRange(otherPlayer, position * GameScreen.GridSize, buildingRangeToEnemy).Count == 0;
            }

            return validBuildPlace;
        }


        public bool AddBuilding(Players playerId, Vector2 position, UnitTypes name, float rotation = 0f, int health = 0)
        {
            ABuilding unit;

            switch (name)
            {
                case UnitTypes.Gate:
                    unit = new Gate(playerId, position, rotation, mGridSize);
                    break;
                case UnitTypes.Wall:
                    unit = new Wall(playerId, position, rotation, mGridSize);
                    break;
                case UnitTypes.SacredStar:
                    unit = new SacredStar(playerId, position, mGridSize);
                    break;
                case UnitTypes.Village:
                    unit = new Village(playerId, position, rotation, mGridSize);
                    VillagePos.Add(playerId, position);
                    VillagesByPlayer.Add(playerId,(Village)unit);
                    break;
                case UnitTypes.GoldMine:
                    unit = new Mine(playerId, position, rotation, true, mGridSize, Resources);
                    break;
                case UnitTypes.Quarry:
                    unit = new Mine(playerId, position, rotation, false, mGridSize, Resources);
                    break;
                case UnitTypes.Rock:
                    unit = new Rock(position, mGridSize, rotation);
                    break;
                default:
                    return false;
            }

            if (unit is IDamageableUnit damageableUnit && health != 0)
            {
                damageableUnit.Health = health;
            }

            var blockedFields = unit.BlockedFields();
            var passableFields = unit.PassableFields();

            if (AreFieldsOccupied(blockedFields, passableFields))
            {
                return false;
            }

            ChangeFieldState(TileStates.Blocked, blockedFields);
            ChangeFieldState(TileStates.Passable, passableFields);

            if (!BuildingsByPlayer.ContainsKey(playerId))
            {
                BuildingsByPlayer.Add(playerId, new List<IUnit>());
            }

            if ((unit.UnitType == UnitTypes.GoldMine || unit.UnitType == UnitTypes.Quarry ||
                 unit.UnitType == UnitTypes.SacredStar) && !BuildingsByPlayer[Players.Global].Contains(unit))
            {
                BuildingsByPlayer[Players.Global].Add(unit);
            }
            BuildingsByPlayer[playerId].Add(unit);
            if (!UnitsByModel.ContainsKey(unit.ModelType))
            {
                UnitsByModel.Add(unit.ModelType, new List<IUnit>());
            }

            UnitsByModel[unit.ModelType].Add(unit);

            return true;

        }

        public void GenerateEmptyFreeMap(int sizeX, int sizeY)
        {
            mMap = new TileStates[sizeX, sizeY];
            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    mMap[x, y] = TileStates.Free;
                }

            }

            mPathZones = new PathFindingZones(sizeX, sizeY, mGridSize);
        }

        private void UpdateResources(Players playerId)
        {
            if (!Resources.ContainsKey(playerId))
            {
                Resources[playerId] = new Dictionary<Resources, int>
                {
                    [Unit.Resources.Gold] = 0,
                    [Unit.Resources.Stone] = 0
                };
            }
        }

        public void CreateCollisionManger(int gridSize)
        {
            mCollision = new CollisionHandler(gridSize, UnitsByPlayer, IsObstructed);
        }

        private void InitStatisticsByPlayer(Players playerId)
        {
            if (!StatisticsByPlayer.ContainsKey(playerId))
            {
                StatisticsByPlayer[playerId] = new Dictionary<string, int>
                {
                    ["Gold"] = 0,
                    ["Stone"] = 0,
                    ["KilledTroops"] = 0,
                    ["LostTroops"] = 0,
                    ["CreateTroops"] = 0,
                    ["Won"] = 0,
                    ["Lost"] = 0,
                    ["LastGamePlaytime"] = 0
                };
            }
        }

        private bool CheckVictory()
        {
            foreach (var village in VillagesByPlayer)
            {
                if (village.Value.Health <= 0)
                {
                    if (village.Key == Players.Player)
                    {
                        StatisticsByPlayer[Players.Player]["Lost"] = 1;
                        StatisticsByPlayer[Players.Ai]["Won"] = 1;
                        mStatistics.mStatisticsNew["Lost"] = 1;
                    }
                    else
                    {
                        StatisticsByPlayer[Players.Player]["Won"] = 1;
                        StatisticsByPlayer[Players.Ai]["Lost"] = 1;
                        mStatistics.mStatisticsNew["Won"] = 1;
                    }

                    return true;
                }
            }
            return false;
        }

        private void InGameStatistics()
        {
            foreach (var player in StatisticsByPlayer.Keys)
            {
                StatisticsByPlayer[player]["LastGamePlaytime"] = mLastGamePlaytime;
            }
            foreach (var str in StatisticsByPlayer[Players.Player].Keys.ToList())
            {

                mStatistics.mStatisticsNew[str] = StatisticsByPlayer[Players.Player][str];

            }
        }

        private void InGameTime(GameTime time)
        {
            int timeDifference;
            if (mGameStartTime <= 0)
            {
                mGameStartTime = time.TotalGameTime.Seconds;
                timeDifference = 60 - mGameStartTime;
                mFirstFewSec = true;
            }
            else
            {
                timeDifference = 60 - mGameStartTime;
            }
            var timeCounter = time.TotalGameTime.Seconds;
            if (!mFirstFewSec)
            {
                if (timeCounter == 0 && (mLastGamePlaytime - timeDifference) % 60 == 59)
                {
                    mLastGamePlaytime += 1;
                }
                else
                {
                    mLastGamePlaytime += timeCounter - (mLastGamePlaytime - timeDifference) % 60;
                }
            }
            else
            {
                if (timeCounter - mGameStartTime >= 0)
                {
                    mLastGamePlaytime = timeCounter - mGameStartTime;
                }
                else
                {
                    mLastGamePlaytime += 1;
                    mFirstFewSec = false;
                }
            }
        }

        public void SignalModelUpdate(ModelManager.Model oldModel, IUnit unit)
        {
            UnitsByModel[oldModel].Remove(unit);
            if (!UnitsByModel.ContainsKey(unit.ModelType))
            {
                UnitsByModel[unit.ModelType] = new List<IUnit>();
            }

            UnitsByModel[unit.ModelType].Add(unit);
        }

        public void SaveStatistics() => mStatistics.WriteStatistics();
    }
}