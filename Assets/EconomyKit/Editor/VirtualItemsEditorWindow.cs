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
        GetVirtualItemsConfigAndCreateIfNonExist();

        if (_config == null)
        {
            _config = EconomyKit.Config;
        }
        if (_itemsExplorer == null)
        {
            _itemsExplorer = new VirtualItemsTreeExplorer(_config);
        }
        _selectedItemTypeIndex = -1;
        _itemTypes = new string[] { "Virtual Currency", "Single Use", "LifeTime Item", "Pack", "Categories" };

        VirtualItemsEditUtil.UpdateDisplayedOptions();
    }

    private static VirtualItemsConfig GetVirtualItemsConfigAndCreateIfNonExist()
    {
        string configFilePath = VirtualItemsEditUtil.DefaultVirtualItemDataPath + "/VirtualItemsConfig.asset";
        VirtualItemsConfig virtualItemsConfig = AssetDatabase.LoadAssetAtPath(configFilePath, typeof(VirtualItemsConfig)) as VirtualItemsConfig;
        if (virtualItemsConfig == null)
        {
            virtualItemsConfig = VirtualItemsEditUtil.CreateAsset<VirtualItemsConfig>(configFilePath);
        }
        return virtualItemsConfig;
    }

    private void OnGUI()
    {
        int newSelectedCategoryIdx = Mathf.Max(0, GUILayout.SelectionGrid(_selectedItemTypeIndex, _itemTypes, _itemTypes.Length));
        if (newSelectedCategoryIdx != _selectedItemTypeIndex)
        {
            switch (newSelectedCategoryIdx)
            {
                case 0:
                    SetCurrentListView(new VirtualCurrencyListView(_config.VirtualCurrencies));
                    break;
                case 1:
                    SetCurrentListView(new SingleUseItemListView(_config.SingleUseItems));
                    break;
                case 2:
                    SetCurrentListView(new LifeTimeItemListView(_config.LifeTimeItems));
                    break;
                case 3:
                    SetCurrentListView(new VirtualItemPackListView(_config.ItemPacks));
                    break;
                case 4:
                    SetCurrentListView(new CategoryListView(_config.Categories));
                    break;
                default:
                    break;
            }
        }

        if (_currentListView != null)
        {
            //_currentListView.Draw(position);
        }
        _itemsExplorer.Draw(new Rect(10, 30, 200, position.height));
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
    private VirtualItemsTreeExplorer _itemsExplorer;

    private const float RowHeight = 20;
}
