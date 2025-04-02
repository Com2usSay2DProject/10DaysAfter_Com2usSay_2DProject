using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// base class for <see cref="IUndoRedoAction"/> that adds or removes a building from the map
    /// </summary>
    public abstract class UndoRedoActionBuildingBase : UndoRedoActionBase
    {
        public IBuilding Building
        {
            get
            {
                var reference = Dependencies.Get<IBuildingManager>().GetBuildingReference(new Guid(_data.Id));
                if (reference == null || !reference.HasInstance)
                    return null;
                return reference.Instance;
            }
        }

        private BuildingMetaData _data;

        public UndoRedoActionBuildingBase(IBuilding building)
        {
            _data = building.GetMetaData();
        }

        protected void add()
        {
            Dependencies.Get<IBuildingManager>().Add(_data)?.LoadData(_data.Data);
        }

        protected void remove()
        {
            Building.Terminate();
        }

        public override IEnumerable<Vector2Int> GetPoints()
        {
            var info = Dependencies.Get<IKeyedSet<BuildingInfo>>().GetObject(_data.Key);
            if (info == null)
                return Enumerable.Empty<Vector2Int>();
            return info.GetPoints(_data);
        }

        #region Saving
        public override string SaveData()
        {
            return JsonUtility.ToJson(_data);
        }
        public override void LoadData(string json)
        {
            _data = JsonUtility.FromJson<BuildingMetaData>(json);
        }
        #endregion
    }
}
