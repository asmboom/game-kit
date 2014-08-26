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
        _currentItemsInfo = new List<VirtualItemDisplayInfo>();
        _itemTypes = new string[] { "Virtual Currency", "Single Use", "LifeTime Item", "Upgrade Item", "Pack" };

        VirtualItemsEditUtil.UpdateDisplayedOptions();
    }

    private void UpdateItems(List<VirtualItem> items)
    {
        VirtualItemsEditUtil.UpdateDisplayedOptions();

        _currentItemsInfo.Clear();
        foreach (var item in items)
        {
            var info = new VirtualItemDisplayInfo()
            {
                Item = item,
                CategoryIndex = item.Category == null ? 0 : VirtualItemsEditUtil.GetCategoryIndexById(item.Category.ID),
            };
            if (item is PurchasableItem)
            {
                PurchasableItem purchasable = item as PurchasableItem;
                info.VirtualCurrencyIndexOfPrimaryPurchase = VirtualItemsEditUtil.GetVirtualCurrencyIndexById(purchasable.PrimaryPurchase.AssociatedID);
                info.VirtualCurrencyIndexOfSecondaryPurchase = VirtualItemsEditUtil.GetVirtualCurrencyIndexById(purchasable.SecondaryPurchase.AssociatedID);
            }
            if (item is UpgradeItem)
            {
                UpgradeItem upgradeItem = item as UpgradeItem;
                info.RelatedItemIndex = VirtualItemsEditUtil.GetItemIndexById(upgradeItem.RelatedItemID);
            }

            _currentItemsInfo.Add(info);
        }
    }

    private void OnGUI()
    {
        int newSelectedCategoryIdx = Mathf.Max(0, GUILayout.SelectionGrid(_selectedItemTypeIndex, _itemTypes, _itemTypes.Length));
        _selectedItemType = (VirtualItemType)newSelectedCategoryIdx;
        if (newSelectedCategoryIdx != _selectedItemTypeIndex)
        {
            var items = new List<VirtualItem>();
            switch (_selectedItemType)
            {
                case VirtualItemType.VirtualCurrency:
                    foreach (var item in _config.VirtualCurrencies)
                    {
                        items.Add(item);
                    }
                    SetCurrentListView(new VirtualCurrencyListView(_config.VirtualCurrencies));
                    break;
                case VirtualItemType.SingleUseItem:
                    foreach (var item in _config.SingleUseItems)
                    {
                        items.Add(item);
                    }
                    SetCurrentListView(new SingleUseItemListView(_config.SingleUseItems));
                    break;
                case VirtualItemType.LifeTimeItem:
                    foreach (var item in _config.LifeTimeItems)
                    {
                        items.Add(item);
                    }
                    break;
                case VirtualItemType.UpgradeItem:
                    foreach (var item in _config.UpgradeItems)
                    {
                        items.Add(item);
                    }
                    break;
                case VirtualItemType.Pack:
                    foreach (var item in _config.ItemPacks)
                    {
                        items.Add(item);
                    }
                    break;
                default:
                    break;
            }
            UpdateItems(items);
            _scrollPosition = Vector2.zero;
        }

        //DrawCurrentItemsInfo();
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

    private void DrawCurrentItemsInfo()
    {
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false);

        for (int i = -1; i < _currentItemsInfo.Count; i++)
        {
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(800));
            bool drawTitle = i < 0;
            VirtualItemDisplayInfo info = i < 0 ? null : _currentItemsInfo[i];

            DrawDeleteButton(drawTitle, info);
            DrawID(drawTitle, info);
            DrawName(drawTitle, info);
            DrawDescription(drawTitle, info);
            DrawCategory(drawTitle, info);

            if (_selectedItemType == VirtualItemType.LifeTimeItem)
            {
                DrawIsEquippable(drawTitle, info);
            }
            else if (_selectedItemType == VirtualItemType.UpgradeItem)
            {
                DrawRelatedItem(drawTitle, info);
            }
            else if (_selectedItemType == VirtualItemType.Pack)
            {
                DrawPackElements(drawTitle, info);
            }
            if (_selectedItemType != VirtualItemType.VirtualCurrency)
            {
                DrawPurchaseInfo(drawTitle, info);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    private void DrawDeleteButton(bool drawTitle, VirtualItemDisplayInfo info)
    {
        if (drawTitle)
        {
            GUILayout.Label(string.Empty, GUILayout.Width(20), GUILayout.Height(RowHeight));
        }
        else
        {
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(RowHeight)))
            {
                if (EditorUtility.DisplayDialog("Confirm to delete",
                        "Confirm to delete virtual item [" + info.Item.Name + "]?", "OK", "Cancel"))
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(info.Item));
                }
            }
        }
    }

    private void DrawID(bool drawTitle, VirtualItemDisplayInfo info)
    {
        if (drawTitle)
        {
            GUILayout.Label("ID", GUILayout.Width(150), GUILayout.Height(RowHeight));
        }
        else
        {
            info.Item.ID = GUILayout.TextField(info.Item.ID, GUILayout.Width(150), GUILayout.Height(RowHeight));
        }
    }

    private void DrawName(bool drawTitle, VirtualItemDisplayInfo info)
    {
        if (drawTitle)
        {
            GUILayout.Label("Name", GUILayout.Width(150), GUILayout.Height(RowHeight));
        }
        else
        {
            info.Item.Name = GUILayout.TextField(info.Item.Name, GUILayout.Width(150), GUILayout.Height(RowHeight));
        }
    }

    private void DrawDescription(bool drawTitle, VirtualItemDisplayInfo info)
    {
        if (drawTitle)
        {
            GUILayout.Label("Description", GUILayout.Width(300), GUILayout.Height(RowHeight));
        }
        else
        {
            info.Item.Description = GUILayout.TextField(info.Item.Description, GUILayout.Width(300), GUILayout.Height(RowHeight));
        }
    }

    private void DrawCategory(bool drawTitle, VirtualItemDisplayInfo info)
    {
        if (drawTitle)
        {
            GUILayout.Label("Category", GUILayout.Width(100), GUILayout.Height(RowHeight));
        }
        else
        {
            GUI.changed = false;
            info.CategoryIndex = EditorGUILayout.Popup(info.CategoryIndex, VirtualItemsEditUtil.DisplayedCategories, 
                GUILayout.Width(100), GUILayout.Height(RowHeight));
            if (GUI.changed == true)
            {
                VirtualItemsEditUtil.UpdateItemCategoryByIndex(info.Item, info.CategoryIndex);
                EditorUtility.SetDirty(info.Item);
            }
        }
    }

    private void DrawPurchaseInfo(bool drawTitle, VirtualItemDisplayInfo info)
    {
        PurchasableItem purchasable = drawTitle ? null : info.Item as PurchasableItem;
        Purchase primaryPurchase = purchasable != null ? purchasable.PrimaryPurchase : null;
        Purchase secondaryPurchase = purchasable != null ? purchasable.SecondaryPurchase : null;
        DrawOnePurchase(drawTitle, info, primaryPurchase, true);
        DrawOnePurchase(drawTitle, info, secondaryPurchase, false);
    }

    private void DrawOnePurchase(bool drawTitle, VirtualItemDisplayInfo info, Purchase purchase, bool isPrimaryPurchase)
    {
        float typeWidth = 150, priceWidth = 150, idWidth = 200;
        if (drawTitle)
        {
            string prefix = isPrimaryPurchase ? "1st " : "2nd ";
            GUILayout.Label(prefix + "Purchase Type", GUILayout.Width(typeWidth), GUILayout.Height(RowHeight));
            GUILayout.Label(prefix + "Purchase Price", GUILayout.Width(priceWidth), GUILayout.Height(RowHeight));
            GUILayout.Label(prefix + "Purchase Associated ID", GUILayout.Width(idWidth), GUILayout.Height(RowHeight));
        }
        else
        {
            GUI.changed = false;
            purchase.Type = (PurchaseType)EditorGUILayout.EnumPopup(purchase.Type, GUILayout.Width(typeWidth), GUILayout.Height(RowHeight));
            if (GUI.changed)
            {
                VirtualItemsEditUtil.UpdatePurchaseByIndex(info.Item, 
                    info.VirtualCurrencyIndexOfPrimaryPurchase, info.VirtualCurrencyIndexOfSecondaryPurchase);
                EditorUtility.SetDirty(info.Item);
            }
            if (purchase.Type == PurchaseType.None)
            {
                GUILayout.Label(string.Empty, GUILayout.Width(priceWidth), GUILayout.Height(RowHeight));
                GUILayout.Label(string.Empty, GUILayout.Width(idWidth), GUILayout.Height(RowHeight));
            }
            else if (purchase.Type == PurchaseType.PurchaseWithMarket)
            {
                purchase.Price = EditorGUILayout.FloatField(purchase.Price, GUILayout.Width(priceWidth), GUILayout.Height(RowHeight));
                purchase.AssociatedID = EditorGUILayout.TextField(purchase.AssociatedID,
                    GUILayout.Width(idWidth), GUILayout.Height(RowHeight));
            }
            else
            {
                purchase.Price = EditorGUILayout.IntField((int)purchase.Price, GUILayout.Width(priceWidth), GUILayout.Height(RowHeight));
                GUI.changed = false;
                if (isPrimaryPurchase)
                {
                    info.VirtualCurrencyIndexOfPrimaryPurchase =
                        EditorGUILayout.Popup(info.VirtualCurrencyIndexOfPrimaryPurchase, VirtualItemsEditUtil.DisplayedVirtualCurrencyIDs,
                            GUILayout.Width(idWidth), GUILayout.Height(RowHeight));
                }
                else
                {
                    info.VirtualCurrencyIndexOfSecondaryPurchase =
                        EditorGUILayout.Popup(info.VirtualCurrencyIndexOfSecondaryPurchase, VirtualItemsEditUtil.DisplayedVirtualCurrencyIDs,
                            GUILayout.Width(idWidth), GUILayout.Height(RowHeight));
                }
                if (GUI.changed == true)
                {
                    VirtualItemsEditUtil.UpdatePurchaseByIndex(info.Item, 
                        info.VirtualCurrencyIndexOfPrimaryPurchase, info.VirtualCurrencyIndexOfSecondaryPurchase);
                    EditorUtility.SetDirty(info.Item);
                }
            }
        }
    }

    private void DrawIsEquippable(bool drawTitle, VirtualItemDisplayInfo info)
    {
        if (drawTitle)
        {
            GUILayout.Label("Equippable", GUILayout.Width(70));
        }
        else
        {
            LifeTimeItem item = info.Item as LifeTimeItem;
            if (item != null)
            {
                item.IsEquippable = EditorGUILayout.Toggle(item.IsEquippable, GUILayout.Width(70));
            }
        }
    }

    private void DrawRelatedItem(bool drawTitle, VirtualItemDisplayInfo info)
    {
        if (drawTitle)
        {
            GUILayout.Label("Related Item", GUILayout.Width(100));
        }
        else
        {
            GUI.changed = false;
            info.RelatedItemIndex = EditorGUILayout.Popup(info.RelatedItemIndex, VirtualItemsEditUtil.DisplayedItemIDs, 
                GUILayout.Width(100), GUILayout.Height(RowHeight));
            if (GUI.changed == true)
            {
                VirtualItemsEditUtil.UpdateRelatedItemByIndex(info.Item, info.RelatedItemIndex);
                EditorUtility.SetDirty(info.Item);
            }
        }
    }

    private void DrawPackElements(bool drawTitle, VirtualItemDisplayInfo info)
    {
        if (drawTitle)
        {
            GUILayout.Label("Pack Detail", GUILayout.Width(200));
        }
        else
        {
            VirtualItemPack pack = info.Item as VirtualItemPack;
            if (pack != null)
            {
                if (GUILayout.Button("c", GUILayout.Width(20)))
                {
                    PackElementsEditorWindow window = EditorWindow.GetWindow<PackElementsEditorWindow>();
                    window.Init(pack);
                }
                GUILayout.Label(pack.ToString(), GUILayout.Width(190));
            }
        }
    }

    private class VirtualItemDisplayInfo
    {
        public VirtualItem Item;
        public int CategoryIndex;
        public int VirtualCurrencyIndexOfPrimaryPurchase;
        public int VirtualCurrencyIndexOfSecondaryPurchase;
        public int RelatedItemIndex;
    }

    private VirtualItemsConfig _config;
    private int _selectedItemTypeIndex;
    private VirtualItemType _selectedItemType;
    private string[] _itemTypes;
    private List<VirtualItemDisplayInfo> _currentItemsInfo;
    private Vector2 _scrollPosition;

    private IView _currentListView;

    private const float RowHeight = 20;
}
