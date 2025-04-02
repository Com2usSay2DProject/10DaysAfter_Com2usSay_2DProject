using UnityEditor;
using UnityEngine;

namespace CityBuilderCore.Editor
{
    [CustomEditor(typeof(DefaultItemManager), true)]
    [CanEditMultipleObjects]
    public class DefaultItemManagerEditor : UnityEditor.Editor
    {
        private Item _item;
        private int _quantity = 1;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var store = (DefaultItemManager)target;

            EditorGUILayout.LabelField("Items:");

            EditorGUILayout.BeginHorizontal();
            _item = (Item)EditorGUILayout.ObjectField(_item, typeof(Item), false);
            _quantity = EditorGUILayout.IntField(_quantity);
            if (GUILayout.Button("+") && _item != null)
                store.Items.AddItems(new ItemQuantity(_item, _quantity));
            if (GUILayout.Button("-") && _item != null)
                store.Items.RemoveItems(new ItemQuantity(_item, _quantity));
            EditorGUILayout.EndHorizontal();

            foreach (var itemQuantity in store.Items.GetItemQuantities())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(itemQuantity.Item.Name);
                EditorGUILayout.LabelField(itemQuantity.Quantity.ToString());
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}