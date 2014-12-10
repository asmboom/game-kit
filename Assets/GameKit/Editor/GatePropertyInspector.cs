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
            _subGatesDrawers = new List<ItemPopupDrawer>();

            _subGateListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand |
                ReorderableListFlags.ShowIndices);
            _subGateListControl.ItemInserted += OnInsertSubGate;
            _subGateListControl.ItemRemoving += OnRemoveSubGate;
        }

        protected override void DoOnExplorerSelectionChange(IItem item)
        {
            if (item != null)
            {
                Gate gate = item as Gate;
                UpdateItemPopupDrawer(gate.Type);

                _subGateListAdaptor = new GenericClassListAdaptor<string>(gate.SubGateIDs, 20,
                    () => { return string.Empty; },
                    (position, subGateID, index) =>
                    {
                        subGateID = _subGatesDrawers[index].Draw(new Rect(position.x, position.y, position.width * 0.5f, position.height), subGateID, GUIContent.none);
                        if (GUI.Button(new Rect(position.x + position.width * 0.5f + 10, position.y, 50, position.height), "Edit"))
                        {
                            _treeExplorer.SelectItem(GameKit.Config.GetGateByID(subGateID));
                        }
                        return subGateID;
                    });
            }
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
                    bool error = false;
                    if (newType == GateType.GateListOr || newType == GateType.GateListAnd)
                    {
                        Gate[] gateLists = GameKit.Config.FindGateListThatGateBelongsTo(gate);
                        if (gateLists.Length > 0)
                        {
                            EditorUtility.DisplayDialog("Error", 
                                "Not allowed to change to " + newType + 
                                ", because this gate is already included by a " + gateLists[0].Type + " [" 
                                    + gateLists[0].ID + "].", "OK");
                            error = true;
                        }
                    }
                    if (!error)
                    {
                        gate.Type = newType;
                        UpdateItemPopupDrawer(gate.Type);
                    }
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
                    yOffset += 20;
                }
                else if (gate.Type == GateType.VirtualItemGate)
                {
                    gate.RelatedNumber = Mathf.Max(0, 
                        (float)EditorGUI.IntField(new Rect(0, yOffset, width, 20), 
                            "Required Balance", (int)gate.RelatedNumber));
                    yOffset += 20;
                }
                else if (gate.Type == GateType.GateListAnd || gate.Type == GateType.GateListOr)
                {
                    EditorGUI.LabelField(new Rect(0, yOffset, width, yOffset), "Sub Gates");
                    yOffset += 20;
                    float height = _subGateListControl.CalculateListHeight(_subGateListAdaptor);
                    _subGateListControl.Draw(new Rect(0, yOffset, width, height), _subGateListAdaptor);
                    yOffset += height;
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
            UpdateSubGatesPopupDrawers();
        }

        private void OnInsertSubGate(object sender, ItemInsertedEventArgs args) 
        {
            UpdateSubGatesPopupDrawers();
        }

        private void OnRemoveSubGate(object sender, ItemRemovingEventArgs args) 
        { 
            UpdateSubGatesPopupDrawers();
        }

        private void UpdateSubGatesPopupDrawers()
        {
            _subGatesDrawers.Clear();
            Gate gate = _currentDisplayItem as Gate;
            if (gate.IsGroup)
            {
                for (int i = 0; i < gate.SubGateIDs.Count; i++)
                {
                    _subGatesDrawers.Add(new ItemPopupDrawer(ItemType.Gate, false, false));
                }
            }
        }

        private bool _isBasicPropertiesExpanded = true;
        private bool _isGatePropertyExpanded = true;
        private ItemPopupDrawer _itemPopupDrawer;
        private string _itemPopupLabel;

        private ReorderableListControl _subGateListControl;
        private GenericClassListAdaptor<string> _subGateListAdaptor;
        private List<ItemPopupDrawer> _subGatesDrawers;
    }
}
