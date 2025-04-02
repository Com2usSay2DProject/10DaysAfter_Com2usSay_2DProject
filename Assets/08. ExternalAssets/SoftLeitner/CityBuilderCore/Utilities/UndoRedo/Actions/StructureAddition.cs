using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// <see cref="IUndoRedoAction"/> that represents points being added to a structure
    /// </summary>
    public class StructureAddition : UndoRedoActionStructureBase
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnLoad()
        {
            UndoRedoTypes.Register("StructureAddition", typeof(StructureAddition));
        }

        public override string Name => "Add";

        public StructureAddition(IStructure structure, IEnumerable<Vector2Int> points) : base(structure, points) { }

        public override void Undo() => remove();
        public override void Redo() => add();
    }
}
