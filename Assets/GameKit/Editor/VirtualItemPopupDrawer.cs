using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Beetle23
{
    [CustomPropertyDrawer(typeof(VirtualItemPopupAttritube))]
    public class VirtualItemPopupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_itemPopupDrawer == null)
            {
                VirtualItemPopupAttritube popupAttribute = (VirtualItemPopupAttritube)attribute;
                _itemPopupDrawer = new ItemPopupDrawer(ItemType.VirtualItem, 
                    popupAttribute.AllowNone, popupAttribute.TypeInclude);
            }
            property.stringValue = _itemPopupDrawer.Draw(position, property.stringValue, label);
        }

        private ItemPopupDrawer _itemPopupDrawer;
    }
}
