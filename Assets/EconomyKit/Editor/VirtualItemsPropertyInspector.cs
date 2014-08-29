using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

public class VirtualItemsPropertyInspector
{
    public VirtualItemsPropertyInspector(ScriptableObject currentDisplayedItem)
    {
        _currentDisplayedItem = currentDisplayedItem;
        _purchaseListView = new PurchaseInfoListView(currentDisplayedItem as PurchasableItem);
        _packListView = new PackInfoListView(currentDisplayedItem as VirtualItemPack);
        _upgradesListView = new UpgradesListView(currentDisplayedItem as VirtualItem);
    }

    public void OnExplorerSelectionChange(ScriptableObject item)
    {
        _currentDisplayedItem = item;

        if (item is VirtualItem &&
            !(item is VirtualItemPack || item is UpgradeItem))
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
                DrawVirtualCategory(category);
            }
        }

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    private void DrawVirtualItem(VirtualItem item)
    {
        _isVirtualItemPropertiesExpanded = EditorGUILayout.Foldout(_isVirtualItemPropertiesExpanded, "Basic Property");
        if (_isVirtualItemPropertiesExpanded)
        {
            DrawID(item);
            DrawName(item);
            DrawDesciption(item);
            DrawItemCategory(item);
            DrawIcon(item);
        }
        if (item is LifeTimeItem)
        {
            GUILayout.Space(10);

            LifeTimeItem lifetimeItem = item as LifeTimeItem;
            DrawIsEquippable(lifetimeItem);
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
        if (!(item is VirtualItemPack || item is UpgradeItem))
        {
            GUILayout.Space(10);

            _isUpgradeInfoExpanded = EditorGUILayout.Foldout(_isUpgradeInfoExpanded, "Upgrade Info (" + item.Upgrades.Count + " levels)");
            if (_isUpgradeInfoExpanded)
            {
                _upgradesListView.Draw();
            }
        }
    }

    private static void DrawID(VirtualItem item)
    {
        string controlName = item.GetInstanceID() + "_input_field";
        GUI.SetNextControlName(controlName);
        if (EditorGUILayout.TextField("Unique ID", item.ID).KeyPressed<string>(controlName, KeyCode.Return, out item.ID))
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), item.ID);
        }
    }

    private static void DrawName(VirtualItem item)
    {
        item.Name = EditorGUILayout.TextField("Name", item.Name);
    }

    private static void DrawDesciption(VirtualItem item)
    {
        item.Description = EditorGUILayout.TextField("Desription", item.Description);
    }

    private void DrawItemCategory(VirtualItem item)
    {
        EditorGUILayout.LabelField("Category", item.Category == null ? "None" : item.Category.ID);
    }

    private void DrawIcon(VirtualItem item)
    {
        item.Icon = EditorGUILayout.ObjectField("Icon", item.Icon, typeof(Sprite), false) as Sprite;
    }

    private void DrawIsEquippable(LifeTimeItem item)
    {
        item.IsEquippable = EditorGUILayout.Toggle("Is Equippable", item.IsEquippable);
    }

    private void DrawVirtualCategory(VirtualCategory category)
    {

    }

    private Vector2 _scrollPosition;
    private bool _isVirtualItemPropertiesExpanded = true;
    private bool _isPackInfoExpanded = true;
    private bool _isPurchaseInfoExpanded = true;
    private bool _isUpgradeInfoExpanded = false;

    private ScriptableObject _currentDisplayedItem;

    private PurchaseInfoListView _purchaseListView;
    private PackInfoListView _packListView;
    private UpgradesListView _upgradesListView;
}