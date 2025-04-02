using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// <see cref="IUndoRedoAction"/> that represents points being removed to a structure
    /// </summary>
    public class StructureRemoval : UndoRedoActionStructureBase
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnLoad()
        {
            UndoRedoTypes.Register("StructureRemoval", typeof(StructureRemoval));
        }

        public override string Name => "Remove";

        public StructureRemoval(IStructure structure, IEnumerable<Vector2Int> points) : base(structure, points) { }

        public override void Undo() => add();
        public override void Redo() => remove();
    }
}
