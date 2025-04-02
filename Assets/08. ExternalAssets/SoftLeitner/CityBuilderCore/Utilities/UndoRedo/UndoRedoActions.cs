using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// combines multiple <see cref="IUndoRedoAction"/> into one<br/>
    /// is passed around, for example, during demolish to collect all the different removal actions
    /// </summary>
    public class UndoRedoActions : IUndoRedoAction
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnLoad()
        {
            UndoRedoTypes.Register("UndoRedoActions", typeof(UndoRedoActions));
        }

        public string Name { get; private set; }

        public virtual bool CanUndo => _actions.All(a => a.CanUndo);
        public virtual bool CanRedo => _actions.All(a => a.CanRedo);

        private List<IUndoRedoAction> _actions = new List<IUndoRedoAction>();

        public UndoRedoActions(string name = null)
        {
            _actions = new List<IUndoRedoAction>();
            Name = name;
        }
        public UndoRedoActions(IEnumerable<IUndoRedoAction> actions, string name = null)
        {
            _actions = actions.ToList();
            Name = name;
        }

        public void Add(IUndoRedoAction action)
        {
            if (action is UndoRedoGlobalItems items)
            {
                var existingItems = _actions.OfType<UndoRedoGlobalItems>().FirstOrDefault();
                if (existingItems != null)
                {
                    existingItems.Combine(items);
                    return;
                }
            }

            _actions.Add(action);
        }

        public void Undo()
        {
            foreach (var action in _actions)
            {
                action.Undo();
            }
        }

        public void Redo()
        {
            foreach (var action in _actions)
            {
                action.Redo();
            }
        }

        public void Push()
        {
            if (_actions.Count == 0)
                return;
            Dependencies.Get<IUndoRedoStack>().Push(this);
        }

        public IEnumerable<Vector2Int> GetPoints()
        {
            foreach (var action in _actions)
            {
                foreach (var point in action.GetPoints())
                {
                    yield return point;
                }
            }
        }

        public static UndoRedoActions Create(string name = null)
        {
            if (Dependencies.Contains<IUndoRedoStack>())
                return new UndoRedoActions(name);
            return null;
        }

        #region Saving
        [Serializable]
        public class UndoRedoActionsData
        {
            public UndoRedoActionData[] Actions;
        }

        public string SaveData()
        {
            return JsonUtility.ToJson(new UndoRedoActionsData()
            {
                Actions = _actions.Select(UndoRedoActionData.Serialize).ToArray()
            });
        }
        public void LoadData(string json)
        {
            var data = JsonUtility.FromJson<UndoRedoActionsData>(json);

            _actions = data.Actions.Select(UndoRedoActionData.Deserialize).ToList();
        }
        #endregion
    }
}
