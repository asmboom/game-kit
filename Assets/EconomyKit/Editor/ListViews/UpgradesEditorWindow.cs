using Rotorz.ReorderableList;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UpgradesEditorWindow : EditorWindow
{
    public void Init(VirtualItem item)
    {
        _currentEditItem = item;
        _listAdaptor = new GenericClassListAdaptor<UpgradeItem>(_currentEditItem.Upgrades, 22, 
            CreateUpgradeItem, DrawItem);
    }

    private void OnEnable()
    {
        _listControl = new ReorderableListControl();
        _listControl.ItemInserted += OnItemInsert;
        _listControl.ItemRemoving += OnItemRemoving;
    }

    private void OnDisable()
    {
        if (_listControl != null)
        {
            _listControl.ItemInserted -= OnItemInsert;
            _listControl.ItemRemoving -= OnItemRemoving;
        }
    }

    private void OnItemRemoving(object sender, ItemRemovingEventArgs args)
    {
        if (EditorUtility.DisplayDialog("Confirm to delete",
                "Confirm to delete virtual item [" + _listAdaptor[args.itemIndex].Name + "]?", "OK", "Cancel"))
        {
            args.Cancel = false;
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_listAdaptor[args.itemIndex]));
        }
        else
        {
            args.Cancel = true;
        }
    }

    private void OnItemInsert(object sender, ItemInsertedEventArgs args)
    {
        string prefix = (args.itemIndex + 1) < 10 ? "0" + (args.itemIndex + 1) : (args.itemIndex + 1).ToString();
        _listAdaptor[args.itemIndex].ID = string.Format("{0}Upgrade0{1}", _currentEditItem.ID, prefix);
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_listAdaptor[args.itemIndex]), _listAdaptor[args.itemIndex].ID);
        _listAdaptor[args.itemIndex].Name = string.Format("Upgrade {0} to level {1}", _currentEditItem.Name, args.itemIndex + 2);
        _listAdaptor[args.itemIndex].Description = _listAdaptor[args.itemIndex].Name;
    }

    private void OnGUI()
    {
        if (_currentEditItem == null || _listAdaptor == null) return;

        float yOffset = 30;
        float width = 850;
        float listHeight = _listControl.CalculateListHeight(_listAdaptor);

        var centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.richText = true;
        GUI.Label(new Rect(0, 0, width, 30), 
            "<size=15><color=lightblue>" + _currentEditItem.Name + "</color> upgrades</size>");
        centeredStyle.richText = false;

        _scrollPosition = GUI.BeginScrollView(new Rect(0, yOffset, position.width, position.height - yOffset), 
            _scrollPosition, new Rect(0, yOffset, width, listHeight + 20));

        DrawTitle(new Rect(0, yOffset, width, 20));
        EditorGUI.BeginChangeCheck();
        _listControl.Draw(new Rect(0, yOffset + 20, width, listHeight), _listAdaptor);
        if (EditorGUI.EndChangeCheck() && ReorderableListGUI.indexOfChangedItem != -1)
        {
            EditorUtility.SetDirty(_listAdaptor[ReorderableListGUI.indexOfChangedItem]);
        }

        GUI.EndScrollView();
    }

    public UpgradeItem CreateUpgradeItem()
    {
        return VirtualItemsEditUtil.CreateNewVirtualItem<UpgradeItem>();
    }

    public UpgradeItem DrawItem(Rect position, UpgradeItem item, int index)
    {
        float xOffset = VirtualItemsDrawUtil.DrawVirtualItemInfo(position.x, position.y, position.height, item, index, null, false);
        VirtualItemsDrawUtil.DrawPurchase(xOffset, position.y, position.height, false, item);
        return item;
    }

    private void DrawTitle(Rect position)
    {
        VirtualItemsDrawUtil.BeginDrawTitle();
        float xOffset = VirtualItemsDrawUtil.DrawVirtualItemTitle(position.x, position.y, position.height, false);
        VirtualItemsDrawUtil.DrawPurchase(xOffset, position.y, position.height, true, null);
        VirtualItemsDrawUtil.EndDrawTitle();
    }

    private VirtualItem _currentEditItem;
    private ReorderableListControl _listControl;
    private GenericClassListAdaptor<UpgradeItem> _listAdaptor;
    private Vector2 _scrollPosition;
}
