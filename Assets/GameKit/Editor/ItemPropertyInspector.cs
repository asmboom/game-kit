using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
{
    public abstract class ItemPropertyInspector
    {
        public ItemPropertyInspector(ItemTreeExplorer treeExplorer)
        {
            _treeExplorer = treeExplorer;
            _currentDisplayItem = treeExplorer.CurrentSelectedItem;
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

        public void DrawIDField(Rect position, IItem item, bool isEditable, bool isUnique)
        {
            if (isEditable)
            {
                if (isUnique)
                {
                    GUI.SetNextControlName(IDInputControlName);
                    if (EditorGUI.TextField(position, "Unique ID",
                        _currentItemID).KeyPressed<string>(IDInputControlName, KeyCode.Return, out _currentItemID) ||
                        (GUI.GetNameOfFocusedControl() != IDInputControlName &&
                         _currentItemID != item.ID))
                    {
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
                            item.ID = _currentItemID;
                            GameKitEditorWindow.GetInstance().Repaint();
                        }
                    }
                }
                else
                {
                    item.ID = EditorGUI.TextField(position, "Unique ID", item.ID);
                }
            }
            else
            {
                EditorGUI.LabelField(position, "Unique ID", item.ID);
            }
        }

        protected abstract void DoOnExplorerSelectionChange(IItem item);
        protected abstract float DoDrawItem(Rect rect, IItem item);
        protected abstract IItem GetItemWithConflictingID(IItem item, string id);

        protected ItemTreeExplorer _treeExplorer;
        protected IItem _currentDisplayItem;
        protected string _currentItemID;
        private Vector2 _scrollPosition;
        private float _currentYOffset;
        private const string IDInputControlName = "game_kit_id_field";
    }
}