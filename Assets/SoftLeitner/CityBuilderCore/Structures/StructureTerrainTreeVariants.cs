using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CityBuilderCore
{
    /// <summary>
    /// structure that adds and removes terrain trees using a <see cref="TerrainModifier"/>, can handle multiple tree variants<br/>
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual/structures">https://citybuilder.softleitner.com/manual/structures</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_structure_terrain_tree_variants.html")]
    public class StructureTerrainTreeVariants : KeyedBehaviour, IStructure
    {
        [Serializable]
        public class Variant
        {
            [Tooltip("the index of the tree prototype")]
            public int Index;
            [Tooltip("minimum value for tree size when a tree is added")]
            public float MinHeight = 1;
            [Tooltip("maximum value for tree size when a tree is added")]
            public float MaxHeight = 1;
            [Tooltip("how much the color may differ at most from the original color")]
            [Range(0, 1)]
            public float ColorVariation;
        }

        [Tooltip("name of the structure for UI purposes")]
        public string Name;
        [Tooltip("the terrain modifier used to retrieve and change the trees")]
        public TerrainModifier TerrainModifier;
        [Tooltip("")]
        public Variant[] Variants;
        [Header("Structure Setting")]
        [Tooltip("whether the structure can be removed by the player")]
        public bool IsDestructible = true;
        [Tooltip("whether the structure can be moved by the MoveTool")]
        public bool IsMovable = true;
        [Tooltip("whether the structure is automatically removed when something is built on top of it")]
        public bool IsDecorator = false;
        [Tooltip("whether walkers can pass the points of this structure")]
        public bool IsWalkable = false;
        [Tooltip("determines which other structures can reside in the same points")]
        public StructureLevelMask Level;

        bool IStructure.IsDestructible => IsDestructible;
        bool IStructure.IsMovable => IsMovable;
        bool IStructure.IsDecorator => IsDecorator;
        bool IStructure.IsWalkable => IsWalkable;
        int IStructure.Level => Level.Value;

        public StructureReference StructureReference { get; set; }

        public event Action<PointsChanged<IStructure>> PointsChanged;

        private List<int> _indices;
        private HashSet<Vector2Int> _points;
        private UnityAction _terrainLoadedAction;

        private void Start()
        {
            _indices = new List<int>(Variants.Select(v => v.Index));
            _points = new HashSet<Vector2Int>(Variants.SelectMany(v => TerrainModifier.GetTreePoints(v.Index).Distinct()));

            StructureReference = new StructureReference(this);
            Dependencies.Get<IStructureManager>().RegisterStructure(this, true);

            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), _points));
        }

        private void OnEnable()
        {
            if (_terrainLoadedAction == null)
                _terrainLoadedAction = new UnityAction(terrainLoaded);

            TerrainModifier.Loaded.AddListener(_terrainLoadedAction);
        }
        private void OnDisable()
        {
            TerrainModifier.Loaded.RemoveListener(_terrainLoadedAction);
        }

        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded)
                Dependencies.Get<IStructureManager>().DeregisterStructure(this);
        }

        public string GetName() => Name;

        public IEnumerable<Vector2Int> GetPoints() => _points;
        public bool HasPoint(Vector2Int point) => _points.Contains(point);

        public void Add(IEnumerable<Vector2Int> points, UndoRedoActions undoRedoActions = null)
        {
            var changedPoints = new List<Vector2Int>();
            var changedTrees = new List<List<TreeInstance>>();

            foreach (var point in points)
            {
                if (_points.Contains(point))
                    continue;

                var variant = Variants.Random();
                var size = UnityEngine.Random.Range(variant.MinHeight, variant.MaxHeight);
                var color = 1f - UnityEngine.Random.Range(0, variant.ColorVariation);

                _points.Add(point);
                changedPoints.Add(point);
                changedTrees.Add(new List<TreeInstance>() { TerrainModifier.AddTree(point, variant.Index, size, size, color) });
            }

            if (changedPoints.Count == 0)
                return;

            undoRedoActions?.Add(new StructureTerrainTreeVariantsAddition(this, changedPoints, changedTrees));
            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), changedPoints));
        }
        public void Add(Vector2Int point, TreeInstance template, int? variantIndex = null, UndoRedoActions undoRedoActions = null)
        {
            Variant variant = null;

            if (variantIndex.HasValue)
            {
                variant = Variants[variantIndex.Value];
            }
            else if (_indices.Contains(template.prototypeIndex))
            {
                variant = Variants[_indices.IndexOf(template.prototypeIndex)];
            }
            else
            {
                variant = Variants.Random();
            }

            _points.Add(point);

            var changedPoints = new List<Vector2Int> { point };
            var changedTrees = new List<List<TreeInstance>>() { new List<TreeInstance>() { TerrainModifier.AddTree(template, variant.Index) } };

            undoRedoActions?.Add(new StructureTerrainTreeVariantsAddition(this, changedPoints, changedTrees));
            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), changedPoints));
        }
        public void Add(Vector2Int point, int variantIndex, UndoRedoActions undoRedoActions = null)
        {
            var variant = Variants[variantIndex];
            var size = UnityEngine.Random.Range(variant.MinHeight, variant.MaxHeight);
            var color = 1f - UnityEngine.Random.Range(0, variant.ColorVariation);

            _points.Add(point);

            var changedPoints = new List<Vector2Int> { point };
            var changedTrees = new List<List<TreeInstance>>() { new List<TreeInstance>() { TerrainModifier.AddTree(point, variant.Index, size, size, color) } };

            undoRedoActions?.Add(new StructureTerrainTreeVariantsAddition(this, changedPoints, changedTrees));
            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), changedPoints));
        }
        public IEnumerable<Vector2Int> Remove(IEnumerable<Vector2Int> points, UndoRedoActions undoRedoActions = null)
        {
            var changedPoints = new List<Vector2Int>();
            var changedTrees = new List<List<TreeInstance>>();

            foreach (var point in points)
            {
                if (!_points.Contains(point))
                    continue;

                _points.Remove(point);
                changedPoints.Add(point);
                changedTrees.Add(TerrainModifier.RemoveTrees(point, _indices).ToList());
            }

            if (changedPoints.Count == 0)
                return changedPoints;

            undoRedoActions?.Add(new StructureTerrainTreeVariantsRemoval(this, changedPoints, changedTrees));
            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, changedPoints, Enumerable.Empty<Vector2Int>()));

            return changedPoints;
        }

        public TreeInstance GetTree(Vector2Int point)
        {
            return TerrainModifier.GetTree(point, _indices);
        }
        public IEnumerable<TreeInstance> GetTrees(Vector2Int point)
        {
            return TerrainModifier.GetTrees(point, _indices);
        }

        public int GetVariantIndex(Vector2Int point)
        {
            return GetVariantIndex(GetTree(point));
        }
        public int GetVariantIndex(TreeInstance instance)
        {
            return _indices.IndexOf(instance.prototypeIndex);
        }

        private void terrainLoaded()
        {
            if (_points == null)
                return;

            var previousPoint = _points.ToList();
            _points = new HashSet<Vector2Int>(TerrainModifier.GetTreePoints(_indices).Distinct());

            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, previousPoint, _points));
        }

        #region UndoRedo
        [Serializable]
        public class UndoRedoStructureTerrainTreeVariantsData
        {
            public string Key;
            public UndoRedoStructureTerrainPointTrees[] PointTrees;
        }
        [Serializable]
        public class UndoRedoStructureTerrainPointTrees
        {
            public Vector2Int Point;
            public TreeInstance[] Trees;
        }
        public abstract class UndoRedoStructureTerrainTreeVariantsBase : UndoRedoActionBase
        {
            private UndoRedoStructureTerrainTreeVariantsData _data;

            public UndoRedoStructureTerrainTreeVariantsBase(StructureTerrainTreeVariants structure, List<Vector2Int> points, List<List<TreeInstance>> trees)
            {
                var pointTrees = new UndoRedoStructureTerrainPointTrees[points.Count];

                for (int i = 0; i < points.Count; i++)
                {
                    pointTrees[i] = new UndoRedoStructureTerrainPointTrees()
                    {
                        Point = points[i],
                        Trees = trees[i].ToArray()
                    };
                }

                _data = new UndoRedoStructureTerrainTreeVariantsData()
                {
                    Key = structure.Key,
                    PointTrees = pointTrees
                };
            }

            protected virtual void add()
            {
                var collection = Dependencies.Get<IStructureManager>().GetStructure(_data.Key) as StructureTerrainTreeVariants;

                foreach (var pointTrees in _data.PointTrees)
                {
                    foreach (var tree in pointTrees.Trees)
                    {
                        collection.Add(pointTrees.Point, tree);
                    }
                }
            }

            protected virtual void remove()
            {
                var collection = Dependencies.Get<IStructureManager>().GetStructure(_data.Key) as StructureTerrainTreeVariants;

                collection.Remove(_data.PointTrees.Select(p => p.Point));
            }

            public override IEnumerable<Vector2Int> GetPoints() => _data.PointTrees.Select(pt => pt.Point);

            #region Saving
            public override string SaveData()
            {
                return JsonUtility.ToJson(_data);
            }
            public override void LoadData(string json)
            {
                _data = JsonUtility.FromJson<UndoRedoStructureTerrainTreeVariantsData>(json);
            }
            #endregion
        }
        public class StructureTerrainTreeVariantsAddition : UndoRedoStructureTerrainTreeVariantsBase
        {
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            static void OnLoad()
            {
                UndoRedoTypes.Register("StructureTerrainTreeVariantsAddition", typeof(StructureTerrainTreeVariantsAddition));
            }

            public override string Name => "Add Trees";

            public StructureTerrainTreeVariantsAddition(StructureTerrainTreeVariants structure, List<Vector2Int> points, List<List<TreeInstance>> trees) : base(structure, points, trees)
            {
            }

            public override void Undo() => remove();
            public override void Redo() => add();
        }
        public class StructureTerrainTreeVariantsRemoval : UndoRedoStructureTerrainTreeVariantsBase
        {
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            static void OnLoad()
            {
                UndoRedoTypes.Register("StructureTerrainTreeVariantsRemoval", typeof(StructureTerrainTreeVariantsRemoval));
            }

            public override string Name => "Remove Trees";

            public StructureTerrainTreeVariantsRemoval(StructureTerrainTreeVariants structure, List<Vector2Int> points, List<List<TreeInstance>> trees) : base(structure, points, trees)
            {
            }

            public override void Undo() => add();
            public override void Redo() => remove();
        }
        #endregion
    }
}
