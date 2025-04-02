using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// abstract base class for <see cref="IUndoRedoAction"/> that provides default implementations for some functions
    /// </summary>
    public abstract class UndoRedoActionBase : IUndoRedoAction
    {
        public abstract string Name { get; }

        public virtual bool CanUndo => true;
        public virtual bool CanRedo => true;

        public abstract void Undo();
        public abstract void Redo();

        public virtual IEnumerable<Vector2Int> GetPoints() => Enumerable.Empty<Vector2Int>();

        #region Saving
        public abstract string SaveData();
        public abstract void LoadData(string json);
        #endregion
    }
}
