using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// helper used by <see cref="IRoadManager"/> to manage a single road network<br/>
    /// it holds the pathfinding for both regular and blocked road pathing<br/>
    /// also it counts as a <see cref="IStructure"/> and can therefore be queried and checked against using <see cref="IStructureManager"/><br/>
    /// this basic RoadNetwork is not visualized in any way, it is only a logical helper<br/>
    /// for a visualized network inherit this class and implement <see cref="setPoint(Vector2Int, Road)"/> and <see cref="checkPoint(Vector2Int)"/><br/>
    /// examples of this can be found in <see cref="TilemapRoadNetwork"/> and <see cref="TerrainRoadNetwork"/>
    /// </summary>
    public class RoadNetwork : IStructure, ILayerDependency
    {
        public event Action<PointsChanged<IStructure>> PointsChanged;

        public Road Road { get; private set; }

        public GridPathfindingBase DefaultPathfinding { get; private set; }
        public GridPathfindingBase BlockedPathfinding { get; private set; }
        public List<Vector2Int> Blocked { get; private set; }

        public StructureReference StructureReference { get; set; }

        public bool IsDestructible => Road?.IsDestructible ?? true;
        public bool IsMovable => Road?.IsMovable ?? true;
        public bool IsDecorator => false;
        public bool IsWalkable => true;

        public int Level => Road == null ? _level : Road.Level.Value;
        public string Key => Road?.Key ?? "ROD";

        private int _level;
        private GridLinks _links;

        public RoadNetwork(GridPathfindingSettings pathfindingSettings, Road road, int level = 0)
        {
            Road = road;

            _level = level;
            _links = new GridLinks();

            DefaultPathfinding = pathfindingSettings.Create();
            BlockedPathfinding = pathfindingSettings.Create();
            Blocked = new List<Vector2Int>();

            StructureReference = new StructureReference(this);
        }

        public virtual void Initialize()
        {
            Dependencies.Get<IStructureManager>().RegisterStructure(this, true);
        }

        public virtual void Dispose()
        {
            DefaultPathfinding.Dispose();
            BlockedPathfinding.Dispose();
        }

        public void Calculate(int maxCalculations = PathQuery.DEFAULT_MAX_CALCULATIONS)
        {
            DefaultPathfinding.Calculate(maxCalculations);
            BlockedPathfinding.Calculate(maxCalculations);
        }

        public string GetName() => Road ? Road.Name : "Roads";

        public IEnumerable<Vector2Int> GetPoints() => DefaultPathfinding.GetPoints();
        public bool HasPoint(Vector2Int point) => DefaultPathfinding.HasPoint(point);

        public void Add(IEnumerable<Vector2Int> points, UndoRedoActions undoRedoActions = null) => Add(points, null, undoRedoActions);
        public List<Vector2Int> Add(IEnumerable<Vector2Int> points, Road road, UndoRedoActions undoRedoActions = null)
        {
            var structureManager = Dependencies.Get<IStructureManager>();
            if (road == null)
                road = Road;

            var changedPoints = points.Where(p => !DefaultPathfinding.HasPoint(p) && structureManager.CheckAvailability(p, Level, road)).ToList();
            if (changedPoints.Count == 0)
                return null;

            foreach (var point in changedPoints)
            {
                setPoint(point, road);

                DefaultPathfinding.Add(point);
                if (!Blocked.Contains(point))
                    BlockedPathfinding.Add(point);
            }

            structureManager.Remove(changedPoints, Level, true, undoRedoActions: undoRedoActions);

            undoRedoActions?.Add(new RoadAddition(this, road.Key, changedPoints));
            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), changedPoints));

            return changedPoints;
        }
        public IEnumerable<Vector2Int> Remove(IEnumerable<Vector2Int> points, UndoRedoActions undoRedoActions = null)
        {
            var changedPoints = new List<Vector2Int>();
            var changedRoads = new Dictionary<string, List<Vector2Int>>();

            foreach (var point in points)
            {
                if (!checkPoint(point))
                    continue;

                if (undoRedoActions != null && TryGetRoad(point, out var road, out var _))
                {
                    if (!changedRoads.ContainsKey(road.Key))
                        changedRoads.Add(road.Key, new List<Vector2Int>());
                    changedRoads[road.Key].Add(point);
                }

                setPoint(point, null);

                DefaultPathfinding.Remove(point);
                BlockedPathfinding.Remove(point);

                changedPoints.Add(point);
            }

            if (changedPoints.Count == 0)
                return changedPoints;

            undoRedoActions?.Add(new RoadRemoval(this, changedRoads));
            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, changedPoints, Enumerable.Empty<Vector2Int>()));

            return changedPoints;
        }

        public void Register(IEnumerable<Vector2Int> points)
        {
            foreach (var point in points)
            {
                DefaultPathfinding.Add(point);
                if (!Blocked.Contains(point))
                    BlockedPathfinding.Add(point);
            }
        }
        public void Deregister(IEnumerable<Vector2Int> points)
        {
            foreach (var point in points.Where(p => !checkPoint(p)))
            {
                DefaultPathfinding.Remove(point);
                BlockedPathfinding.Remove(point);
            }
        }

        public void RegisterLink(IGridLink link)
        {
            DefaultPathfinding.AddLink(link);
            _links.Add(link);
        }
        public void DeregisterLink(IGridLink link)
        {
            DefaultPathfinding.RemoveLink(link);
            _links.Remove(link);
        }
        public IEnumerable<IGridLink> GetLinks(Vector2Int start) => _links.Get(start);
        public IGridLink GetLink(Vector2Int start, Vector2Int end) => _links.Get(start, end);

        public void RegisterSwitch(Vector2Int point, RoadNetwork other)
        {
            DefaultPathfinding.AddSwitch(point, other.DefaultPathfinding);
            BlockedPathfinding.AddSwitch(point, other.BlockedPathfinding);
        }
        public void RegisterSwitch(Vector2Int entry, Vector2Int point, Vector2Int exit, RoadNetwork other)
        {
            DefaultPathfinding.AddSwitch(entry, point, exit, other.DefaultPathfinding);
            BlockedPathfinding.AddSwitch(entry, point, exit, other.BlockedPathfinding);
        }

        public void Block(IEnumerable<Vector2Int> points)
        {
            List<Vector2Int> blocked = new List<Vector2Int>();
            foreach (var point in points)
            {
                if (!Blocked.Contains(point))
                    blocked.Add(point);
                Blocked.Add(point);
            }

            foreach (var point in blocked)
            {
                if (DefaultPathfinding.HasPoint(point))
                    BlockedPathfinding.Remove(point);
            }
        }
        public void Unblock(IEnumerable<Vector2Int> points)
        {
            foreach (var point in points)
            {
                if (DefaultPathfinding.HasPoint(point))
                    BlockedPathfinding.Add(point);
            }
        }

        public void BlockTags(IEnumerable<Vector2Int> points, IEnumerable<object> tags) => BlockedPathfinding.BlockTags(points, tags);
        public void UnblockTags(IEnumerable<Vector2Int> points, IEnumerable<object> tags) => BlockedPathfinding.UnblockTags(points, tags);

        public virtual void CheckLayers(IEnumerable<Vector2Int> points)
        {
        }

        public virtual bool TryGetRoad(Vector2Int point, out Road road, out string stage)
        {
            road = null;
            stage = null;

            return HasPoint(point);
        }

        /// <summary>
        /// checks if a point is defined by the network itself<br/>
        /// points registered from the outside return false
        /// </summary>
        /// <param name="point">a point on the map</param>
        /// <returns>true if the network contains the point, false if the point does not exist or has only been registered from outside</returns>
        public bool CheckPoint(Vector2Int point) => checkPoint(point);

        protected virtual void setPoint(Vector2Int point, Road road) { }
        protected virtual bool checkPoint(Vector2Int point) => false;

        protected void onPointsChanged(PointsChanged<IStructure> pointsChanged)
        {
            PointsChanged?.Invoke(pointsChanged);
        }

        #region UndoRedo
        [Serializable]
        public class UndoRedoRoadData
        {
            public string Key;
            public UndoRedoRoadPointsData[] RoadPoints;
        }
        [Serializable]
        public class UndoRedoRoadPointsData
        {
            public string Road;
            public Vector2Int[] Points;
        }
        public abstract class UndoRedoRoadBase : UndoRedoActionBase
        {
            protected UndoRedoRoadData _data;

            protected virtual void add()
            {
                var collection = Dependencies.Get<IStructureManager>().GetStructure(_data.Key) as RoadNetwork;
                var roads = Dependencies.Get<IKeyedSet<Road>>();

                foreach (var roadPoints in _data.RoadPoints)
                {
                    collection.Add(roadPoints.Points, roads.GetObject(roadPoints.Road));
                }
            }

            protected virtual void remove()
            {
                var collection = Dependencies.Get<IStructureManager>().GetStructure(_data.Key) as RoadNetwork;

                foreach (var roadPoints in _data.RoadPoints)
                {
                    collection.Remove(roadPoints.Points);
                }
            }

            public override IEnumerable<Vector2Int> GetPoints() => _data.RoadPoints.SelectMany(rp => rp.Points);

            #region Saving
            public override string SaveData()
            {
                return JsonUtility.ToJson(_data);
            }
            public override void LoadData(string json)
            {
                _data = JsonUtility.FromJson<UndoRedoRoadData>(json);
            }
            #endregion
        }
        public class RoadAddition : UndoRedoRoadBase
        {
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            static void OnLoad()
            {
                UndoRedoTypes.Register("RoadAddition", typeof(RoadAddition));
            }

            public override string Name => "Add Roads";

            public RoadAddition(RoadNetwork structure, string road, List<Vector2Int> points)
            {
                _data = new UndoRedoRoadData()
                {
                    Key = structure.Key,
                    RoadPoints = new UndoRedoRoadPointsData[]
                    {
                        new UndoRedoRoadPointsData()
                        {
                            Road = road,
                            Points = points.ToArray()
                        }
                    }
                };
            }

            public override void Undo() => remove();
            public override void Redo() => add();
        }
        public class RoadRemoval : UndoRedoRoadBase
        {
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            static void OnLoad()
            {
                UndoRedoTypes.Register("RoadRemoval", typeof(RoadRemoval));
            }

            public override string Name => "Remove Roads";

            public RoadRemoval(RoadNetwork structure, Dictionary<string, List<Vector2Int>> roadDict)
            {
                _data = new UndoRedoRoadData()
                {
                    Key = structure.Key,
                    RoadPoints = roadDict.Select(r => new UndoRedoRoadPointsData() { Road = r.Key, Points = r.Value.ToArray() }).ToArray()
                };
            }

            public override void Undo() => add();
            public override void Redo() => remove();
        }
        #endregion
        #region Saving
        [Serializable]
        public class RoadsData
        {
            public string Key;
            public RoadData[] Roads;
        }
        [Serializable]
        public class RoadData
        {
            public string Key;
            public Vector2Int[] Positions;
        }

        public virtual RoadsData SaveData()
        {
            return new RoadsData()
            {
                Key = Road?.Key,
                Roads = new RoadData[]
                {
                    new RoadData()
                    {
                        Key=string.Empty,
                        Positions=DefaultPathfinding.GetPoints().ToArray()
                    }
                }
            };
        }

        public virtual void LoadData(RoadsData roadsData)
        {
            var oldPoints = GetPoints().ToList();

            DefaultPathfinding.Clear();
            BlockedPathfinding.Clear();

            if (roadsData.Roads != null && roadsData.Roads.Length > 0)
            {
                foreach (var point in roadsData.Roads[0].Positions)
                {
                    DefaultPathfinding.Add(point);
                    if (!Blocked.Contains(point))
                        BlockedPathfinding.Add(point);
                }
            }

            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, oldPoints, GetPoints()));
        }
        #endregion
    }
}
