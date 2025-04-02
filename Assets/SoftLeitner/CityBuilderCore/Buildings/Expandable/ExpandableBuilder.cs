using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CityBuilderCore
{
    /// <summary>
    /// special builder that can place <see cref="ExpandableBuilding"/> of dynamic size by dragging out the size
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual/buildings">https://citybuilder.softleitner.com/manual/buildings</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_expandable_builder.html")]
    public class ExpandableBuilder : PointerToolBase
    {
        [Tooltip("the building that will be created by this builder")]
        public ExpandableBuildingInfo BuildingInfo;
        [Tooltip("whether the builder starts at the minimum size of the building instead of a single point")]
        public bool StartAtMinimum;
        [Tooltip("whether buildings can be rotated using R, for example in Isometric games where this does not make sense")]
        public bool AllowRotate = true;
        [Tooltip("fired whenever a building is built")]
        public UnityEvent<Building> Built;

        public override string TooltipName => BuildingInfo.Cost != null && BuildingInfo.Cost.Length > 0 ? $"{BuildingInfo.Name}({BuildingInfo.Cost.ToDisplayString()})" : BuildingInfo.Name;
        public override string TooltipDescription => BuildingInfo.Description;

        protected BuildingRotation _rotation;
        protected int _index;
        protected IExpandableVisual _ghost;

        protected List<ItemQuantity> _costs = new List<ItemQuantity>();
        protected IGlobalStorage _globalStorage;
        protected IHighlightManager _highlighting;
        protected IMap _map;

        protected override void Start()
        {
            base.Start();

            _globalStorage = Dependencies.GetOptional<IGlobalStorage>();
            _highlighting = Dependencies.Get<IHighlightManager>();
            _map = Dependencies.Get<IMap>();
        }

        public override void ActivateTool()
        {
            base.ActivateTool();

            _index = 0;
            _rotation = Dependencies.GetOptional<BuildingRotationKeeper>()?.Rotation ?? BuildingRotation.Create();

            recreateGhost();
        }

        public override void DeactivateTool()
        {
            if (_ghost != null)
            {
                Destroy(_ghost.GameObject);
                _ghost = null;
            }

            _costs.Clear();

            base.DeactivateTool();
        }

        public override int GetCost(Item item)
        {
            return _costs.FirstOrDefault(c => c.Item == item)?.Quantity ?? 0;
        }

        protected override void updateTool()
        {
            base.updateTool();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _index++;
                recreateGhost();
            }
        }

        protected override void updatePointer(Vector2Int mousePoint, Vector2Int dragStart, bool isDown, bool isApply)
        {
            if (!isDown)
            {
                if (AllowRotate && Input.GetKeyDown(KeyCode.R))
                {
                    _rotation.TurnClockwise();
                }
            }

            List<Vector2Int> validPoints = new List<Vector2Int>();
            List<Vector2Int> invalidPoints = new List<Vector2Int>();

            Vector2Int drag = mousePoint - dragStart;

            Vector2Int expansion;
            Vector2Int point;

            var sizePre = BuildingInfo.Size;
            var sizeStep = BuildingInfo.PositiveStep;
            var sizePost = BuildingInfo.SizePost;
            var sizeBorder = sizePre + sizePost;
            var expansionMinimum = BuildingInfo.ExpansionMinimum;

            if (isDown && drag.sqrMagnitude > 0f)
            {
                if (BuildingInfo.IsArea)
                {
                    expansion = new Vector2Int(Mathf.Abs(drag.x), Mathf.Abs(drag.y)) - sizeBorder + Vector2Int.one;
                    expansion = _rotation.RotateSize(expansion);
                    expansion = new Vector2Int(
                        Mathf.CeilToInt(expansion.x / (float)sizeStep.x) * sizeStep.x,
                        Mathf.CeilToInt(expansion.y / (float)sizeStep.y) * sizeStep.y);

                    if (StartAtMinimum)
                        expansion = new Vector2Int(Mathf.Max(BuildingInfo.ExpansionMinimum.x, expansion.x), Mathf.Max(BuildingInfo.ExpansionMinimum.y, expansion.y));

                    var deltaX = (_rotation.RotateSize(expansion).x + sizeBorder.x - 1) * (int)Mathf.Sign(drag.x);
                    var deltaY = (_rotation.RotateSize(expansion).y + sizeBorder.y - 1) * (int)Mathf.Sign(drag.y);

                    point = dragStart;

                    if (drag.x < 0)
                        point = new Vector2Int(point.x + deltaX, point.y);

                    if (drag.y < 0)
                        point = new Vector2Int(point.x, point.y + deltaY);
                }
                else
                {
                    if (Mathf.Abs(drag.x) >= Mathf.Abs(drag.y))
                    {
                        expansion = new Vector2Int(Mathf.Abs(drag.x) - sizeBorder.x + 1, 0);
                        expansion = new Vector2Int(Mathf.CeilToInt(expansion.x / (float)sizeStep.x) * sizeStep.x, 0);

                        if (StartAtMinimum)
                            expansion = new Vector2Int(Mathf.Max(expansionMinimum.x, expansion.x), Mathf.Max(expansionMinimum.y, expansion.y));

                        var deltaX = (expansion.x + sizeBorder.x - 1) * (int)Mathf.Sign(drag.x);

                        if (drag.x > 0 || expansion.x < 0)
                            point = dragStart;
                        else
                            point = new Vector2Int(dragStart.x + deltaX, dragStart.y);

                        if (drag.x > 0)
                            _rotation = new BuildingRotationRectangle(0);
                        else if (drag.x < 0)
                            _rotation = new BuildingRotationRectangle(2);
                    }
                    else
                    {
                        expansion = new Vector2Int(Mathf.Abs(drag.y) - sizeBorder.x + 1, 0);
                        expansion = new Vector2Int(Mathf.CeilToInt(expansion.x / (float)sizeStep.x) * sizeStep.x, 0);

                        if (StartAtMinimum)
                            expansion = new Vector2Int(Mathf.Max(expansionMinimum.x, expansion.x), Mathf.Max(expansionMinimum.y, expansion.y));

                        var deltaY = (expansion.x + sizeBorder.x - 1) * (int)Mathf.Sign(drag.y);

                        if (drag.y > 0 || expansion.x < 0)
                            point = dragStart;
                        else
                            point = new Vector2Int(dragStart.x, dragStart.y + deltaY);

                        if (drag.y > 0)
                            _rotation = new BuildingRotationRectangle(3);
                        else if (drag.y < 0)
                            _rotation = new BuildingRotationRectangle(1);
                    }
                }
            }
            else
            {
                point = mousePoint;
                expansion = Vector2Int.one - sizeBorder;

                if (StartAtMinimum)
                {
                    expansion = new Vector2Int(Mathf.Max(expansionMinimum.x, expansion.x), Mathf.Max(expansionMinimum.y, expansion.y));

                    if (_rotation.State == 1)
                    {
                        point = new Vector2Int(point.x, point.y - (expansion.x + sizeBorder.x - 1));
                    }
                    else if (_rotation.State == 2)
                    {
                        point = new Vector2Int(point.x - (expansion.x + sizeBorder.x - 1), point.y);
                    }
                }
            }

            var size = BuildingInfo.Size + expansion + BuildingInfo.SizePost;
            var structurePoints = PositionHelper.GetStructurePositions(point, _rotation.RotateSize(size));

            if (structurePoints.All(p => _map.IsInside(p)) && BuildingInfo.CheckExpansionLimits(expansion) && BuildingInfo.CheckExpandedRequirements(point, expansion, _rotation))
            {
                foreach (var structurePoint in structurePoints)
                {
                    if (BuildingInfo.CheckAvailability(structurePoint))
                        validPoints.Add(structurePoint);
                    else
                        invalidPoints.Add(structurePoint);
                }
            }
            else
            {
                invalidPoints.AddRange(structurePoints);
            }

            if (!checkCost(expansion))
            {
                invalidPoints.AddRange(validPoints);
                validPoints.Clear();
            }

            _highlighting.Clear();
            _highlighting.Highlight(validPoints, true);
            _highlighting.Highlight(invalidPoints, false);

            if (_ghost != null)
            {
                _ghost.GameObject.SetActive(BuildingInfo.CheckExpansionLimits(expansion));
                _ghost.GameObject.transform.position = Dependencies.Get<IGridPositions>().GetWorldPosition(_rotation.RotateOrigin(point, size));
                _ghost.GameObject.transform.rotation = _rotation.GetRotation();
                _ghost.UpdateVisuals(expansion);
            }

            if (isApply)
            {
                if (validPoints.Count > 0 && invalidPoints.Count == 0)
                    build(point, _rotation, expansion);
            }
        }

        protected virtual void recreateGhost()
        {
            if (_ghost != null)
            {
                Destroy(_ghost.GameObject);
                _ghost = null;
            }

            var prefab = BuildingInfo.GetGhost(_index);

            if (prefab)
            {
                _ghost = Instantiate(prefab).GetComponent<IExpandableVisual>();
                _ghost.GameObject.SetActive(false);
            }
        }

        protected virtual bool checkCost(Vector2Int expansion)
        {
            bool hasCost = true;
            _costs.Clear();

            foreach (var items in BuildingInfo.Cost)
            {
                _costs.AddQuantity(items.Item, items.Quantity);
                if (_globalStorage != null && !_globalStorage.Items.HasItemsRemaining(items.Item, items.Quantity))
                {
                    hasCost = false;
                }
            }

            var expansionCount = getExpansionCount(expansion);
            foreach (var items in BuildingInfo.ExpansionCost)
            {
                _costs.AddQuantity(items.Item, items.Quantity * expansionCount);
                if (_globalStorage != null && !_globalStorage.Items.HasItemsRemaining(items.Item, items.Quantity * expansionCount))
                {
                    hasCost = false;
                }
            }

            return hasCost;
        }

        protected virtual int getExpansionCount(Vector2Int expansion) => expansion.y == 0 ? expansion.x : expansion.x * expansion.y;

        protected virtual void build(Vector2Int point, BuildingRotation rotation, Vector2Int expansion)
        {
            var buildingManager = Dependencies.Get<IBuildingManager>();
            var gridPositions = Dependencies.Get<IGridPositions>();
            var size = BuildingInfo.Size + expansion + BuildingInfo.SizePost;

            onApplied();

            var undoRedoActions = UndoRedoActions.Create($"Build {BuildingInfo.Name} Building");

            if (_globalStorage != null)
            {
                foreach (var items in BuildingInfo.Cost)
                {
                    _globalStorage.Items.RemoveItems(items.Item, items.Quantity, undoRedoActions);
                }

                var expansionCount = getExpansionCount(expansion);
                foreach (var items in BuildingInfo.ExpansionCost)
                {
                    _globalStorage.Items.RemoveItems(items.Item, items.Quantity * expansionCount, undoRedoActions);
                }
            }

            BuildingInfo.PrepareExpanded(point, expansion, rotation);
            Built?.Invoke(buildingManager.Add(gridPositions.GetWorldPosition(rotation.RotateOrigin(point, size)), rotation.GetRotation(), BuildingInfo.Prefab, b => ((ExpandableBuilding)b).Expansion = expansion, undoRedoActions));

            undoRedoActions?.Push();
        }
    }
}
