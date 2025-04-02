using System;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// holds undo redo data when the game is saved<br/>
    /// in addition to the actual data actions also save their name<br/>
    /// this is needed to deserialize the correct type using <see cref="UndoRedoTypes"/>
    /// </summary>
    [Serializable]
    public class UndoRedoActionData
    {
        public string Type;
        public string Data;

        public static UndoRedoActionData Serialize(IUndoRedoAction action)
        {
            return new UndoRedoActionData()
            {
                Type = UndoRedoTypes.GetName(action),
                Data = action.SaveData(),
            };
        }
        public static IUndoRedoAction Deserialize(UndoRedoActionData actionData)
        {
            var action = (IUndoRedoAction)JsonUtility.FromJson(actionData.Data, UndoRedoTypes.GetType(actionData.Type));
            action.LoadData(actionData.Data);
            return action;
        }
    }
}
