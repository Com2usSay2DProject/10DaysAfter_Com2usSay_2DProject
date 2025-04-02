using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// <see cref="IUndoRedoAction"/> that represents a building being created on the map
    /// </summary>
    public class BuildingAddition : UndoRedoActionBuildingBase
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnLoad()
        {
            UndoRedoTypes.Register("BuildingAddition", typeof(BuildingAddition));
        }

        public override bool CanUndo => base.CanUndo && Building != null;

        public override string Name => "Add";

        public BuildingAddition(IBuilding building) : base(building) { }

        public override void Undo() => remove();
        public override void Redo() => add();
    }
}
