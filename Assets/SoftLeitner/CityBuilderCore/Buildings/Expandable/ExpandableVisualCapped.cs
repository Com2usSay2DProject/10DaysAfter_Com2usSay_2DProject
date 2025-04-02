using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// alternate variant or <see cref="ExpandableVisual"/> that uses caps and sides instead of start and end<br/>
    /// caps are places in the corners af the expandable and sides are instantiated between those
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual/buildings">https://citybuilder.softleitner.com/manual/buildings</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_expandable_visual_capped.html")]
    public class ExpandableVisualCapped : MonoBehaviour, IExpandableVisual
    {
        public enum RepetitionMode { Inside, Offset, Exended }

        [Tooltip("info that defines how the expansion works")]
        public ExpandableBuildingInfo Info;
        [Tooltip("the building the visual belongs to, can be left empty for ghosts")]
        public ExpandableBuilding Building;
        [Tooltip("if set this pivot will be moved to the center of the expanded building")]
        public Transform Pivot;
        [Tooltip("instantiated in the start and end for linear buildings and in each corner for areas")]
        public Transform CapPart;
        [Tooltip("instantiated between corner caps")]
        public Transform SidePart;
        [Tooltip("part that will be instantiated for each expansion step and placed at its center")]
        public Transform RepeatingPart;
        [Tooltip("whether repetitions are only placed inside, offset into the border or extend a full row outside")]
        public RepetitionMode RepeatingMode;

        public event Action VisualsUpdated;

        public GameObject GameObject => gameObject;

        private List<Transform> _capParts = new List<Transform>();
        private List<Transform> _sideParts = new List<Transform>();
        private List<Transform> _repeatedParts = new List<Transform>();

        protected virtual void Start()
        {
            if (Building)
            {
                if (Building.StructureReference != null)
                    UpdateVisuals(Building.Expansion);
                Building.ExpansionChanged += UpdateVisuals;
            }

            if (CapPart)
            {
                CapPart.gameObject.SetActive(false);
            }

            if (SidePart)
            {
                SidePart.gameObject.SetActive(false);
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

            if (CapPart)
            {
                instanceParts(_capParts, CapPart, 2);

                setPosition(Vector2Int.zero, Info.Size, _capParts[0], cellOffset, pivotOffset, map.IsXY);
                setPosition(new Vector2Int(Info.Size.x + repeats * step, 0), new Vector2Int(Info.SizePost.x, Info.Size.y), _capParts[1], cellOffset, pivotOffset, map.IsXY, 180f);
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
                    case RepetitionMode.Exended:
                        pivotOff += new Vector2(cellOffset * step, 0);
                        reps += 2;
                        break;
                }

                instanceParts(_repeatedParts, RepeatingPart, reps);

                for (int i = 0; i < reps; i++)
                {
                    var part = _repeatedParts[i];
                    part.gameObject.SetActive(true);

                    setPosition(new Vector2Int(Info.Size.x + i * step, 0), new Vector2Int(Info.PositiveStep.x, Info.Size.y), part, cellOffset, pivotOff, map.IsXY);
                }
            }

            if (SidePart)
            {
                instanceParts(_sideParts, SidePart, repeats * 2);

                for (int i = 0; i < repeats; i++)
                {
                    var partA = _sideParts[i * 2];
                    partA.gameObject.SetActive(true);

                    setPosition(new Vector2Int(Info.Size.x + i * step, 0), new Vector2Int(Info.PositiveStep.x, Info.Size.y), partA, cellOffset, pivotOffset, map.IsXY);

                    var partB = _sideParts[i * 2 + 1];
                    partB.gameObject.SetActive(true);

                    setPosition(new Vector2Int(Info.Size.x + i * step, 0), new Vector2Int(Info.PositiveStep.x, Info.Size.y), partB, cellOffset, pivotOffset, map.IsXY, 180f);
                }
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

            var cellOffset = map.CellOffset.x;
            var pivotOffset = new Vector2((Info.Size.x + expansion.x + Info.SizePost.x) * map.CellOffset.x / 2f, (Info.Size.y + expansion.y + Info.SizePost.y) * map.CellOffset.y / 2f);

            var endX = Info.Size.x + repeatsX * stepX;
            var endY = Info.Size.y + repeatsY * stepY;

            if (CapPart)
            {
                instanceParts(_capParts, CapPart, 4);

                setPosition(Vector2Int.zero, Info.Size, _capParts[0], cellOffset, pivotOffset, map.IsXY);
                setPosition(new Vector2Int(endX, 0), new Vector2Int(Info.SizePost.x, Info.Size.y), _capParts[1], cellOffset, pivotOffset, map.IsXY, 270f);
                setPosition(new Vector2Int(endX, endY), Info.SizePost, _capParts[2], cellOffset, pivotOffset, map.IsXY, 180);
                setPosition(new Vector2Int(0, endY), new Vector2Int(Info.SizePost.x, Info.Size.y), _capParts[3], cellOffset, pivotOffset, map.IsXY, 90f);
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
                    case RepetitionMode.Exended:
                        pivotOff += new Vector2(cellOffset * stepX, cellOffset * stepY);
                        repsX += 2;
                        repsY += 2;
                        break;
                }

                instanceParts(_repeatedParts, RepeatingPart, repsX * repsY);

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

            if (SidePart)
            {
                instanceParts(_sideParts, SidePart, repeatsX * 2 + repeatsY * 2);

                for (int x = 0; x < repeatsX; x++)
                {
                    var partA = _sideParts[x * 2];
                    partA.gameObject.SetActive(true);

                    setPosition(new Vector2Int(Info.Size.x + x * stepX, 0), step, partA, cellOffset, pivotOffset, map.IsXY);

                    var partB = _sideParts[x * 2 + 1];
                    partB.gameObject.SetActive(true);

                    setPosition(new Vector2Int(Info.Size.x + x * stepX, endY), step, partB, cellOffset, pivotOffset, map.IsXY, 180f);
                }

                for (int y = 0; y < repeatsY; y++)
                {
                    var partA = _sideParts[repeatsX * 2 + y * 2];
                    partA.gameObject.SetActive(true);

                    setPosition(new Vector2Int(0, Info.Size.y + y * stepY), step, partA, cellOffset, pivotOffset, map.IsXY, 90f);

                    var partB = _sideParts[repeatsX * 2 + y * 2 + 1];
                    partB.gameObject.SetActive(true);

                    setPosition(new Vector2Int(endX, Info.Size.y + y * stepY), step, partB, cellOffset, pivotOffset, map.IsXY, 270f);
                }
            }
        }

        private void instanceParts(List<Transform> collection, Transform prefab, int count)
        {
            if (!prefab)
                return;

            while (collection.Count < count)
            {
                collection.Add(Instantiate(prefab, prefab.parent));
            }

            while (collection.Count > count)
            {
                Destroy(collection[collection.Count - 1].gameObject);
                collection.RemoveAt(collection.Count - 1);
            }
        }

        private void setPosition(Vector2Int point, Vector2Int size, Transform transform, float cellOffset, Vector2 pivotOffset, bool isXY, float rotation = 0f)
        {
            if (isXY)
                transform.localPosition = new Vector3((point.x + size.x / 2f) * cellOffset - pivotOffset.x, (point.y + size.y / 2f) * cellOffset - pivotOffset.y, transform.localPosition.z);
            else
                transform.localPosition = new Vector3((point.x + size.x / 2f) * cellOffset - pivotOffset.x, transform.localPosition.y, (point.y + size.y / 2f) * cellOffset - pivotOffset.y);

            if (isXY)
                transform.localRotation = Quaternion.Euler(0, 0, rotation);
            else
                transform.localRotation = Quaternion.Euler(0, rotation, 0);
        }
    }
}