using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// structure made up of a collection of gameobjects each occupying exactly one point on the map<br/>
    /// saves full position instead of just points like <see cref="StructureCollection"/><br/>
    /// if the members of the collection are <see cref="ISaveData"/> that data will also be stored
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual/structures">https://citybuilder.softleitner.com/manual/structures</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_structure_collection_float.html")]
    public class StructureCollectionFloat : KeyedBehaviour, IStructure
    {
        [Serializable]
        public class Variant
        {
            [Tooltip("prefab instantiated by this variant")]
            public GameObject Prefab;
            [Tooltip("how far newly created objects can randomly be offset from the cell center")]
            [Range(0f, 0.95f)]
            public float OffsetMaximum;
            [Tooltip("how newly added objects are rotated(no rotation | rand 90° steps | rand full float)")]
            public StructureRotationMode RotationMode;
            [Tooltip("lower bound for random scale of new objects")]
            public float ScaleMinimum = 1f;
            [Tooltip("upper bound for random scale of new objects")]
            public float ScaleMaximum = 1f;

            public GameObject Instantiate(Transform parent, Vector2Int point, IMap map, IGridPositions gridPositions, IGridRotations gridRotations)
            {
                var instance = UnityEngine.Object.Instantiate(Prefab, parent);
                Adjust(instance, point, map, gridPositions, gridRotations);
                return instance;
            }
            public GameObject Instantiate(Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
            {
                var instance = UnityEngine.Object.Instantiate(Prefab, parent);
                instance.transform.localPosition = localPosition;
                instance.transform.localRotation = localRotation;
                instance.transform.localScale = localScale;
                return instance;
            }

            public void Adjust(GameObject instance, Vector2Int point, IMap map, IGridPositions gridPositions, IGridRotations gridRotations)
            {
                instance.transform.position = gridPositions.GetWorldCenterPosition(point);

                if (OffsetMaximum != 0f)
                {
                    var offset = new Vector3(
                        map.CellOffset.x * UnityEngine.Random.Range(-OffsetMaximum / 2f, OffsetMaximum / 2f),
                        map.CellOffset.y * UnityEngine.Random.Range(-OffsetMaximum / 2f, OffsetMaximum / 2f),
                        map.CellOffset.z * UnityEngine.Random.Range(-OffsetMaximum / 2f, OffsetMaximum / 2f));

                    if (map.IsXY)
                        offset.z = 0f;
                    else
                        offset.y = 0f;

                    instance.transform.position += offset;
                }

                instance.transform.localScale = Prefab.transform.localScale;

                if (ScaleMinimum != 0f && ScaleMaximum != 0f && !(ScaleMinimum == 1f && ScaleMaximum == 1f))
                    instance.transform.localScale *= UnityEngine.Random.Range(ScaleMinimum, ScaleMaximum);

                switch (RotationMode)
                {
                    case StructureRotationMode.Stepped:
                        gridRotations.SetRotation(instance.transform, UnityEngine.Random.Range(0, 3) * 90);
                        break;
                    case StructureRotationMode.Full:
                        gridRotations.SetRotation(instance.transform, UnityEngine.Random.Range(0f, 360f));
                        break;
                }
            }
        }

        [Tooltip("name of the structure in the UI")]
        public string Name;
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
        [Tooltip("all the prefabs that can be used for new points or when loading, gameobject names of already placed objects have to start with the prefab name")]
        public Variant[] Variants;

        bool IStructure.IsDestructible => IsDestructible;
        bool IStructure.IsMovable => IsMovable;
        bool IStructure.IsDecorator => IsDecorator;
        bool IStructure.IsWalkable => IsWalkable;
        int IStructure.Level => Level.Value;

        public StructureReference StructureReference { get; set; }

        public Transform Root => transform;

        public event Action<PointsChanged<IStructure>> PointsChanged;

        private Dictionary<Vector2Int, GameObject> _objects = new Dictionary<Vector2Int, GameObject>();
        private IGridPositions _gridPositions;
        private IGridRotations _gridRotations;
        private IGridHeights _gridHeights;

        private void Start()
        {
            _gridPositions = Dependencies.Get<IGridPositions>();
            _gridRotations = Dependencies.Get<IGridRotations>();
            _gridHeights = Dependencies.GetOptional<IGridHeights>();

            foreach (Transform child in transform)
            {
                _objects.Add(_gridPositions.GetGridPoint(child.position), child.gameObject);
            }

            StructureReference = new StructureReference(this);
            Dependencies.Get<IStructureManager>().RegisterStructure(this);

            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), GetPoints()));
        }

        public IEnumerable<Vector2Int> GetChildPoints(IGridPositions positions)
        {
            foreach (Transform child in transform)
            {
                yield return positions.GetGridPoint(child.position);
            }
        }

        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded)
                Dependencies.Get<IStructureManager>().DeregisterStructure(this);
        }

        public IEnumerable<Vector2Int> GetPoints() => _objects.Keys;

        public bool HasPoint(Vector2Int point) => _objects.ContainsKey(point);

        public void Add(Vector2Int point, UndoRedoActions undoRedoActions = null) => Add(new Vector2Int[] { point }, undoRedoActions);
        public void Add(IEnumerable<Vector2Int> points, UndoRedoActions undoRedoActions = null)
        {
            var map = Dependencies.Get<IMap>();
            var changedPoints = new List<Vector2Int>();
            var changedObjects = new List<GameObject>();

            foreach (var point in points)
            {
                if (_objects.ContainsKey(point))
                    continue;

                var instance = Variants.Random().Instantiate(transform, point, map, _gridPositions, _gridRotations);

                _gridHeights?.ApplyHeight(instance.transform);
                _objects.Add(point, instance);
                changedPoints.Add(point);
                changedObjects.Add(instance);
            }

            if (changedPoints.Count == 0)
                return;

            undoRedoActions?.Add(new StructureCollectionFloatAddition(this, changedObjects));
            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), changedPoints));
        }
        public void Add(Vector2Int point, int variantIndex, UndoRedoActions undoRedoActions = null)
        {
            var map = Dependencies.Get<IMap>();

            if (_objects.ContainsKey(point))
                return;

            var instance = Variants[variantIndex].Instantiate(transform, point, map, _gridPositions, _gridRotations);

            _objects.Add(point, instance);

            var changedPoints = new Vector2Int[] { point };

            undoRedoActions?.Add(new StructureCollectionFloatAddition(this, new GameObject[] { instance }));
            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), changedPoints));
        }
        public void Add(Vector2Int point, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, int? variantIndex = null, string data = null, UndoRedoActions undoRedoActions = null)
        {
            var map = Dependencies.Get<IMap>();

            if (_objects.ContainsKey(point))
                return;

            var variant = variantIndex.HasValue ? Variants[variantIndex.Value] : Variants.Random();
            var instance = variant.Instantiate(transform, localPosition, localRotation, localScale);

            if (!string.IsNullOrWhiteSpace(data) && instance.TryGetComponent<ISaveData>(out var saver))
                saver.LoadData(data);

            _objects.Add(point, instance);

            var changedPoints = new Vector2Int[] { point };

            undoRedoActions?.Add(new StructureCollectionFloatAddition(this, new GameObject[] { instance }));
            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), changedPoints));
        }
        public IEnumerable<Vector2Int> Remove(Vector2Int point, UndoRedoActions undoRedoActions = null) => Remove(new Vector2Int[] { point }, undoRedoActions);
        public IEnumerable<Vector2Int> Remove(IEnumerable<Vector2Int> points, UndoRedoActions undoRedoActions = null)
        {
            var children = new List<GameObject>();
            var changedPoints = new List<Vector2Int>();
            var changedObjects = new List<GameObject>();

            foreach (var point in points)
            {
                if (!_objects.ContainsKey(point))
                    continue;

                if (children.Contains(_objects[point]))
                    continue;

                children.Add(_objects[point]);
                changedPoints.Add(point);
            }

            foreach (var child in children)
            {
                _objects.Remove(_gridPositions.GetGridPoint(child.transform.position), out var instance);
                changedObjects.Add(instance);
                Destroy(child);
            }

            if (changedPoints.Count == 0)
                return changedPoints;

            undoRedoActions?.Add(new StructureCollectionFloatRemoval(this, changedObjects));
            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, changedPoints, Enumerable.Empty<Vector2Int>()));

            return changedPoints;
        }

        public GameObject GetObject(Vector2Int point) => _objects.GetValueOrDefault(point);
        public int GetVariantIndex(Vector2Int point)
        {
            return GetVariantIndex(GetObject(point).name);
        }
        public int GetVariantIndex(string name)
        {
            if (Variants == null || Variants.Length < 2)
                return 0;

            for (int i = 0; i < Variants.Length; i++)
            {
                if (name.StartsWith(Variants[i].Prefab.name))
                    return i;
            }

            return 0;
        }

        public void Clear()
        {
            _objects.ForEach(o => Destroy(o.Value));
            _objects.Clear();
        }

        public string GetName() => string.IsNullOrWhiteSpace(Name) ? name : Name;

        #region UndoRedo
        [Serializable]
        public class UndoRedoStructureCollectionFloatData
        {
            public string Key;
            public int[] Indices;
            public Vector2Int[] Points;
            public Vector3[] Positions;
            public Quaternion[] Rotations;
            public Vector3[] Scales;
            public string[] InstanceData;
        }
        public abstract class UndoRedoStructureCollectionFloatBase : UndoRedoActionBase
        {
            private UndoRedoStructureCollectionFloatData _data;

            public UndoRedoStructureCollectionFloatBase(StructureCollectionFloat structure, IEnumerable<GameObject> objects)
            {
                var indices = new List<int>();
                var points = new List<Vector2Int>();
                var positions = new List<Vector3>();
                var rotations = new List<Quaternion>();
                var scales = new List<Vector3>();
                var instanceData = new List<string>();

                var gridPositions = Dependencies.Get<IGridPositions>();

                foreach (var o in objects)
                {
                    indices.Add(structure.GetVariantIndex(o.name));
                    points.Add(gridPositions.GetGridPoint(o.transform.position));
                    positions.Add(o.transform.localPosition);
                    rotations.Add(o.transform.localRotation);
                    scales.Add(o.transform.localScale);
                    if (o.TryGetComponent<ISaveData>(out var saver))
                        instanceData.Add(saver.SaveData());
                    else
                        instanceData.Add(null);
                }

                _data = new UndoRedoStructureCollectionFloatData()
                {
                    Key = structure.Key,
                    Indices = indices.ToArray(),
                    Points = points.ToArray(),
                    Positions = positions.ToArray(),
                    Rotations = rotations.ToArray(),
                    Scales = scales.ToArray(),
                    InstanceData = instanceData.ToArray()
                };
            }

            protected virtual void add()
            {
                var collection = Dependencies.Get<IStructureManager>().GetStructure(_data.Key) as StructureCollectionFloat;

                for (int i = 0; i < _data.Points.Length; i++)
                {
                    collection.Add(_data.Points[i], _data.Positions[i], _data.Rotations[i], _data.Scales[i], _data.Indices[i], _data.InstanceData[i]);
                }
            }

            protected virtual void remove()
            {
                var collection = Dependencies.Get<IStructureManager>().GetStructure(_data.Key) as StructureCollectionFloat;

                collection.Remove(_data.Points);
            }

            public override IEnumerable<Vector2Int> GetPoints() => _data.Points;

            #region Saving
            public override string SaveData()
            {
                return JsonUtility.ToJson(_data);
            }
            public override void LoadData(string json)
            {
                _data = JsonUtility.FromJson<UndoRedoStructureCollectionFloatData>(json);
            }
            #endregion
        }
        public class StructureCollectionFloatAddition : UndoRedoStructureCollectionFloatBase
        {
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            static void OnLoad()
            {
                UndoRedoTypes.Register("StructureCollectionFloatAddition", typeof(StructureCollectionFloatAddition));
            }

            public override string Name => "Add";

            public StructureCollectionFloatAddition(StructureCollectionFloat structure, IEnumerable<GameObject> objects) : base(structure, objects)
            {
            }

            public override void Undo() => remove();
            public override void Redo() => add();
        }
        public class StructureCollectionFloatRemoval : UndoRedoStructureCollectionFloatBase
        {
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            static void OnLoad()
            {
                UndoRedoTypes.Register("StructureCollectionFloatRemoval", typeof(StructureCollectionFloatRemoval));
            }

            public override string Name => "Remove";

            public StructureCollectionFloatRemoval(StructureCollectionFloat structure, IEnumerable<GameObject> objects) : base(structure, objects)
            {
            }

            public override void Undo() => add();
            public override void Redo() => remove();
        }
        #endregion
        #region Saving
        [Serializable]
        public class StructureCollectionFloatData
        {
            public string Key;
            public StructureCollectionFloatVariantData[] Variants;
        }
        [Serializable]
        public class StructureCollectionFloatVariantData
        {
            public string Prefab;
            public Vector3[] Positions;
            public float[] Rotations;
            public float[] Scales;
            public string[] InstanceData;
        }

        public StructureCollectionFloatData SaveData()
        {
            var gridRotations = Dependencies.Get<IGridRotations>();
            var variants = new List<StructureCollectionFloatVariantData>();

            foreach (var variant in Variants)
            {
                var positions = new List<Vector3>();
                var rotations = new List<float>();
                var scales = new List<float>();
                var instanceData = new List<string>();
                var hasInstanceData = variant.Prefab.GetComponent<ISaveData>() != null;

                foreach (var o in _objects)
                {
                    if (o.Value.name.StartsWith(variant.Prefab.name))
                    {
                        positions.Add(o.Value.transform.position);
                        rotations.Add(gridRotations.GetRotation(o.Value.transform));
                        scales.Add(o.Value.transform.localScale.x / variant.Prefab.transform.localScale.x);
                        if (hasInstanceData)
                            instanceData.Add(o.Value.GetComponent<ISaveData>().SaveData());
                    }
                }

                if (positions.Count > 0)
                {
                    variants.Add(new StructureCollectionFloatVariantData()
                    {
                        Prefab = variant.Prefab.name,
                        Positions = positions.ToArray(),
                        Rotations = rotations.ToArray(),
                        Scales = scales.ToArray(),
                        InstanceData = instanceData.ToArray()
                    });
                }
            }

            return new StructureCollectionFloatData() { Key = Key, Variants = variants.ToArray() };
        }
        public void LoadData(StructureCollectionFloatData data)
        {
            var oldPoints = _objects.Keys.ToList();

            Clear();

            foreach (var variantData in data.Variants)
            {
                var variant = Variants.FirstOrDefault(p => p.Prefab.name.Equals(variantData.Prefab));
                if (variant == null)
                {
                    Debug.LogError($"Decorator {name} could not find prefab {variantData.Prefab}");
                    continue;
                }

                for (int i = 0; i < variantData.Positions.Length; i++)
                {
                    var position = variantData.Positions[i];
                    var rotation = variantData.Rotations[i];
                    var scale = variantData.Scales[i];

                    var instance = Instantiate(variant.Prefab, transform);
                    instance.transform.position = position;
                    instance.transform.localScale = variant.Prefab.transform.localScale * scale;
                    _gridRotations.SetRotation(instance.transform, rotation);
                    _objects.Add(_gridPositions.GetGridPoint(position), instance);
                }

                if (variantData.InstanceData != null && variantData.InstanceData.Length == _objects.Count)
                {
                    for (int i = 0; i < variantData.InstanceData.Length; i++)
                    {
                        _objects.ElementAt(i).Value.GetComponent<ISaveData>().LoadData(variantData.InstanceData[i]);
                    }
                }
            }


            PointsChanged?.Invoke(new PointsChanged<IStructure>(this, oldPoints, GetPoints()));
        }
        #endregion
    }
}