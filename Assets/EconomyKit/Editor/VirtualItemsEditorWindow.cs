using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class VirtualItemsEditorWindow : EditorWindow
{
    [MenuItem("Window/Open Virtual Items Editor Window")]
    private static void OpenVirutalItemEditorWindow()
    {
        EditorWindow.GetWindow<VirtualItemsEditorWindow>("Virtual Item Edit Window");
    }

    private void OnEnable()
    {
        if (_config == null)
        {
            _config = EconomyKit.Config;
        }
        _currentItemDetails = new List<VirtualItemInfo>();
        _itemTypes = new string[] { "Virtual Currency", "Single Use", "LifeTime Item", "Upgrade Item", "Pack" };
        UpdateCategories();
    }

    private void UpdateItems(List<VirtualItem> items)
    {
        _currentItemDetails.Clear();

        foreach (var item in items)
        {
            _currentItemDetails.Add(new VirtualItemInfo()
            {
                Item = item,
                CategoryIndex = 0,
            });
        } 
    }

    private int GetCategoryIndexById(string categoryId)
    {
        for(int i = 0; i < _categories.Length; i++)
        {
            if (_categories[i].Equals(categoryId))
            {
                return i;
            }
        }
        Debug.LogError("Failed find categogy id: [" + categoryId + "]");
        return 0;
    }

    private void UpdateCategories()
    {
        List<string> categoryTitles = new List<string>();
        foreach (var category in _config.Categories)
        {
            categoryTitles.Add(category.ID);
        }
        _categories = categoryTitles.ToArray();
    }

    private void OnGUI()
    {
        int newSelectedCategoryIdx = GUILayout.SelectionGrid(_selectedCategoryIdx, _itemTypes, _itemTypes.Length);
        switch (newSelectedCategoryIdx)
        {
            case 0:
                DrawVirtualCurrencies();
                break;
            default:
                break;
        }
        _selectedCategoryIdx = newSelectedCategoryIdx;
    }

    private void DrawVirtualCurrencies()
    {
        GUILayout.BeginHorizontal(GUILayout.MaxWidth(800));

        List<VirtualItem> items = new List<VirtualItem>();
        foreach (var item in _config.VirtualCurrencies)
        {
            items.Add(item as VirtualItem);
        } 
        DrawIDs(items);
        DrawNames(items);
        DrawDescriptions(items);
        DrawCategories(items);

        GUILayout.EndHorizontal();
    }

    private void DrawIDs(List<VirtualItem> items)
    {
        GUILayout.BeginVertical(GUILayout.MaxWidth(100));
        GUILayout.Label("ID");
        foreach (var virtualItem in items)
        {
            virtualItem.ID = GUILayout.TextField(virtualItem.ID);
        }
        GUILayout.EndVertical();
    }

    private void DrawNames(List<VirtualItem> items)
    {
        GUILayout.BeginVertical(GUILayout.MaxWidth(100));
        GUILayout.Label("Name");
        foreach (var virtualItem in items)
        {
            virtualItem.Name = GUILayout.TextField(virtualItem.Name);
        }
        GUILayout.EndVertical();
    }

    private void DrawDescriptions(List<VirtualItem> items)
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Description");
        foreach (var virtualItem in items)
        {
            virtualItem.Description = GUILayout.TextField(virtualItem.Description);
        }
        GUILayout.EndVertical();
    }

    private void DrawCategories(List<VirtualItem> items)
    {
        GUILayout.BeginVertical(GUILayout.MaxWidth(100));
        GUILayout.Label("Categories");
        foreach (var virtualItem in items)
        {
            int index = 0;
            index = EditorGUILayout.Popup(index, _categories);
        }
        GUILayout.EndVertical();
    }

    private class VirtualItemInfo
    {
        public VirtualItem Item;
        public int CategoryIndex;
    }

    private VirtualItemsConfig _config;
    private int _selectedCategoryIdx;
    private string[] _itemTypes;
    private string[] _categories;
    private List<VirtualItemInfo> _currentItemDetails;
}