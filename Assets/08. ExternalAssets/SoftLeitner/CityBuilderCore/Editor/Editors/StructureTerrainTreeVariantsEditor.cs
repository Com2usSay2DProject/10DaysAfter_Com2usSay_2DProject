﻿using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CityBuilderCore.Editor
{
    [CustomEditor(typeof(StructureTerrainTreeVariants))]
    public class StructureTerrainTreeVariantsEditor : UnityEditor.Editor
    {
        public enum PrefabSelection { Random, Alternating, First, Second, Third, Fourth, Fifth }

        private int _num;

        private IMap _map;
        private IGridPositions _gridPositions;
        private IGridHeights _gridHeights;

        private PrefabSelection _prefabSelection;

        private int _prefabIndex;
        private bool _isPlacing;
        private bool _isValid;
        private Vector3 _position;

        private void OnEnable()
        {
            _map = this.FindObjects<MonoBehaviour>().OfType<IMap>().FirstOrDefault();
            _gridPositions = this.FindObjects<MonoBehaviour>().OfType<IGridPositions>().FirstOrDefault();
            _gridHeights = this.FindObjects<MonoBehaviour>().OfType<IGridHeights>().FirstOrDefault();
        }

        private void OnDisable()
        {
            if (_isPlacing)
                stopPlacing();
        }

        private void OnSceneGUI()
        {
            if (_map == null || _gridPositions == null || !_isPlacing)
                return;

            _isValid = false;
            if (_map != null && _gridPositions != null)
            {
                if (EditorHelper.GetWorldPosition(out _position, _map))
                {
                    _isValid = true;
                    Handles.DrawWireDisc(EditorHelper.ApplyEditorHeight(_map, _gridHeights, _gridPositions.GetWorldCenterPosition(_position)), Vector3.up, _map.CellOffset.x / 2f);
                }
            }

            SceneView.RepaintAll();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (_isPlacing)
            {
                if (GUILayout.Button(new GUIContent("Stop Placing", "click in scene view to place, needs gizmos visible")))
                    stopPlacing();
            }
            else
            {
                if (GUILayout.Button(new GUIContent("Start Placing", "click in scene view to place, needs gizmos visible")))
                    startPlacing();
            }

            _prefabSelection = (PrefabSelection)EditorGUILayout.EnumPopup("PrefabSelection", _prefabSelection);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            _num = EditorGUILayout.IntField(_num);
            if (GUILayout.Button("Add Random"))
            {
                var trees = (StructureTerrainTreeVariants)target;
                var terrain = trees.TerrainModifier.GetComponent<Terrain>();

                var treeInstances = terrain.terrainData.treeInstances.ToList();

                for (int i = 0; i < _num; i++)
                {
                    var variant = trees.Variants.Random();
                    var size = Random.Range(variant.MinHeight, variant.MaxHeight);
                    var color = 1f - Random.Range(0, variant.ColorVariation);

                    treeInstances.Add(new TreeInstance()
                    {
                        prototypeIndex = variant.Index,
                        position = new Vector3(Random.Range(0, 1f), 0f, Random.Range(0, 1f)),
                        heightScale = size,
                        widthScale = size,
                        color = new Color(color, color, color),
                        lightmapColor = Color.white
                    });
                }

                terrain.terrainData.SetTreeInstances(treeInstances.ToArray(), true);
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Clear"))
            {
                var trees = (StructureTerrainTreeVariants)target;
                var terrain = trees.TerrainModifier.GetComponent<Terrain>();
                var indices = trees.Variants.Select(v => v.Index).ToHashSet();

                terrain.terrainData.SetTreeInstances(terrain.terrainData.treeInstances.Where(i => !indices.Contains(i.prototypeIndex)).ToArray(), true);
            }
        }

        private void startPlacing()
        {
            SceneView.beforeSceneGui += beforeSceneGui;
            _isPlacing = true;
        }
        private void stopPlacing()
        {
            SceneView.beforeSceneGui -= beforeSceneGui;
            _isPlacing = false;
        }

        private void place()
        {
            var structure = (StructureTerrainTreeVariants)target;
            var point = _gridPositions.GetGridPoint(_position);

            if (Application.isPlaying)
            {
                if (structure.HasPoint(point))
                    structure.Remove(point);
                else
                    structure.Add(point);
            }
            else
            {
                var terrain = structure.TerrainModifier.GetComponent<Terrain>();
                Undo.RegisterCompleteObjectUndo(terrain.terrainData, "Place Tree");

                for (int i = 0; i < terrain.terrainData.treeInstances.Length; i++)
                {
                    if (getTreePoint(terrain, terrain.terrainData.treeInstances[i].position) == point)
                    {
                        var trees = terrain.terrainData.treeInstances.ToList();
                        trees.RemoveAt(i);
                        terrain.terrainData.SetTreeInstances(trees.ToArray(), false);
                        return;
                    }
                }

                StructureTerrainTreeVariants.Variant variant;
                switch (_prefabSelection)
                {
                    case PrefabSelection.Random:
                        variant = structure.Variants.Random();
                        break;
                    case PrefabSelection.Alternating:
                        variant = structure.Variants.ElementAtOrDefault(_prefabIndex);
                        _prefabIndex++;
                        if (_prefabIndex >= structure.Variants.Length - 1)
                            _prefabIndex = 0;
                        break;
                    default:
                    case PrefabSelection.First:
                        variant = structure.Variants.ElementAtOrDefault(0);
                        break;
                    case PrefabSelection.Second:
                        variant = structure.Variants.ElementAtOrDefault(1);
                        break;
                    case PrefabSelection.Third:
                        variant = structure.Variants.ElementAtOrDefault(2);
                        break;
                    case PrefabSelection.Fourth:
                        variant = structure.Variants.ElementAtOrDefault(3);
                        break;
                    case PrefabSelection.Fifth:
                        variant = structure.Variants.ElementAtOrDefault(4);
                        break;
                }

                if (variant == null)
                    return;

                var position = _gridPositions.GetWorldCenterPosition(point);
                position.y = terrain.SampleHeight(position);
                position = new Vector3(position.x / terrain.terrainData.size.x, position.y / terrain.terrainData.size.y, position.z / terrain.terrainData.size.z);

                var size = Random.Range(variant.MinHeight, variant.MaxHeight);
                var color = 1f - Random.Range(0, variant.ColorVariation);

                terrain.AddTreeInstance(new TreeInstance()
                {
                    prototypeIndex = variant.Index,
                    position = position,
                    heightScale = size,
                    widthScale = size,
                    color = new Color(color, color, color),
                    lightmapColor = Color.white
                });
            }
        }

        private Vector2Int getTreePoint(Terrain terrain,Vector3 position)
        {
            return _gridPositions.GetGridPoint(Vector3.Scale(position, terrain.terrainData.size));
        }

        private void beforeSceneGui(SceneView sceneView)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Event.current.Use();
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                Event.current.Use();
                if (_isValid)
                    place();
            }
        }
    }
}