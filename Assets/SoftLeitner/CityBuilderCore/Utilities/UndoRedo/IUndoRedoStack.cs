namespace CityBuilderCore
{
    /// <summary>
    /// optional interface that manages undo redo in games that support it
    /// </summary>
    public interface IUndoRedoStack
    {
        /// <summary>
        /// an action that can be undone is present and valid
        /// </summary>
        bool CanUndo { get; }
        /// <summary>
        /// an action that can be redone is present and valid
        /// </summary>
        bool CanRedo {  get; }

        /// <summary>
        /// puts an action into the stack<br/>
        /// it is now the next action that will be undone
        /// </summary>
        /// <param name="action">the action representing some process just performed by the player</param>
        void Push(IUndoRedoAction action);
        /// <summary>
        /// undoes the top action in the undo stack
        /// </summary>
        void Undo();
        /// <summary>
        /// redoes the last action that has been undone
        /// </summary>
        void Redo();
    }
}
