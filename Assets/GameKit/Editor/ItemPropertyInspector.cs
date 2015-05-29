using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Codeplay
{
    public abstract class ItemPropertyInspector
    {
        public ItemPropertyInspector(ItemTreeExplorer treeExplorer)
        {
            _treeExplorer = treeExplorer;
            _currentDisplayItem = treeExplorer.CurrentSelectedItem;
        }

        public string GetAffectedItemsWarningString(IItem[] items)
        {
            string warning = "\n";
            foreach (var item in items)
            {
                warning += "\t(" + item.GetType() + ")" + item.ID + "\n";
            }
            return warning;
        }

        public void OnExplorerSelectionChange(IItem item)
        {
            _currentDisplayItem = item;
            if (item != null)
            {
                _currentItemID = item.ID;
            }
            DoOnExplorerSelectionChange(_currentDisplayItem);
        }

        public void Draw(Rect position)
        {
            GUI.BeginGroup(position, string.Empty, "Box");
            if (_currentDisplayItem != null)
            {
                GUI.BeginGroup(new Rect(10, 0, position.width - 10, position.height));
                _scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width - 10, position.height),
                    _scrollPosition, new Rect(0, 0, position.width - 30, _currentYOffset));

                bool showScrollbar = position.height < _currentYOffset;
                float width = position.width - 10 - (showScrollbar ? 20 : 10);

                _currentYOffset = DoDrawItem(new Rect(0, 0, width, position.height), _currentDisplayItem);

                GUI.EndScrollView();
                GUI.EndGroup();
            }
            GUI.EndGroup();
        }

        public void DrawIDField(IItem item, bool isEditable, bool isUnique)
        {
            RestrictID();
            if (isEditable)
            {
                if (isUnique)
                {
                    GUI.SetNextControlName(IDInputControlName);
                    if (EditorGUILayout.TextField(IDLabelName,
                        _currentItemID).KeyPressed<string>(IDInputControlName, KeyCode.Return, out _currentItemID) ||
                        (GUI.GetNameOfFocusedControl() != IDInputControlName &&
                         _currentItemID != item.ID))
                    {
                        OnGetNewIDFromTextField(item);
                    }
                }
                else
                {
                    item.ID = EditorGUILayout.TextField(IDLabelName, item.ID);
                }
            }
            else
            {
                EditorGUILayout.LabelField(IDLabelName, item.ID);
            }
        }

        public void DrawIDField(Rect position, IItem item, bool isEditable, bool isUnique)
        {
            RestrictID();
            if (isEditable)
            {
                if (isUnique)
                {
                    GUI.SetNextControlName(IDInputControlName);
                    if (EditorGUI.TextField(position, IDLabelName,
                        _currentItemID).KeyPressed<string>(IDInputControlName, KeyCode.Return, out _currentItemID) ||
                        (Event.current.type != EventType.Layout &&
                         GUI.GetNameOfFocusedControl() != IDInputControlName &&
                         _currentItemID != item.ID))
                    {
                        OnGetNewIDFromTextField(item);
                    }
                }
                else
                {
                    item.ID = EditorGUI.TextField(position, IDLabelName, item.ID);
                }
            }
            else
            {
                EditorGUI.LabelField(position, IDLabelName, item.ID);
            }
        }

        public abstract IItem[] GetAffectedItems(string itemID);
        protected abstract void DoOnExplorerSelectionChange(IItem item);
        protected abstract float DoDrawItem(Rect rect, IItem item);
        protected abstract IItem GetItemWithConflictingID(IItem item, string id);

        protected void UpdateGateID(Gate gate)
        {
            if (_currentDisplayItem != null)
            {
                gate.ID = string.Format("gate_{0}", _currentDisplayItem.ID);
                if (gate.IsGroup)
                {
                    for (int i = 0; i < gate.SubGates.Count; i++)
                    {
                        gate.SubGates[i].ID = string.Format("{0}_{1}", gate.ID, i);
                    }
                }
            }
        }

        private void RestrictID()
        {
            _currentItemID = System.Text.RegularExpressions.Regex.Replace(_currentItemID.Trim(), @"[^a-zA-Z0-9_]", "");
        }

        private void OnGetNewIDFromTextField(IItem item)
        {
            _currentItemID = _currentItemID.Trim();
            if (string.IsNullOrEmpty(_currentItemID))
            {
                EditorUtility.DisplayDialog("Invlid ID", "ID couldn't be empty.", "OK");
                _currentItemID = item.ID;
                GUI.FocusControl(IDInputControlName);
                return;
            }

            IItem itemWithID = GetItemWithConflictingID(item, _currentItemID);
            if (itemWithID != null && itemWithID != item)
            {
                GUIUtility.keyboardControl = 0;
                EditorUtility.DisplayDialog("Duplicate ID", "A " + item.GetType().ToString() + " with ID[" +
                    _currentItemID + "] already exists!!!", "OK");
                _currentItemID = item.ID;
            }
            else
            {
                IItem[] affectedItems = GetAffectedItems(item.ID);
                if (affectedItems.Length == 0 ||
                    EditorUtility.DisplayDialog("Renaming ID will affect the following items, are you sure to change?",
                        "Affected items: " + GetAffectedItemsWarningString(affectedItems), "OK", "Cancel"))
                {
                    UpdateRelatedIDOfItems(affectedItems, item.ID, _currentItemID);
                    item.ID = _currentItemID;
                    GameKit.Config.UpdateMapsAndTree();
                }
                else
                {
                    _currentItemID = item.ID;
                }
            }
            GameKitEditorWindow.GetInstance().Repaint();
        }

        private void UpdateRelatedIDOfItems(IItem[] items, string oldID, string newID)
        {
            foreach (var item in items)
            {
                if (item is Gate)
                {
                    Gate gate = item as Gate;
                    if (gate.RelatedItemID.Equals(oldID))
                    {
                        gate.RelatedItemID = newID;
                    }
                }
                else if (item is Reward)
                {
                    Reward reward = item as Reward;
                    if (reward.RelatedItemID.Equals(oldID))
                    {
                        reward.RelatedItemID = newID;
                    }
                }
                else if (item is Score)
                {
                    Score score = item as Score;
                    if (score.RelatedVirtualItemID.Equals(oldID))
                    {
                        score.RelatedVirtualItemID = newID;
                    }
                }
                else if (item is VirtualCategory)
                {
                    VirtualCategory category = item as VirtualCategory;
                    for (int i = 0; i < category.ItemIDs.Count; i++)
                    {
                        if (category.ItemIDs[i].Equals(oldID))
                        {
                            category.ItemIDs[i] = newID;
                        }
                    }
                }
                else if (item is VirtualItem)
                {
                    if (item is PurchasableItem)
                    {
                        PurchasableItem purchsable = item as PurchasableItem;
                        for (int i = 0; i < purchsable.PurchaseInfo.Count; i++)
                        {
                            if (!purchsable.PurchaseInfo[i].IsMarketPurchase &&
                                purchsable.PurchaseInfo[i].VirtualCurrencyID.Equals(oldID))
                            {
                                purchsable.PurchaseInfo[i].VirtualCurrencyID = newID;
                            }
                        }
                    }
                    if (item is VirtualItemPack)
                    {
                        VirtualItemPack pack = item as VirtualItemPack;
                        for (int i = 0; i < pack.PackElements.Count; i++)
                        {
                            if (pack.PackElements[i].ItemID.Equals(oldID))
                            {
                                pack.PackElements[i].ItemID = newID;
                            }
                        }
                    }
                }
            }
        }

        protected ItemTreeExplorer _treeExplorer;
        protected IItem _currentDisplayItem;
        protected string _currentItemID;
        private Vector2 _scrollPosition;
        private float _currentYOffset;

        private const string IDLabelName = "Unique ID";
        private const string IDInputControlName = "game_kit_id_field";
    }
}