using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// represents an action that can be undone and redone using an <see cref="IUndoRedoStack"></see>
    /// </summary>
    public interface IUndoRedoAction : ISaveData
    {
        /// <summary>
        /// whether the action is currently valid to be undone<br/>
        /// for example building a hut cannot be undone if that hut has since been destroyed
        /// </summary>
        bool CanUndo { get; }
        /// <summary>
        /// whether the action is currently valid to be redone
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// name of the action to be displayed in the ui(tooltip)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// undoes the action
        /// </summary>
        void Undo();
        /// <summary>
        /// redoes the action after it has been undone
        /// </summary>
        void Redo();

        /// <summary>
        /// determines and returns the map points affected by this action<br/>
        /// used, for example, to spawn particles at these points
        /// </summary>
        /// <returns>points affected by the action</returns>
        IEnumerable<Vector2Int> GetPoints();
    }
}
