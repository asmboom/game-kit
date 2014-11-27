using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Beetle23
{
    public class CategoryPropertyView
    {
        public CategoryPropertyView(VirtualCategory category)
        {
            _itemsWithoutCategory = new List<VirtualItem>();
            _categoryItemListControl = new ReorderableListControl(ReorderableListFlags.HideAddButton |
                ReorderableListFlags.HideRemoveButtons | ReorderableListFlags.DisableDuplicateCommand);
            UpdateItemsWithoutCategory();
        }

        public void UpdateDisplayItem(VirtualCategory category)
        {
            _currentCategory = category;
            _currentCategoryID = category.ID;
            _categoryItemListAdaptor = new GenericClassListAdaptor<string>(category.ItemIDs, 20, null, DrawItemInCategory);
            UpdateItemsWithoutCategory();
        }

        public void Draw(Rect position, VirtualCategory category)
        {
            DrawCategoryID(category);
            category.Name = EditorGUILayout.TextField("Name", category.Name);

            float itemHeight = 20;
            float width = position.width * 0.4f;
            float height = position.height - 30;

            CheckAndRemoveInvalidIdsInCategory(_currentCategory);

            GUI.BeginGroup(new Rect(position.x, position.y + 50, width, height));
            _scrollPositionOfCategory = GUI.BeginScrollView(new Rect(0, 0, width, height),
                _scrollPositionOfCategory, new Rect(0, 0, width - 20, 20 * category.ItemIDs.Count));
            GUI.Label(new Rect(0, 0, width, 20), "In Category", GameKitEditorDrawUtil.TitleStyle);
            _categoryItemListControl.Draw(new Rect(0, 20, width, height - 20), _categoryItemListAdaptor);
            GUI.EndScrollView();
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(position.x + position.width - width - 20, position.y + 50, width, height));
            GUI.Label(new Rect(0, 0, width, 20), "Not in Category", GameKitEditorDrawUtil.TitleStyle);
            GUI.BeginGroup(new Rect(0, 20, width, height - 20), string.Empty, "Box");
            _scrollPositionOfNonCategory = GUI.BeginScrollView(new Rect(0, 0, width, height - 20),
                _scrollPositionOfNonCategory, new Rect(0, 0, width - 20, 20 * _itemsWithoutCategory.Count));
            float yOffset = 0;
            foreach (var item in _itemsWithoutCategory)
            {
                if (GUI.Button(new Rect(0, yOffset, position.width * 0.4f, itemHeight), item.ID,
                    item == _currentSelectedItem ?
                        GameKitEditorDrawUtil.ItemSelectedCenterStyle : GameKitEditorDrawUtil.ItemCenterLabelStyle))
                {
                    _currentSelectedItem = item;
                    _isCurrentSelectedItemInCategory = false;
                }
                yOffset += itemHeight;
            }
            GUI.EndScrollView();
            GUI.EndGroup();
            GUI.EndGroup();

            GUI.enabled = _currentSelectedItem != null && !_isCurrentSelectedItemInCategory;
            if (GUI.Button(new Rect(position.width * 0.5f - 50, position.height * 0.5f - 30, 100, 20), "<Add"))
            {
                category.ItemIDs.Add(_currentSelectedItem.ID);
                _isCurrentSelectedItemInCategory = true;
                UpdateItemsWithoutCategory();
                EditorUtility.SetDirty(GameKit.Config);
            }
            GUI.enabled = _currentSelectedItem != null && _isCurrentSelectedItemInCategory;
            if (_currentSelectedItem != null &&
                GUI.Button(new Rect(position.width * 0.5f - 50, position.height * 0.5f + 30, 100, 20), "Remove>"))
            {
                category.ItemIDs.Remove(_currentSelectedItem.ID);
                _isCurrentSelectedItemInCategory = false;
                UpdateItemsWithoutCategory();
                EditorUtility.SetDirty(GameKit.Config);
            }
            GUI.enabled = true;
        }

        private void CheckAndRemoveInvalidIdsInCategory(VirtualCategory category)
        {
            List<string> toBeRemoved = new List<string>();
            foreach (var itemId in category.ItemIDs)
            {
                if (GameKit.Config.GetVirtualItemByID(itemId) == null)
                {
                    toBeRemoved.Add(itemId);
                }
            }
            foreach (var itemId in toBeRemoved)
            {
                Debug.LogWarning("Category [" + category.ID + "]'s item [" + itemId + 
                    "] doesn't exist, remove it.");
                category.ItemIDs.Remove(itemId);
            }
        }

        private string DrawItemInCategory(Rect position, string itemID, int index)
        {
            if (GUI.Button(position, itemID,
                _currentSelectedItem != null && itemID == _currentSelectedItem.ID ?
                    GameKitEditorDrawUtil.ItemSelectedCenterStyle : GameKitEditorDrawUtil.ItemCenterLabelStyle))
            {
                _currentSelectedItem = GameKit.Config.GetVirtualItemByID(itemID);
                _isCurrentSelectedItemInCategory = true;
            }
            return itemID;
        }

        private void DrawCategoryID(VirtualCategory category)
        {
            GUI.SetNextControlName(IDInputControlName);
            if (EditorGUILayout.TextField("ID",
                _currentCategoryID).KeyPressed<string>(IDInputControlName, KeyCode.Return, out _currentCategoryID) ||
                (GUI.GetNameOfFocusedControl() != IDInputControlName &&
                 _currentCategoryID != category.ID))
            {
                VirtualCategory categoryWithID = GameKit.Config.GetCategoryByID(_currentCategoryID);
                if (categoryWithID != null && categoryWithID != category)
                {
                    GUIUtility.keyboardControl = 0;
                    EditorUtility.DisplayDialog("Duplicate ID", "A category with ID[" +
                        _currentCategoryID + "] already exists!!!", "OK");
                    _currentCategoryID = category.ID;
                }
                else
                {
                    category.ID = _currentCategoryID;
                    GameKitEditorWindow.GetInstance().Repaint();
                }
            }
        }

        private void UpdateItemsWithoutCategory()
        {
            GameKit.Config.UpdateIdToItemMap();
            GameKit.Config.UpdateIdToCategoryMap();
            _itemsWithoutCategory.Clear();
            foreach (var item in GameKit.Config.VirtualItems)
            {
                if (item.Category == null)
                {
                    _itemsWithoutCategory.Add(item);
                }
            }
        }

        private VirtualCategory _currentCategory;
        private string _currentCategoryID;
        private GenericClassListAdaptor<string> _categoryItemListAdaptor;
        private ReorderableListControl _categoryItemListControl;
        private VirtualCategory _currentDisplayedCategory;
        private List<VirtualItem> _itemsWithoutCategory;
        private Vector2 _scrollPositionOfNonCategory;
        private Vector2 _scrollPositionOfCategory;
        private VirtualItem _currentSelectedItem;
        private bool _isCurrentSelectedItemInCategory;

        private const string IDInputControlName = "virtual_item_id_field";
    }
}