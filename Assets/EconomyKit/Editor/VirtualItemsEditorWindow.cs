using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class VirtualItemsEditorWindow : EditorWindow
{
    [MenuItem("Window/Virtual Items Editor Window")]
    private static void OpenVirutalItemEditorWindow()
    {
        VirtualItemsEditorWindow.GetInstance(); 
    }

    public static VirtualItemsEditorWindow GetInstance()
    {
        if (_instance == null)
        {
            _instance = EditorWindow.GetWindow<VirtualItemsEditorWindow>("Virtual Item Edit Window");
        }
        return _instance;
    }

    private static VirtualItemsEditorWindow _instance;

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
        if (_itemInspector == null)
        {
            _itemInspector = new VirtualItemsPropertyInspector(_itemsExplorer.CurrentSelectedItem);
            _itemsExplorer.OnSelectionChange += _itemInspector.OnExplorerSelectionChange;
        }

        VirtualItemsEditUtil.UpdateDisplayedOptions();
    }

    private void OnDisable()
    {
        _itemsExplorer.OnSelectionChange -= _itemInspector.OnExplorerSelectionChange;
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
        _itemsExplorer.Draw(new Rect(10, 5, 250, position.height - 10));
        if (_itemInspector != null)
        {
            _itemInspector.Draw(new Rect(270, 5, position.width - 280, position.height - 10));
        }
    }

    private VirtualItemsConfig _config;
    private VirtualItemsTreeExplorer _itemsExplorer;
    private VirtualItemsPropertyInspector _itemInspector;

    private const float RowHeight = 20;
}
