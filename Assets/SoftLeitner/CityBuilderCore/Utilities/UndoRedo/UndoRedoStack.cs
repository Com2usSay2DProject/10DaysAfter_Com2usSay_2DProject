using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CityBuilderCore
{
    /// <summary>
    /// manages and executes <see cref="IUndoRedoAction"/>s
    /// </summary>
    /// <remarks><see href="https://citybuilder.softleitner.com/manual">https://citybuilder.softleitner.com/manual</see></remarks>
    [HelpURL("https://citybuilderapi.softleitner.com/class_city_builder_core_1_1_undo_redo_stack.html")]
    public class UndoRedoStack : ExtraDataBehaviour, IUndoRedoStack
    {
        [Tooltip("maximum number of actions in the undo stack")]
        public int Size = 10;
        [Tooltip("gets enabled when the stack has undo actions, undo is performed on click")]
        public Button UndoButton;
        [Tooltip("gets enabled when the stack has redo actions, redo is performed on click")]
        public Button RedoButton;
        [Tooltip("gets the name of the current undo action as its text and the next one as its description")]
        public TooltipArea UndoArea;
        [Tooltip("gets the name of the current redo action as its text and the next one as its description")]
        public TooltipArea RedoArea;

        [Tooltip("fried when an action has been undone(can be used to spawn particles at affected points)")]
        public UnityEvent<IUndoRedoAction> Undone;
        [Tooltip("fried when an action has been redone(can be used to spawn particles at affected points)")]
        public UnityEvent<IUndoRedoAction> Redone;

        public bool CanUndo => _done.Count > 0 && _done.Last.Value.CanUndo;
        public bool CanRedo => _undone.Count > 0 && _undone.Last.Value.CanRedo;

        private LinkedList<IUndoRedoAction> _done = new LinkedList<IUndoRedoAction>();
        private LinkedList<IUndoRedoAction> _undone = new LinkedList<IUndoRedoAction>();

        private Dictionary<string, Type> _typeDict = new Dictionary<string, Type>();
        private Dictionary<Type, string> _typeNameDict = new Dictionary<Type, string>();

        private bool _canUndo;
        private bool _canRedo;

        private void Awake()
        {
            UndoButton?.onClick.AddListener(new UnityAction(Undo));
            RedoButton?.onClick.AddListener(new UnityAction(Redo));

            onChanged();

            Dependencies.Register<IUndoRedoStack>(this);

            _typeNameDict = _typeDict.ToDictionary(k => k.Value, v => v.Key);
        }

        private void Update()
        {
            //in case of changes outside of the stack
            //(for example building destroyed by tornado in urban)
            if (_canUndo != CanUndo || _canRedo != CanRedo)
                onChanged();
        }

        public void Push(IUndoRedoAction action)
        {
            _done.AddLast(action);
            if (_done.Count > Size)
                _done.RemoveFirst();
            _undone.Clear();
            onChanged();
        }

        public void Undo()
        {
            if (!CanUndo)
                return;

            var action = _done.Last.Value;
            _done.RemoveLast();

            action.Undo();
            _undone.AddLast(action);
            onChanged();

            Undone?.Invoke(action);
        }

        public void Redo()
        {
            if (!CanRedo)
                return;

            var action = _undone.Last.Value;
            _undone.RemoveLast();

            action.Redo();
            _done.AddLast(action);
            onChanged();

            Redone?.Invoke(action);
        }

        private void onChanged()
        {
            _canUndo = CanUndo;
            _canRedo = CanRedo;

            if (UndoButton)
                UndoButton.interactable = CanUndo;
            if (RedoButton)
                RedoButton.interactable = CanRedo;

            if (UndoArea)
            {
                string name, description;

                if (CanUndo)
                {
                    name = "Undo: " + _done.Last.Value.Name;
                    if (_done.Count > 1)
                        description = "Next: " + _done.Skip(1).Select(a => a.Name).First();
                    else
                        description = string.Empty;
                }
                else
                {
                    name = "Undo";
                    description = string.Empty;
                }

                UndoArea.Set(name, description);
            }

            if (RedoArea)
            {
                string name, description;

                if (CanRedo)
                {
                    name = "Redo: " + _undone.Last.Value.Name;
                    if (_undone.Count > 1)
                        description = "Next: " + _undone.Skip(1).Select(a => a.Name).First();
                    else
                        description = string.Empty;
                }
                else
                {
                    name = "Redo";
                    description = string.Empty;
                }

                RedoArea.Set(name, description);
            }
        }

        #region Saving
        [Serializable]
        public class UndoRedoStackData
        {
            public UndoRedoActionData[] Done;
            public UndoRedoActionData[] Undone;
        }

        public override string SaveData()
        {
            return JsonUtility.ToJson(new UndoRedoStackData()
            {
                Done = _done.Select(UndoRedoActionData.Serialize).ToArray(),
                Undone = _undone.Select(UndoRedoActionData.Serialize).ToArray()
            });
        }
        public override void LoadData(string json)
        {
            var data = JsonUtility.FromJson<UndoRedoStackData>(json);

            _done = new LinkedList<IUndoRedoAction>(data.Done.Select(UndoRedoActionData.Deserialize));
            _undone = new LinkedList<IUndoRedoAction>(data.Undone.Select(UndoRedoActionData.Deserialize));

            onChanged();
        }
        #endregion
    }
}
