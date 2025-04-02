using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// base class for <see cref="IUndoRedoAction"/> that adds or removes items from global storage
    /// </summary>
    public class UndoRedoGlobalItems : UndoRedoActionBase
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnLoad()
        {
            UndoRedoTypes.Register("UndoRedoGlobalItems", typeof(UndoRedoGlobalItems));
        }

        private List<ItemQuantity> _items;

        public override string Name => "Items";

        public override bool CanUndo
        {
            get
            {
                if (!base.CanUndo)
                    return false;

                var storage = Dependencies.Get<IGlobalStorage>();
                foreach (var items in _items)
                {
                    if (items.Quantity > 0)//undo addition > remove > check quantity
                    {
                        if (storage.Items.GetItemQuantityRemaining(items.Item) < items.Quantity)
                            return false;
                    }

                    //dont check capacity, undoredo is allowed to overfill
                }

                return true;
            }
        }
        public override bool CanRedo
        {
            get
            {
                if (!base.CanRedo)
                    return false;

                var storage = Dependencies.Get<IGlobalStorage>();
                foreach (var items in _items)
                {
                    if (items.Quantity < 0)//redo removal > remove > check quantity
                    {
                        if (storage.Items.GetItemQuantityRemaining(items.Item) < -items.Quantity)
                            return false;
                    }

                    //dont check capacity, undoredo is allowed to overfill
                }

                return true;
            }
        }

        public UndoRedoGlobalItems(ItemQuantity items)
        {
            _items = new List<ItemQuantity>() { items };
        }
        public UndoRedoGlobalItems(Item item, int quantity)
        {
            _items = new List<ItemQuantity>() { new ItemQuantity(item, quantity) };
        }

        public void Combine(UndoRedoGlobalItems other)
        {
            foreach (var otherItems in other._items)
            {
                _items.AddQuantity(otherItems.Item, otherItems.Quantity);
            }
        }

        public override void Undo()
        {
            var storage = Dependencies.Get<IGlobalStorage>();
            foreach (var items in _items)
            {
                if (items.Quantity > 0)//undo addition > remove
                    storage.Items.RemoveItems(items);
                else if (items.Quantity < 0)//undo removal > add
                    storage.Items.AddItems(-items, true);
            }
        }
        public override void Redo()
        {
            var storage = Dependencies.Get<IGlobalStorage>();
            foreach (var items in _items)
            {
                if (items.Quantity > 0)//redo addition > add
                    storage.Items.AddItems(items, true);
                else if (items.Quantity < 0)//redo removal > remove
                    storage.Items.RemoveItems(-items);
            }
        }

        public static UndoRedoGlobalItems CreateAddition(ItemQuantity itemQuantity) => new UndoRedoGlobalItems(itemQuantity);
        public static UndoRedoGlobalItems CreateAddition(Item item, int quantity) => new UndoRedoGlobalItems(item, quantity);
        public static UndoRedoGlobalItems CreateRemoval(ItemQuantity itemQuantity) => new UndoRedoGlobalItems(itemQuantity.Item, -itemQuantity.Quantity);
        public static UndoRedoGlobalItems CreateRemoval(Item item, int quantity) => new UndoRedoGlobalItems(item, -quantity);

        #region Saving
        [Serializable]
        public class UndoRedoGlobalItemsData
        {
            public ItemQuantity.ItemQuantityData[] Items;
        }
        public override string SaveData()
        {
            return JsonUtility.ToJson(new UndoRedoGlobalItemsData() { Items = _items.Select(i => i.GetData()).ToArray() });
        }
        public override void LoadData(string json)
        {
            _items = JsonUtility.FromJson<UndoRedoGlobalItemsData>(json).Items.Select(i => i.GetItemQuantity()).ToList();
        }
        #endregion
    }
}
