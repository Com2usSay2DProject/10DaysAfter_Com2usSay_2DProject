using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// behaviour that can be used to adjust an expandable buildings visuals to its actual size<br/>
    /// also adjusts the building pivot position to the expanded size<br/>
    /// can also be used for the ghost by only filling out the info and leaving the building empty
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual/buildings">https://citybuilder.softleitner.com/manual/buildings</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_expandable_visual.html")]
    public class ExpandableVisual : MonoBehaviour, IExpandableVisual
    {
        public enum RepetitionMode { Inside, Offset, Extended }

        [Tooltip("info that defines how the expansion works")]
        public ExpandableBuildingInfo Info;
        [Tooltip("the building the visual belongs to, can be left empty for ghosts")]
        public ExpandableBuilding Building;
        [Tooltip("if set this pivot will be moved to the center of the expanded building")]
        public Transform Pivot;

        [Tooltip("part will be moved to the center of the first point")]
        public Transform StartPart;
        [Tooltip("part that will be instantiated for each expansion step and placed at its center")]
        public Transform RepeatingPart;
        [Tooltip("whether repetitions are only placed inside, offset into the border or extend a full row outside")]
        public RepetitionMode RepeatingMode;
        [Tooltip("part will be moved to the center of the last point")]
        public Transform EndPart;

        public event Action VisualsUpdated;

        public GameObject GameObject => gameObject;

        private List<Transform> _repeatedParts = new List<Transform>();

        protected virtual void Start()
        {
            if (Building)
            {
                if (Building.StructureReference != null)
                    UpdateVisuals(Building.Expansion);
                Building.ExpansionChanged += UpdateVisuals;
            }

            if (RepeatingPart)
            {
                RepeatingPart.gameObject.SetActive(false);
            }
        }

        public Transform GetPart(int i) => _repeatedParts.ElementAtOrDefault(i);

        public virtual void UpdateVisuals(Vector2Int expansion)
        {
            updatePivot(expansion);

            if (Info.IsArea)
                updateAreaObjects(expansion);
            else
                updateLinearObjects(expansion);

            VisualsUpdated?.Invoke();
        }

        private void updatePivot(Vector2Int expansion)
        {
            if (Pivot)
            {
                var map = Dependencies.Get<IMap>();
                var size = Info.Size + expansion + Info.SizePost;

                if (map.IsXY)
                    Pivot.localPosition = new Vector3(size.x / 2f * map.CellOffset.x, size.y / 2f * map.CellOffset.y, 0f);
                else
                    Pivot.localPosition = new Vector3(size.x / 2f * map.CellOffset.x, 0f, size.y / 2f * map.CellOffset.y);
            }
        }

        private void updateLinearObjects(Vector2Int expansion)
        {
            var map = Dependencies.Get<IMap>();
            var step = Info.PositiveStep.x;
            var repeats = Mathf.Max(0, expansion.x) / step;

            var cellOffset = map.CellOffset.x;
            var pivotOffset = new Vector2((Info.Size.x + expansion.x + Info.SizePost.x) * map.CellOffset.x / 2f, (Info.Size.y + expansion.y + Info.SizePost.y) * map.CellOffset.y / 2f);

            if (StartPart)
            {
                setPosition(Vector2Int.zero, Info.Size, StartPart, cellOffset, pivotOffset, map.IsXY);
            }

            if (RepeatingPart)
            {
                var pivotOff = pivotOffset;
                var reps = repeats;

                switch (RepeatingMode)
                {
                    case RepetitionMode.Offset:
                        pivotOff += new Vector2(cellOffset * step / 2f, 0);
                        reps += 1;
                        break;
                    case RepetitionMode.Extended:
                        pivotOff += new Vector2(cellOffset * step, 0);
                        reps += 2;
                        break;
                }

                instanceParts(reps);

                for (int i = 0; i < reps; i++)
                {
                    var part = _repeatedParts[i];
                    part.gameObject.SetActive(true);

                    setPosition(new Vector2Int(Info.Size.x + i * step, 0), new Vector2Int(Info.PositiveStep.x, Info.Size.y), part, cellOffset, pivotOff, map.IsXY);
                }
            }

            if (EndPart)
            {
                setPosition(new Vector2Int(Info.Size.x + repeats * step, 0), new Vector2Int(Info.SizePost.x, Info.Size.y), EndPart, cellOffset, pivotOffset, map.IsXY);
            }
        }

        private void updateAreaObjects(Vector2Int expansion)
        {
            var map = Dependencies.Get<IMap>();
            var stepX = Info.PositiveStep.x;
            var stepY = Info.PositiveStep.y;
            var step = new Vector2Int(stepX, stepY);
            var repeatsX = Mathf.Max(0, expansion.x) / stepX;
            var repeatsY = Mathf.Max(0, expansion.y) / stepY;
            var repeats = Mathf.Max(0, repeatsX * repeatsY);

            var cellOffset = map.CellOffset.x;
            var pivotOffset = new Vector2((Info.Size.x + expansion.x + Info.SizePost.x) * map.CellOffset.x / 2f, (Info.Size.y + expansion.y + Info.SizePost.y) * map.CellOffset.y / 2f);

            if (StartPart)
            {
                setPosition(Vector2Int.zero, Info.Size, StartPart, cellOffset, pivotOffset, map.IsXY);
            }

            if (RepeatingPart)
            {
                var pivotOff = pivotOffset;
                var repsX = repeatsX;
                var repsY = repeatsY;

                switch (RepeatingMode)
                {
                    case RepetitionMode.Offset:
                        pivotOff += new Vector2(cellOffset * stepX / 2f, cellOffset * stepY / 2f);
                        repsX += 1;
                        repsY += 1;
                        break;
                    case RepetitionMode.Extended:
                        pivotOff += new Vector2(cellOffset * stepX, cellOffset * stepY);
                        repsX += 2;
                        repsY += 2;
                        break;
                }

                instanceParts(repsX * repsY);

                for (int x = 0; x < repsX; x++)
                {
                    for (int y = 0; y < repsY; y++)
                    {
                        var part = _repeatedParts[x * repsY + y];
                        part.gameObject.SetActive(true);

                        setPosition(Info.Size + new Vector2Int(x * stepX, y * stepY), step, part, cellOffset, pivotOff, map.IsXY);
                    }
                }
            }

            if (EndPart)
            {
                setPosition(Info.Size + repeats * step, Info.SizePost, EndPart, cellOffset, pivotOffset, map.IsXY);
            }
        }

        private void instanceParts(int count)
        {
            if (!RepeatingPart)
                return;

            while (_repeatedParts.Count < count)
            {
                _repeatedParts.Add(Instantiate(RepeatingPart, RepeatingPart.parent));
            }

            while (_repeatedParts.Count > count)
            {
                Destroy(_repeatedParts[_repeatedParts.Count - 1].gameObject);
                _repeatedParts.RemoveAt(_repeatedParts.Count - 1);
            }
        }

        private void setPosition(Vector2Int point, Vector2Int size, Transform transform, float cellOffset, Vector2 pivotOffset, bool isXY)
        {
            if (isXY)
                transform.localPosition = new Vector3((point.x + size.x / 2f) * cellOffset - pivotOffset.x, (point.y + size.y / 2f) * cellOffset - pivotOffset.y, transform.localPosition.z);
            else
                transform.localPosition = new Vector3((point.x + size.x / 2f) * cellOffset - pivotOffset.x, transform.localPosition.y, (point.y + size.y / 2f) * cellOffset - pivotOffset.y);
        }

    }
}