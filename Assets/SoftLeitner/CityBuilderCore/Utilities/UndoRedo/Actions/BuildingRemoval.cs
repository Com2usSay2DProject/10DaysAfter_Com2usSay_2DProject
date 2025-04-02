using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// <see cref="IUndoRedoAction"/> that represents a building being removed on the map
    /// </summary>
    public class BuildingRemoval : UndoRedoActionBuildingBase
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnLoad()
        {
            UndoRedoTypes.Register("BuildingRemoval", typeof(BuildingRemoval));
        }

        public override bool CanRedo => base.CanRedo && Building != null;

        public override string Name => "Remove";

        public BuildingRemoval(IBuilding building) : base(building) { }

        public override void Undo() => add();
        public override void Redo() => remove();
    }
}
