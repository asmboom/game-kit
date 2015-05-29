using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Codeplay
{
    [CustomPropertyDrawer(typeof(WorldPopupAttribute))]
    public class WorldPopupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_itemPopupDrawer == null)
            {
                WorldPopupAttribute popupAttribute = (WorldPopupAttribute)attribute;
                _itemPopupDrawer = new ItemPopupDrawer(ItemType.World, 
                    popupAttribute.AllowNone, null);
            }
            property.stringValue = _itemPopupDrawer.Draw(position, property.stringValue, label);
        }

        private ItemPopupDrawer _itemPopupDrawer;
    }
}