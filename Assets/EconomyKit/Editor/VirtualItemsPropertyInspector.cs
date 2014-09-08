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
            _currentDisplayedItemID = (item as VirtualItem).ID;
            GUI.FocusControl(string.Empty);

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
        VirtualItem item = _currentDisplayedItem as VirtualItem;
        if (item != null)
        {
            GUI.BeginGroup(position, string.Empty, "Box");
            DrawVirtualItem(new Rect(10, 0, position.width - 10, position.height), item);
            GUI.EndGroup();
        }
        else
        {
            VirtualCategory category = _currentDisplayedItem as VirtualCategory;
            if (category != null)
            {
                GUILayout.BeginArea(position, string.Empty, "Box");
                _categoryPropertyView.Draw(new Rect(10, 0, position.width, position.height), category);
                GUILayout.EndArea();
            }
        }
    }

    private void DrawVirtualItem(Rect position, VirtualItem item)
    {
        EditorGUI.BeginChangeCheck();

        GUI.BeginGroup(position);
        _scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height),
            _scrollPosition, new Rect(0, 0, position.width - 20, _currentYOffset));

        float yOffset = 0;
        bool showScrollbar = position.height < _currentYOffset;
        float width = position.width - (showScrollbar ? 20 : 10);
        _isVirtualItemPropertiesExpanded = EditorGUI.Foldout(new Rect(0, 0, width, 20), 
            _isVirtualItemPropertiesExpanded, "Basic Property");
        yOffset += 20;
        if (_isVirtualItemPropertiesExpanded)
        {
            DrawID(new Rect(0, yOffset, width, 20), item);
            yOffset += 20;
            //EditorGUI.LabelField(new Rect(0, yOffset, width, 20), "Sort index", item.SortIndex.ToString());
            //yOffset += 20;
            item.Name = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Name", item.Name);
            yOffset += 20;
            item.Description = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Desription", item.Description);
            yOffset += 20;
            EditorGUI.LabelField(new Rect(0, yOffset, width, 20), "Category", item.Category == null ? "None" : item.Category.ID);
            yOffset += 20;
            item.Icon = EditorGUI.ObjectField(new Rect(0, yOffset, width, 20), "Icon", item.Icon, typeof(Sprite), false) as Sprite;
            yOffset += 20;
        }

        if (item is LifeTimeItem)
        {
            yOffset += 20;
            LifeTimeItem lifetimeItem = item as LifeTimeItem;
            lifetimeItem.IsEquippable = EditorGUI.Toggle(new Rect(0, yOffset, width, 20), "Is Equippable", lifetimeItem.IsEquippable);
            yOffset += 20;
        }
        else if (item is VirtualItemPack)
        {
            yOffset += 20;
            _isPackInfoExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20), _isPackInfoExpanded, "Pack Info");
            yOffset += 20;
            if (_isPackInfoExpanded)
            {
                _packListView.Draw(new Rect(0, yOffset, width, 100));
                yOffset += 100;
            }
        }
        if (item is PurchasableItem)
        {
            yOffset += 20;
            _isPurchaseInfoExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20), _isPurchaseInfoExpanded, "Purchase Info");
            yOffset += 20;
            if (_isPurchaseInfoExpanded)
            {
                _purchaseListView.Draw(new Rect(0, yOffset, width, 100));
                yOffset += 100;
            }
        }
        if (item is SingleUseItem || item is LifeTimeItem)
        {
            yOffset += 20;
            _isUpgradeInfoExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20), 
                _isUpgradeInfoExpanded, "Upgrade Info (" + item.Upgrades.Count + " levels)");
            yOffset += 20;
            if (_isUpgradeInfoExpanded)
            {
                _upgradesListView.Draw(new Rect(0, yOffset, width, 200));
                yOffset += 200;
            }
        }
        _currentYOffset = yOffset;

        GUI.EndScrollView();
        GUI.EndGroup();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(item);
        }
    }

    private void DrawID(Rect position, VirtualItem item)
    {
        GUI.SetNextControlName(IDInputControlName);
        if (EditorGUI.TextField(position, "Unique ID", 
            _currentDisplayedItemID).KeyPressed<string>(IDInputControlName, 
                KeyCode.Return, out _currentDisplayedItemID) ||
            (GUI.GetNameOfFocusedControl() != IDInputControlName && 
             _currentDisplayedItemID != item.ID))
        {
            EconomyKit.Config.UpdateIdToItemMap();
            VirtualItem itemWithID = EconomyKit.Config.GetItemByID(_currentDisplayedItemID);
            if (itemWithID != null && itemWithID != item)
            {
                GUIUtility.keyboardControl = 0;
                EditorUtility.DisplayDialog("Duplicate ID", "An item with ID[" + 
                    _currentDisplayedItemID + "] already exists!!!", "OK");
                _currentDisplayedItemID = item.ID;
            }
            else
            {
                item.ID = _currentDisplayedItemID;
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), item.ID);
                VirtualItemsEditorWindow.GetInstance().Repaint();
                VirtualItemsEditUtil.UpdateDisplayedOptions();
            }
        }
    }

    private bool _isVirtualItemPropertiesExpanded = true;
    private bool _isPackInfoExpanded = true;
    private bool _isPurchaseInfoExpanded = true;
    private bool _isUpgradeInfoExpanded = false;

    private string _currentDisplayedItemID;
    private object _currentDisplayedItem;

    private PurchaseInfoListView _purchaseListView;
    private PackInfoListView _packListView;
    private UpgradesListView _upgradesListView;
    private CategoryPropertyView _categoryPropertyView;

    private Vector2 _scrollPosition;
    private float _currentYOffset;

    private const string IDInputControlName = "virtual_item_id_field";
}
