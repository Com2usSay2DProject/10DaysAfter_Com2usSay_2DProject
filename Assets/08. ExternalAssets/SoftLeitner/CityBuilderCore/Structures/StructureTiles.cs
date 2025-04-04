﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    /// <summary>
    /// structure made up of a collection of tiles on a tilemap
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual/structures">https://citybuilder.softleitner.com/manual/structures</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_structure_tiles.html")]
    public class StructureTiles : KeyedBehaviour, IStructure
    {
        [Tooltip("name of the structure for UI purposes")]
        public string Name;

        [Tooltip("whether the structure can be removed by the player")]
        public bool IsDestructible;
        [Tooltip("whether the structure can be moved by the MoveTool")]
        public bool IsMovable = true;
        [Tooltip("whether the structure is automatically removed when something is built on top of it")]
        public bool IsDecorator;
        [Tooltip("whether walkers can pass the points of this structure")]
        public bool IsWalkable;
        [Tooltip("determines which other structures can reside in the same points")]
        public StructureLevelMask Level;
        [Tooltip("the tilemap that holds the tiles")]
        public Tilemap Tilemap;
        [Tooltip("the tile in the tilemap that counts as a point in this structure")]
        public TileBase Tile;
        [Tooltip(@"structure that any point on this structure will be recreated in
in the defense demo this is used to place a navmesh obstacle on every wall tile")]
        public StructureCollection ReplicaCollection;

        public StructureReference StructureReference { get; set; }

        public Transform Root => transform;
        public bool Changed { get; private set; }

        bool IStructure.IsDestructible => IsDestructible;
        bool IStructure.IsMovable => IsMovable;
        bool IStructure.IsDecorator => IsDecorator;
        bool IStructure.IsWalkable => IsWalkable;
        int IStructure.Level => Level.Value;

        public event Action<PointsChanged<IStructure>> PointsChanged;

        private HashSet<Vector2Int> _points = new HashSet<Vector2Int>();

        private void Awake()
        {
            if (ReplicaCollection)
                ReplicaCollection.IsReplica = true;
        }

        private void Start()
        {
            foreach (var position in Tilemap.cellBounds.allPositionsWithin)
            {
                if (Tile == null && Tilemap.HasTile(position) || Tilemap.GetTile(position) == Tile)
                {
                    _points.Add((Vector2Int)position);
                }
            }

            if (ReplicaCollection)
                ReplicaCollection.Add(_points);

            StructureReference = new StructureReference(this);

            Dependencies.Get<IStructureManager>().RegisterStructure(this);

            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), _points));
        }

        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded)
                Dependencies.Get<IStructureManager>().DeregisterStructure(this);
        }

        public IEnumerable<Vector2Int> GetPoints() => _points;

        public bool HasPoint(Vector2Int point) => _points.Contains(point);

        public void Add(Vector2Int point, UndoRedoActions undoRedoActions = null) => Add(new Vector2Int[] { point }, undoRedoActions);
        public void Add(IEnumerable<Vector2Int> points, UndoRedoActions undoRedoActions = null)
        {
            var changedPoints = new List<Vector2Int>();

            foreach (var point in points)
            {
                if (_points.Contains(point))
                    continue;

                _points.Add(point);
                Tilemap.SetTile((Vector3Int)point, Tile);
                changedPoints.Add(point);
            }

            if (changedPoints.Count == 0)
                return;

            if (ReplicaCollection)
                ReplicaCollection.Add(changedPoints);

            undoRedoActions?.Add(new StructureAddition(this, changedPoints));
            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), points));
        }
        public IEnumerable<Vector2Int> Remove(Vector2Int point, UndoRedoActions undoRedoActions = null) => Remove(new Vector2Int[] { point }, undoRedoActions);
        public IEnumerable<Vector2Int> Remove(IEnumerable<Vector2Int> points, UndoRedoActions undoRedoActions = null)
        {
            var changedPoints = new List<Vector2Int>();

            foreach (var point in points)
            {
                if (!_points.Contains(point))
                    continue;

                _points.Remove(point);
                Tilemap.SetTile((Vector3Int)point, null);
                changedPoints.Add(point);
            }

            if (changedPoints.Count == 0)
                return changedPoints;

            if (ReplicaCollection)
                ReplicaCollection.Remove(changedPoints);

            undoRedoActions?.Add(new StructureRemoval(this, changedPoints));
            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, changedPoints, Enumerable.Empty<Vector2Int>()));

            return changedPoints;
        }

        public void RefreshTile(Vector2Int point)
        {
            Tilemap.RefreshTile((Vector3Int)point);
        }

        public string GetName() => Name;

        #region Saving
        [Serializable]
        public class StructureTilesData
        {
            public string Key;
            public Vector2Int[] Positions;
        }

        public StructureTilesData SaveData()
        {
            return new StructureTilesData()
            {
                Key = Key,
                Positions = _points.ToArray()
            };
        }
        public void LoadData(StructureTilesData data)
        {
            var oldPositions = _points;

            _points.ForEach(p => Tilemap.SetTile((Vector3Int)p, null));
            _points = new HashSet<Vector2Int>();

            if (ReplicaCollection)
                ReplicaCollection.Clear();

            foreach (var position in data.Positions)
            {
                Tilemap.SetTile((Vector3Int)position, Tile);
                _points.Add(position);
            }

            if (ReplicaCollection)
                ReplicaCollection.Add(data.Positions);

            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, oldPositions, _points));
        }
        #endregion
    }
}