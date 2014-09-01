using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

public class VirtualItemsPropertyInspector
{
    public VirtualItemsPropertyInspector(object currentDisplayedItem)
    {
        _currentDisplayedItem = currentDisplayedItem;
        _purchaseListView = new PurchaseInfoListView(currentDisplayedItem as PurchasableItem);
        _packListView = new PackInfoListView(currentDisplayedItem as VirtualItemPack);
        _upgradesListView = new UpgradesListView(currentDisplayedItem as VirtualItem);
        _categoryPropertyView = new CategoryPropertyView(currentDisplayedItem as VirtualCategory);
    }

    public void OnExplorerSelectionChange(object item)
    {
        _currentDisplayedItem = item;

        if (item is VirtualItem)
        {
            _currentItemID = (item as VirtualItem).ID;
            if (item is SingleUseItem || item is LifeTimeItem)
            {
                _upgradesListView.UpdateDisplayItem(item as VirtualItem);
            }
            if (item is PurchasableItem)
            {
                if (item is VirtualItemPack)
                {
                    _packListView.UpdateDisplayItem(item as VirtualItemPack);
                }
                _purchaseListView.UpdateDisplayItem(item as PurchasableItem);
            }
        }
        else if (item is VirtualCategory)
        {
            _categoryPropertyView.UpdateDisplayItem(item as VirtualCategory);
        }
    }

    public void Draw(Rect position)
    {
        GUILayout.BeginArea(position, string.Empty, "Box");

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        VirtualItem item = _currentDisplayedItem as VirtualItem;
        if (item != null)
        {
            DrawVirtualItem(item);
        }
        else
        {
            VirtualCategory category = _currentDisplayedItem as VirtualCategory;
            if (category != null)
            {
                _categoryPropertyView.Draw(new Rect(0, 0, position.width, position.height), category);
            }
        }

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    private void DrawVirtualItem(VirtualItem item)
    {
        EditorGUI.BeginChangeCheck();

        _isVirtualItemPropertiesExpanded = EditorGUILayout.Foldout(_isVirtualItemPropertiesExpanded, "Basic Property");
        if (_isVirtualItemPropertiesExpanded)
        {
            EditorGUILayout.LabelField("Hash", item.HashID);
            DrawID(item);
            item.Name = EditorGUILayout.TextField("Name", item.Name);
            item.Description = EditorGUILayout.TextField("Desription", item.Description);
            EditorGUILayout.LabelField("Category", item.Category == null ? "None" : item.Category.ID);
            item.Icon = EditorGUILayout.ObjectField("Icon", item.Icon, typeof(Sprite), false) as Sprite;
        }
        if (item is LifeTimeItem)
        {
            GUILayout.Space(10);

            LifeTimeItem lifetimeItem = item as LifeTimeItem;
            lifetimeItem.IsEquippable = EditorGUILayout.Toggle("Is Equippable", lifetimeItem.IsEquippable);
        }
        else if (item is VirtualItemPack)
        {
            GUILayout.Space(10);

            _isPackInfoExpanded = EditorGUILayout.Foldout(_isPackInfoExpanded, "Pack Info");
            if (_isPackInfoExpanded)
            {
                _packListView.Draw();
            }
        }
        if (item is PurchasableItem)
        {
            GUILayout.Space(10);

            _isPurchaseInfoExpanded = EditorGUILayout.Foldout(_isPurchaseInfoExpanded, "Purchase Info");
            if (_isPurchaseInfoExpanded)
            {
                _purchaseListView.Draw();
            }
        }
        if (item is SingleUseItem || item is LifeTimeItem)
        {
            GUILayout.Space(10);

            _isUpgradeInfoExpanded = EditorGUILayout.Foldout(_isUpgradeInfoExpanded, "Upgrade Info (" + item.Upgrades.Count + " levels)");
            if (_isUpgradeInfoExpanded)
            {
                _upgradesListView.Draw();
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(item);
        }
    }

    private void DrawID(VirtualItem item)
    {
        GUI.SetNextControlName(item.HashID);
        if (EditorGUILayout.TextField("Unique ID", _currentItemID).KeyPressed<string>(item.HashID, KeyCode.Return, out _currentItemID))
        {
            EconomyKit.Config.UpdateIdToItemMap();
            VirtualItem itemWithID = EconomyKit.Config.GetItemByID(_currentItemID);
            if (itemWithID != null && itemWithID != item)
            {
                GUIUtility.keyboardControl = 0;
                EditorUtility.DisplayDialog("Duplicate ID", "An item with ID[" + _currentItemID + "] already exists!!!", "OK");
                _currentItemID = item.ID;
            }
            else
            {
                item.ID = _currentItemID;
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), item.ID);
                VirtualItemsEditorWindow.GetInstance().Repaint();
            }
        }
    }

    private string _currentItemID;
    private Vector2 _scrollPosition;
    private bool _isVirtualItemPropertiesExpanded = true;
    private bool _isPackInfoExpanded = true;
    private bool _isPurchaseInfoExpanded = true;
    private bool _isUpgradeInfoExpanded = false;

    private object _currentDisplayedItem;

    private PurchaseInfoListView _purchaseListView;
    private PackInfoListView _packListView;
    private UpgradesListView _upgradesListView;
    private CategoryPropertyView _categoryPropertyView;
}
