using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CategoryPropertyView
{
    public CategoryPropertyView(VirtualCategory category)
    {
        _itemsWithoutCategory = new List<VirtualItem>();
        UpdateListAndSelection();
    }

    public void UpdateDisplayItem(VirtualCategory category)
    {
        _currentCategoryID = category.ID;
        UpdateListAndSelection();
    }

    public void Draw(Rect position, VirtualCategory category)
    {
        DrawCategoryID(category);

        float itemHeight = 20;
        float width = position.width * 0.4f;
        float height = position.height - 30;
        GUI.BeginGroup(new Rect(position.x, position.y + 30, width, height), string.Empty, "Box");
        _scrollPositionOfNonCategory = GUI.BeginScrollView(new Rect(0, 0, width, height),
            _scrollPositionOfNonCategory, new Rect(0, 0, width - 20, 20 * _itemsWithoutCategory.Count));
        float yOffset = 0;
        foreach (var item in _itemsWithoutCategory)
        {
            if (GUI.Button(new Rect(0, yOffset, position.width * 0.4f, itemHeight), item.ID, 
                item == _currentSelectedNonCategoryItem ? 
                    VirtualItemsDrawUtil.ItemSelectedStyle : VirtualItemsDrawUtil.ItemStyle))
            {
                _currentSelectedNonCategoryItem = item;
            }
            yOffset += itemHeight;
        }
        GUI.EndScrollView();
        GUI.EndGroup();

        GUI.enabled = _currentSelectedNonCategoryItem != null;
        if (GUI.Button(new Rect(position.width * 0.5f - 50, position.height * 0.5f - 30, 100, 20), "Add>"))
        {
            category.Items.Add(_currentSelectedNonCategoryItem);
            _currentSelectedCategoryItem = _currentSelectedNonCategoryItem;
            _currentSelectedNonCategoryItem = null;
            UpdateListAndSelection();
            EditorUtility.SetDirty(EconomyKit.Config);
        }
        GUI.enabled = _currentSelectedCategoryItem != null;
        if (_currentSelectedCategoryItem != null &&
            GUI.Button(new Rect(position.width * 0.5f - 50, position.height * 0.5f + 30, 100, 20), "<Remove"))
        {
            category.Items.Remove(_currentSelectedCategoryItem);
            _currentSelectedNonCategoryItem = _currentSelectedCategoryItem;
            _currentSelectedCategoryItem = null;
            UpdateListAndSelection();
            EditorUtility.SetDirty(EconomyKit.Config);
        }
        GUI.enabled = true;

        GUI.BeginGroup(new Rect(position.x + position.width - width, position.y + 30, width, height), string.Empty, "Box");
        _scrollPositionOfCategory = GUI.BeginScrollView(new Rect(0, 0, width, height),
            _scrollPositionOfCategory, new Rect(0, 0, width - 20, 20 * category.Items.Count));
        yOffset = 0;
        foreach (var item in category.Items)
        {
            if (GUI.Button(new Rect(0, yOffset, position.width * 0.4f, itemHeight), item.ID, 
                item == _currentSelectedNonCategoryItem ? 
                    VirtualItemsDrawUtil.ItemSelectedStyle : VirtualItemsDrawUtil.ItemStyle))
            {
                _currentSelectedCategoryItem = item;
            }
            yOffset += itemHeight;
        }
        GUI.EndScrollView();
        GUI.EndGroup();
    }

    private void DrawCategoryID(VirtualCategory category)
    {
        string newId = EditorGUILayout.TextField("Unique ID", _currentCategoryID);
        if (!newId.Equals(_currentCategoryID))
        {
            _currentCategoryID = newId;
            VirtualCategory categoryWithID = EconomyKit.Config.GetCategoryByID(_currentCategoryID);
            if (categoryWithID != null && categoryWithID != category)
            {
                GUIUtility.keyboardControl = 0;
                EditorUtility.DisplayDialog("Duplicate ID", "An category with ID[" + _currentCategoryID + "] already exists!!!", "OK");
                _currentCategoryID = category.ID;
            }
            else
            {
                category.ID = _currentCategoryID;
            }
        }
    }

    private void UpdateListAndSelection()
    {
        EconomyKit.Config.UpdateIdToItemMap();
        EconomyKit.Config.UpdateIdToCategoryMap();
        _itemsWithoutCategory.Clear();
        foreach (var item in EconomyKit.Config.Items)
        {
            if (item.Category == null)
            {
                _itemsWithoutCategory.Add(item);
            }
        }
    }

    private string _currentCategoryID;
    private VirtualCategory _currentDisplayedCategory;
    private List<VirtualItem> _itemsWithoutCategory;
    private Vector2 _scrollPositionOfNonCategory;
    private Vector2 _scrollPositionOfCategory;
    private VirtualItem _currentSelectedNonCategoryItem;
    private VirtualItem _currentSelectedCategoryItem;
}