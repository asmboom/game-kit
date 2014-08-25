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
        _currentItemsInfo = new List<VirtualItemInfo>();
        _itemTypes = new string[] { "Virtual Currency", "Single Use", "LifeTime Item", "Upgrade Item", "Pack" };
        UpdateCategories();
    }

    private void UpdateItems(List<VirtualItem> items)
    {
        UpdateCategories();
        UpdateVirtualCurrencyIDs();
        UpdateItemIds();

        _currentItemsInfo.Clear();
        foreach (var item in items)
        {
            var info = new VirtualItemInfo()
            {
                Item = item,
                CategoryIndex = item.Category == null ? 0 : GetCategoryIndexById(item.Category.ID),
            };
            if (item is PurchasableItem)
            {
                PurchasableItem purchasable = item as PurchasableItem;
                info.VirtualCurrencyIndexOfPrimaryPurchase = GetVirtualCurrencyIndexById(purchasable.PrimaryPurchase.AssociatedID);
                info.VirtualCurrencyIndexOfSecondaryPurchase = GetVirtualCurrencyIndexById(purchasable.SecondaryPurchase.AssociatedID);
            }
            if (item is UpgradeItem)
            {
                UpgradeItem upgradeItem = item as UpgradeItem;
                info.RelatedItemIndex = GetItemIndexById(upgradeItem.RelatedItemID);
            }

            _currentItemsInfo.Add(info);
        }
    }

    private int GetCategoryIndexById(string categoryId)
    {
        for (int i = 0; i < _categories.Length; i++)
        {
            if (_categories[i].Equals(categoryId))
            {
                return i;
            }
        }
        Debug.LogError("Failed find categogy id: [" + categoryId + "]");
        return 0;
    }

    private int GetVirtualCurrencyIndexById(string virtualCurrencyId)
    {
        for (int i = 0; i < _virtualCurrencyIds.Length; i++)
        {
            if (_virtualCurrencyIds[i].Equals(virtualCurrencyId))
            {
                return i;
            }
        }
        return 0;
    }

    private int GetItemIndexById(string itemId)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i].Equals(itemId))
            {
                return i;
            }
        }
        return 0;
    }

    private void UpdateCategories()
    {
        List<string> categoryTitles = new List<string>();
        categoryTitles.Add("None");
        foreach (var category in _config.Categories)
        {
            categoryTitles.Add(category.ID);
        }
        _categories = categoryTitles.ToArray();
    }

    private void UpdateVirtualCurrencyIDs()
    {
        List<string> ids = new List<string>();
        foreach (var item in _config.VirtualCurrencies)
        {
            ids.Add(item.ID);
        }
        _virtualCurrencyIds = ids.ToArray();
    }

    private void UpdateItemIds()
    {
        List<string> ids = new List<string>();
        foreach (var item in _config.VirtualCurrencies)
        {
            ids.Add(item.ID);
        }
        foreach (var item in _config.SingleUseItems)
        {
            ids.Add(item.ID);
        }
        foreach (var item in _config.LifeTimeItems)
        {
            ids.Add(item.ID);
        }
        _items = ids.ToArray();
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
                    break;
                case VirtualItemType.SingleUse:
                    foreach (var item in _config.SingleUseItems)
                    {
                        items.Add(item);
                    }
                    break;
                case VirtualItemType.LifeTime:
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

        DrawCurrentItemsInfo();
        _selectedItemTypeIndex = newSelectedCategoryIdx;
    }

    private void DrawCurrentItemsInfo()
    {
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false);

        for (int i = -1; i < _currentItemsInfo.Count; i++)
        {
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(800));
            bool drawTitle = i < 0;
            VirtualItemInfo info = i < 0 ? null : _currentItemsInfo[i];

            DrawDeleteButton(drawTitle, info);
            DrawID(drawTitle, info);
            DrawName(drawTitle, info);
            DrawDescription(drawTitle, info);
            DrawCategory(drawTitle, info);

            if (_selectedItemType == VirtualItemType.LifeTime)
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

    private void DrawDeleteButton(bool drawTitle, VirtualItemInfo info)
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

    private void DrawID(bool drawTitle, VirtualItemInfo info)
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

    private void DrawName(bool drawTitle, VirtualItemInfo info)
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

    private void DrawDescription(bool drawTitle, VirtualItemInfo info)
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

    private void DrawCategory(bool drawTitle, VirtualItemInfo info)
    {
        if (drawTitle)
        {
            GUILayout.Label("Category", GUILayout.Width(100), GUILayout.Height(RowHeight));
        }
        else
        {
            GUI.changed = false;
            info.CategoryIndex = EditorGUILayout.Popup(info.CategoryIndex, _categories, GUILayout.Width(100), GUILayout.Height(RowHeight));
            if (GUI.changed == true)
            {
                info.UpdateItemCategory(_config, _categories);
                EditorUtility.SetDirty(info.Item);
            }
        }
    }

    private void DrawPurchaseInfo(bool drawTitle, VirtualItemInfo info)
    {
        PurchasableItem purchasable = drawTitle ? null : info.Item as PurchasableItem;
        Purchase primaryPurchase = purchasable != null ? purchasable.PrimaryPurchase : null;
        Purchase secondaryPurchase = purchasable != null ? purchasable.SecondaryPurchase : null;
        DrawOnePurchase(drawTitle, info, primaryPurchase, true);
        DrawOnePurchase(drawTitle, info, secondaryPurchase, false);
    }

    private void DrawOnePurchase(bool drawTitle, VirtualItemInfo info, Purchase purchase, bool isPrimaryPurchase)
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
                info.UpdatePurchase(_config, _virtualCurrencyIds);
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
                        EditorGUILayout.Popup(info.VirtualCurrencyIndexOfPrimaryPurchase, _virtualCurrencyIds,
                            GUILayout.Width(idWidth), GUILayout.Height(RowHeight));
                }
                else
                {
                    info.VirtualCurrencyIndexOfSecondaryPurchase =
                        EditorGUILayout.Popup(info.VirtualCurrencyIndexOfSecondaryPurchase, _virtualCurrencyIds,
                            GUILayout.Width(idWidth), GUILayout.Height(RowHeight));
                }
                if (GUI.changed == true)
                {
                    info.UpdatePurchase(_config, _virtualCurrencyIds);
                    EditorUtility.SetDirty(info.Item);
                }
            }
        }
    }

    private void DrawIsEquippable(bool drawTitle, VirtualItemInfo info)
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

    private void DrawRelatedItem(bool drawTitle, VirtualItemInfo info)
    {
        if (drawTitle)
        {
            GUILayout.Label("Related Item", GUILayout.Width(100));
        }
        else
        {
            GUI.changed = false;
            info.RelatedItemIndex = EditorGUILayout.Popup(info.RelatedItemIndex, _items, 
                GUILayout.Width(100), GUILayout.Height(RowHeight));
            if (GUI.changed == true)
            {
                info.UpdateRelatedItem(_config, _items);
                EditorUtility.SetDirty(info.Item);
            }
        }
    }

    private void DrawPackElements(bool drawTitle, VirtualItemInfo info)
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
                GUILayout.Label(pack.ToString(), GUILayout.Width(200));
            }
        }
    }

    private class VirtualItemInfo
    {
        public VirtualItem Item;
        public int CategoryIndex;
        public int VirtualCurrencyIndexOfPrimaryPurchase;
        public int VirtualCurrencyIndexOfSecondaryPurchase;
        public int RelatedItemIndex;
        public int[] PackItemIndices;

        public void UpdateItemCategory(VirtualItemsConfig config, string[] categories)
        {
            if (CategoryIndex == 0)
            {
                Item.Category = null;
            }
            else
            {
                Item.Category = config.GetCategoryByID(categories[CategoryIndex]);
            }
        }

        public void UpdatePurchase(VirtualItemsConfig config, string[] virtualCurrencies)
        {
            if (Item is PurchasableItem)
            {
                PurchasableItem purchasable = Item as PurchasableItem;
                purchasable.PrimaryPurchase.AssociatedID =
                    config.GetItemByID(virtualCurrencies[VirtualCurrencyIndexOfPrimaryPurchase]).ID;
                purchasable.SecondaryPurchase.AssociatedID =
                    config.GetItemByID(virtualCurrencies[VirtualCurrencyIndexOfSecondaryPurchase]).ID;
            }
        }

        public void UpdateRelatedItem(VirtualItemsConfig config, string[] items)
        {
            if (Item is UpgradeItem)
            {
                UpgradeItem upgradeItem = Item as UpgradeItem;
                upgradeItem.RelatedItemID = config.GetItemByID(items[RelatedItemIndex]).ID;
            }
        }

        public void UpdatePackItems(VirtualItemsConfig config, string[] items)
        {
            if (Item is VirtualItemPack)
            {
                VirtualItemPack pack = Item as VirtualItemPack;
                for (int i = 0; i < PackItemIndices.Length; i++)
                {
                    if (pack.PackElements.Count >= i + 1)
                    {
                        pack.PackElements[i].ItemID = config.GetItemByID(items[PackItemIndices[i]]).ID;
                    }
                }
            }
        }
    }

    private VirtualItemsConfig _config;
    private int _selectedItemTypeIndex;
    private VirtualItemType _selectedItemType;
    private string[] _itemTypes;
    private string[] _categories;
    private string[] _virtualCurrencyIds;
    private string[] _items;
    private List<VirtualItemInfo> _currentItemsInfo;
    private Vector2 _scrollPosition;

    private const float RowHeight = 20;
}
