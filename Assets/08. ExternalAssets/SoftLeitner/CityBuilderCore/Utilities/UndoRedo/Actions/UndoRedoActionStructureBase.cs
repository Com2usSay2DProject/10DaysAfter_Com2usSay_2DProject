using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// base class for <see cref="IUndoRedoAction"/> that adds or removes points from a structure
    /// </summary>
    public abstract class UndoRedoActionStructureBase : UndoRedoActionBase
    {
        public IStructure Structure => Dependencies.Get<IStructureManager>().GetStructure(_key);

        private string _key;
        private Vector2Int[] _points;

        public UndoRedoActionStructureBase(IStructure structure, IEnumerable<Vector2Int> points)
        {
            _key = structure.Key;
            _points = points.ToArray();
        }

        protected virtual void add()
        {
            Structure.Add(_points);
        }

        protected virtual void remove()
        {
            Structure.Remove(_points);
        }

        public override IEnumerable<Vector2Int> GetPoints() => _points;

        #region Saving
        [Serializable]
        public class UndoRedoStructureData
        {
            public string Key;
            public Vector2Int[] Points;
        }
        public override string SaveData()
        {
            return JsonUtility.ToJson(new UndoRedoStructureData()
            {
                Key = _key,
                Points = _points
            });
        }
        public override void LoadData(string json)
        {
            var data = JsonUtility.FromJson<UndoRedoStructureData>(json);

            _key = data.Key;
            _points = data.Points;
        }
        #endregion
    }
}
