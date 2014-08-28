using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class VirtualItemsEditorWindow : EditorWindow
{
    [MenuItem("Window/Virtual Items Editor Window")]
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
        _selectedItemTypeIndex = -1;
        _itemTypes = new string[] { "Virtual Currency", "Single Use", "LifeTime Item", "Pack", "Categories" };

        VirtualItemsEditUtil.UpdateDisplayedOptions();
    }

    private void OnGUI()
    {
        int newSelectedCategoryIdx = Mathf.Max(0, GUILayout.SelectionGrid(_selectedItemTypeIndex, _itemTypes, _itemTypes.Length));
        if (newSelectedCategoryIdx != _selectedItemTypeIndex)
        {
            var items = new List<VirtualItem>();
            switch (newSelectedCategoryIdx)
            {
                case 0:
                    foreach (var item in _config.VirtualCurrencies)
                    {
                        items.Add(item);
                    }
                    SetCurrentListView(new VirtualCurrencyListView(_config.VirtualCurrencies));
                    break;
                case 1:
                    foreach (var item in _config.SingleUseItems)
                    {
                        items.Add(item);
                    }
                    SetCurrentListView(new SingleUseItemListView(_config.SingleUseItems));
                    break;
                case 2:
                    foreach (var item in _config.LifeTimeItems)
                    {
                        items.Add(item);
                    }
                    SetCurrentListView(new LifeTimeItemListView(_config.LifeTimeItems));
                    break;
                case 3:
                    foreach (var item in _config.ItemPacks)
                    {
                        items.Add(item);
                    }
                    SetCurrentListView(new VirtualItemPackListView(_config.ItemPacks));
                    break;
                case 4:
                    SetCurrentListView(new CategoryListView(_config.Categories));
                    break;
                default:
                    break;
            }
        }

        _currentListView.Draw(position);
        _selectedItemTypeIndex = newSelectedCategoryIdx;
    }

    private void SetCurrentListView(IView view)
    {
        if (_currentListView != null)
        {
            _currentListView.Hide();
        }
        _currentListView = view;
        if (_currentListView != null)
        {
            _currentListView.Show();
        }
    }

    private VirtualItemsConfig _config;
    private int _selectedItemTypeIndex;
    private string[] _itemTypes;

    private IView _currentListView;

    private const float RowHeight = 20;
}
