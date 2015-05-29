using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Codeplay
{
    [CustomPropertyDrawer(typeof(ScorePopupAttribute))]
    public class ScorePopupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_itemPopupDrawer == null)
            {
                ScorePopupAttribute popupAttribute = (ScorePopupAttribute)attribute;
                _itemPopupDrawer = new ItemPopupDrawer(ItemType.Score, 
                    popupAttribute.AllowNone, null);
            }
            property.stringValue = _itemPopupDrawer.Draw(position, property.stringValue, label);
        }

        private ItemPopupDrawer _itemPopupDrawer;
    }
}