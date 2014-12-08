using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Beetle23
{
    [CustomPropertyDrawer(typeof(GatePopupAttribute))]
    public class GatePopupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_itemPopupDrawer == null)
            {
                GatePopupAttribute popupAttribute = (GatePopupAttribute)attribute;
                _itemPopupDrawer = new ItemPopupDrawer(ItemType.Gate, 
                    popupAttribute.AllowNone, popupAttribute.AllowGroup);
            }
            property.stringValue = _itemPopupDrawer.Draw(position, property.stringValue, label);
        }

        private ItemPopupDrawer _itemPopupDrawer;
    }
}