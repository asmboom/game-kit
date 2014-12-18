using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
{
    public class GatePropertyView
    {
        public GatePropertyView(Gate gate, bool allowSubgates)
        {
            _currentGate = gate;
            if (_currentGate != null)
            {
                UpdateItemPopupDrawer(gate.Type);
            }

            _allowSubgates = allowSubgates;
            if (_allowSubgates)
            {
                _subGatesDrawers = new List<GatePropertyView>();

                _subGateListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand |
                    ReorderableListFlags.ShowIndices);
                _subGateListControl.ItemInserted += OnInsertSubGate;
                _subGateListControl.ItemRemoving += OnRemoveSubGate;
            }
        }

        public void UpdateDisplayItem(Gate gate)
        {
            _currentGate = gate;

            UpdateItemPopupDrawer(gate.Type);

            if (_allowSubgates)
            {
                _subGateListAdaptor = new GenericClassListAdaptor<Gate>(gate.SubGates, 20,
                    () => { return new Gate(); },
                    (position, subGate, index) =>
                    {
                        if (index < _subGatesDrawers.Count)
                        {
                            _subGatesDrawers[index].Draw(new Rect(position.x + 10, position.y, position.width * 0.5f, position.height), subGate);
                        }
                        return subGate;
                    },
                    (subGate) =>
                    {
                        if (_subGatesDrawers.Count > 0)
                        {
                            return _subGatesDrawers[0].CalculateHeight(subGate);
                        }
                        return 20;
                    });
            }
        }

        public float CalculateHeight(Gate gate)
        {
            return DrawGate(gate, 0, true);
        }

        public float Draw(Rect rect, Gate gate)
        {
            GUI.BeginGroup(rect);
            float yOffset = DrawGate(gate, rect.width, false);
            GUI.EndGroup();
            return yOffset;
        }

        private float DrawGate(Gate gate, float width, bool calculateHeight)
        {
            float yOffset = 0;

            if (!calculateHeight)
            {
                if (_allowSubgates)
                {
                    GateType newType = (GateType)EditorGUI.EnumPopup(new Rect(0, yOffset, width, 20), "Type", gate.Type);
                    if (newType != gate.Type)
                    {
                        gate.Type = newType;
                        UpdateItemPopupDrawer(gate.Type);
                    }
                }
                else
                {
                    GateType newType = (GateType)EditorGUI.EnumPopup(new Rect(0, yOffset, width, 20), "Type", (SubGateType)gate.Type);
                    if (newType != gate.Type)
                    {
                        gate.Type = newType;
                        UpdateItemPopupDrawer(gate.Type);
                    }
                }
            }
            yOffset += 20;

            if (gate.Type != GateType.None)
            {
                if (!calculateHeight)
                {
                    EditorGUI.LabelField(new Rect(0, yOffset, width, 20), new GUIContent("ID"), new GUIContent(gate.ID));
                }
                yOffset += 20;
            }

            if (_itemPopupDrawer != null)
            {
                if (!calculateHeight)
                {
                    gate.RelatedItemID = _itemPopupDrawer.Draw(new Rect(0, yOffset, width, 20),
                        gate.RelatedItemID, new GUIContent(_itemPopupLabel));
                }
                yOffset += 20;
            }
            if (gate.Type == GateType.ScoreGate)
            {
                if (!calculateHeight)
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
                yOffset += 20;
            }
            else if (gate.Type == GateType.VirtualItemGate)
            {
                if (!calculateHeight)
                {
                    gate.RelatedNumber = Mathf.Max(0,
                        (float)EditorGUI.IntField(new Rect(0, yOffset, width, 20),
                            "Required Balance", (int)gate.RelatedNumber));
                }
                yOffset += 20;
            }
            else if (gate.Type == GateType.GateListAnd || gate.Type == GateType.GateListOr)
            {
                if (!calculateHeight)
                {
                    EditorGUI.LabelField(new Rect(0, yOffset, width, yOffset), "Sub Gates");
                }
                yOffset += 20;
                float height = _subGateListControl.CalculateListHeight(_subGateListAdaptor);
                if (!calculateHeight)
                {
                    _subGateListControl.Draw(new Rect(0, yOffset, width, height), _subGateListAdaptor);
                }
                yOffset += height;
            }

            return yOffset;
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
            if (_allowSubgates)
            {
                _subGatesDrawers.Clear();
                if (_currentGate != null &&
                    _currentGate.IsGroup)
                {
                    for (int i = 0; i < _currentGate.SubGates.Count; i++)
                    {
                        _subGatesDrawers.Add(new GatePropertyView(_currentGate.SubGates[i], false));
                    }
                }
            }
        }

        private Gate _currentGate;
        private ItemPopupDrawer _itemPopupDrawer;
        private string _itemPopupLabel;

        private bool _allowSubgates;
        private ReorderableListControl _subGateListControl;
        private GenericClassListAdaptor<Gate> _subGateListAdaptor;
        private List<GatePropertyView> _subGatesDrawers;

        private enum SubGateType
        {
            ScoreGate = GateType.ScoreGate,
            VirtualItemGate = GateType.VirtualItemGate,
            WorldCompletionGate = GateType.WorldCompletionGate,
            PurchasableGate = GateType.PurchasableGate,
        }
    }
}