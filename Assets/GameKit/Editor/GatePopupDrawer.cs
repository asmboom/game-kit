using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Beetle23
{
    [CustomPropertyDrawer(typeof(GatePopupAttribute))]
    public class GatePopupDrawer : PropertyDrawer
    {
        public GatePopupDrawer()
        {
            _itemIDs = new List<string>();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string[] itemIDs = GetItemIDs();
            if (itemIDs.Length == 0)
            {
                EditorGUI.LabelField(position, ObjectNames.NicifyVariableName(property.name), None);
                return;
            }
            if (!string.IsNullOrEmpty(property.stringValue))
            {
                PopupAttribute.SelectedValue = GetIndex(itemIDs, property.stringValue);
            }
            PopupAttribute.SelectedValue = EditorGUI.Popup(position, label.text, PopupAttribute.SelectedValue, itemIDs);
            if (PopupAttribute.AllowNone && PopupAttribute.SelectedValue == 0)
            {
                property.stringValue = string.Empty;
            }
            else
            {
                property.stringValue = itemIDs[PopupAttribute.SelectedValue];
            }
        }

        private int GetIndex(string[] itemIDs, string itemID)
        {
            if (PopupAttribute.AllowNone && string.IsNullOrEmpty(itemID))
            {
                return 0;
            }

            int result = 0;
            for (int i = 0; i < itemIDs.Length; i++)
            {
                if (itemID == itemIDs[i])
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        private string[] GetItemIDs()
        {
            _itemIDs.Clear();
            if (PopupAttribute.AllowNone)
            {
                _itemIDs.Add(None);
            }

            foreach (var gate in GameKit.Config.Gates)
            {
                if (!gate.IsGroup || PopupAttribute.AllowGroup)
                {
                    _itemIDs.Add(gate.ID);
                }
            }

            return _itemIDs.ToArray();
        }

        private GatePopupAttribute PopupAttribute
        {
            get
            {
                return (GatePopupAttribute)attribute;
            }
        }

        private List<string> _itemIDs;

        private const string None = "none";
    }
}