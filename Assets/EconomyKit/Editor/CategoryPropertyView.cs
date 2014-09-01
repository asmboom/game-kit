using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;

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
        _currentCategoryID = category.ID;
        _categoryItemListAdaptor = new GenericClassListAdaptor<VirtualItem>(category.Items, 20, null, DrawItemInCategory);
        UpdateItemsWithoutCategory();
    }

    public void Draw(Rect position, VirtualCategory category)
    {
        DrawCategoryID(category);

        float itemHeight = 20;
        float width = position.width * 0.4f;
        float height = position.height - 30;

        GUI.BeginGroup(new Rect(position.x, position.y + 30, width, height));
        _scrollPositionOfCategory = GUI.BeginScrollView(new Rect(0, 0, width, height),
            _scrollPositionOfCategory, new Rect(0, 0, width - 20, 20 * category.Items.Count));
        GUI.Label(new Rect(0, 0, width, 20), "In Category", VirtualItemsDrawUtil.TitleStyle);
        _categoryItemListControl.Draw(new Rect(0, 20, width, height - 20), _categoryItemListAdaptor);
        GUI.EndScrollView();
        GUI.EndGroup();

        GUI.BeginGroup(new Rect(position.x + position.width - width - 20, position.y + 30, width, height));
        GUI.Label(new Rect(0, 0, width, 20), "Not in Category", VirtualItemsDrawUtil.TitleStyle);
        GUI.BeginGroup(new Rect(0, 20, width, height - 20), string.Empty, "Box");
        _scrollPositionOfNonCategory = GUI.BeginScrollView(new Rect(0, 0, width, height - 20),
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
        GUI.EndGroup();

        GUI.enabled = _currentSelectedNonCategoryItem != null;
        if (GUI.Button(new Rect(position.width * 0.5f - 50, position.height * 0.5f - 30, 100, 20), "<Add"))
        {
            category.Items.Add(_currentSelectedNonCategoryItem);
            _currentSelectedCategoryItem = _currentSelectedNonCategoryItem;
            _currentSelectedNonCategoryItem = null;
            UpdateItemsWithoutCategory();
            EditorUtility.SetDirty(EconomyKit.Config);
        }
        GUI.enabled = _currentSelectedCategoryItem != null;
        if (_currentSelectedCategoryItem != null &&
            GUI.Button(new Rect(position.width * 0.5f - 50, position.height * 0.5f + 30, 100, 20), "Remove>"))
        {
            category.Items.Remove(_currentSelectedCategoryItem);
            _currentSelectedNonCategoryItem = _currentSelectedCategoryItem;
            _currentSelectedCategoryItem = null;
            UpdateItemsWithoutCategory();
            EditorUtility.SetDirty(EconomyKit.Config);
        }
        GUI.enabled = true;
    }

    private VirtualItem DrawItemInCategory(Rect position, VirtualItem item, int index)
    {
        if (GUI.Button(position, item.ID, 
            item == _currentSelectedCategoryItem ? 
                VirtualItemsDrawUtil.ItemSelectedStyle : VirtualItemsDrawUtil.ItemStyle))
        {
            _currentSelectedCategoryItem = item;
        }
        return item;
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
                EditorUtility.DisplayDialog("Duplicate ID", "An category with ID[" + 
                    _currentCategoryID + "] already exists!!!", "OK");
                _currentCategoryID = category.ID;
            }
            else
            {
                category.ID = _currentCategoryID;
            }
        }
    }

    private void UpdateItemsWithoutCategory()
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
    private GenericClassListAdaptor<VirtualItem> _categoryItemListAdaptor;
    private ReorderableListControl _categoryItemListControl;
    private VirtualCategory _currentDisplayedCategory;
    private List<VirtualItem> _itemsWithoutCategory;
    private Vector2 _scrollPositionOfNonCategory;
    private Vector2 _scrollPositionOfCategory;
    private VirtualItem _currentSelectedNonCategoryItem;
    private VirtualItem _currentSelectedCategoryItem;
}