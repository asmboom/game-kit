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
            Gate gate = item as Gate;
            UpdateItemPopupDrawer(gate.Type);
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
                GateType newType = (GateType)EditorGUI.EnumPopup(new Rect(0, yOffset, width, 20), "Type", gate.Type);
                yOffset += 20;
                if (newType != gate.Type)
                {
                    gate.Type = newType;
                    UpdateItemPopupDrawer(gate.Type);
                }
                if (_itemPopupDrawer != null)
                {
                    gate.RelatedItemID = _itemPopupDrawer.Draw(new Rect(0, yOffset, width, 20), 
                        gate.RelatedItemID, new GUIContent(_itemPopupLabel));
                    yOffset += 20;
                }
                if (gate.Type == GateType.ScoreGate)
                {
                    Score score = gate.RelatedItem as Score;
                    if (score != null && score.EnableClamp)
                    {
                        gate.RelatedNumber = EditorGUI.Slider(new Rect(0, yOffset, width, 20),
                            "Required Score", gate.RelatedNumber, score.Min, score.Max);
                    }
                    else
                    {
                        gate.RelatedNumber = EditorGUI.FloatField(new Rect(0, yOffset, width, 20), 
                            "Required Score", gate.RelatedNumber);
                    }
                }
                else if (gate.Type == GateType.VirtualItemGate)
                {
                    gate.RelatedNumber = Mathf.Max(0, 
                        (float)EditorGUI.IntField(new Rect(0, yOffset, width, 20), 
                            "Required Balance", (int)gate.RelatedNumber));
                }
            }

            return yOffset;
        }

        protected override IItem GetItemWithConflictingID(IItem item, string id)
        {
            return GameKit.Config.GetGateByID(id);
        }

        private void UpdateItemPopupDrawer(GateType gateType)
        {
            switch (gateType)
            {
                case GateType.ScoreGate:
                    _itemPopupDrawer = new ItemPopupDrawer(ItemType.Score, false, null);
                    _itemPopupLabel = "Score";
                    break;
                case GateType.VirtualItemGate:
                    _itemPopupDrawer = new ItemPopupDrawer(ItemType.VirtualItem, false, 
                        VirtualItemType.VirtualCurrency | VirtualItemType.SingleUseItem);
                    _itemPopupLabel = "Virtual Item";
                    break;
                case GateType.WorldCompletionGate:
                    _itemPopupDrawer = new ItemPopupDrawer(ItemType.World, false, null);
                    _itemPopupLabel = "World";
                    break;
                case GateType.PurchasableGate:
                    _itemPopupDrawer = new ItemPopupDrawer(ItemType.VirtualItem, false, 
                        VirtualItemType.LifeTimeItem);
                    _itemPopupLabel = "Purchasable Item";
                    break;
                default:
                    _itemPopupDrawer = null;
                    break;
            }
        }

        private bool _isBasicPropertiesExpanded = true;
        private bool _isGatePropertyExpanded = true;
        private ItemPopupDrawer _itemPopupDrawer;
        private string _itemPopupLabel;
    }
}
