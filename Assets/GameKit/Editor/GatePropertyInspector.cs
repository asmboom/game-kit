using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
{
    public class GatePropertyInspector : ItemPropertyInspector
    {
        public GatePropertyInspector(GateTreeExplorer treeExplorer)
            :base(treeExplorer)
        {
        }

        protected override void DoOnExplorerSelectionChange(IItem item)
        {
        }

        protected override float DoDrawItem(Rect rect, IItem item)
        {
            float yOffset = 0;
            float width = rect.width;
            Gate gate = item as Gate;

            _isBasicPropertiesExpanded = EditorGUI.Foldout(new Rect(0, 0, width, 20),
                _isBasicPropertiesExpanded, "Item");
            yOffset += 20;
            if (_isBasicPropertiesExpanded)
            {
                DrawIDField(new Rect(0, yOffset, width, 20), gate, true, true);
                yOffset += 20;
                gate.Name = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Name", gate.Name);
                yOffset += 20;
            }
            yOffset += 20;

            _isGatePropertyExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20),
                _isGatePropertyExpanded, "Gate");
            yOffset += 20;
            if (_isGatePropertyExpanded)
            {
                gate.Type = (GateType)EditorGUI.EnumPopup(new Rect(0, yOffset, width, 20), "Type", gate.Type);
            }

            return yOffset;
        }

        protected override IItem GetItemWithConflictingID(IItem item, string id)
        {
            return GameKit.Config.GetGateByID(id);
        }

        private bool _isBasicPropertiesExpanded = true;
        private bool _isGatePropertyExpanded = true;
    }
}
