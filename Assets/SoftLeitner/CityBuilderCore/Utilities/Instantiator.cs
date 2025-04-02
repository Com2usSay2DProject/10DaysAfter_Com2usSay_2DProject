using CityBuilderCore;
using System.Linq;
using UnityEngine;

namespace Assets.SoftLeitner.CityBuilderCore.Utilities
{
    public class Instantiator : MonoBehaviour
    {
        public Transform Parent;

        private LazyDependency<IGridPositions> _positions = new LazyDependency<IGridPositions>();
        private LazyOptionalDependency<IGridHeights> _heights = new LazyOptionalDependency<IGridHeights>();

        public void Instantiate()
        {
            Instantiate(transform.position);
        }

        public void Instantiate(Vector2Int[] points)
        {
            foreach (var point in points)
            {
                Instantiate(point);
            }
        }

        public void Instantiate(Vector3[] positions)
        {
            foreach (var position in positions)
            {
                Instantiate(position);
            }
        }

        public void Instantiate(Vector2Int point)
        {
            var position = _positions.Value.GetWorldCenterPosition(point);
            if (_heights.HasValue)
                position = _heights.Value.ApplyHeight(position);

            Instantiate(position);
        }

        public void Instantiate(IBuilding building)
        {
            Instantiate(building.GetPoints().ToArray());
        }

        public void Instantiate(IUndoRedoAction action)
        {
            foreach (var point in action.GetPoints())
            {
                Instantiate(point);
            }
        }

        public void Instantiate(Vector3 position)
        {
            Instantiate(gameObject, position, Quaternion.identity, Parent).SetActive(true);
        }
    }
}
