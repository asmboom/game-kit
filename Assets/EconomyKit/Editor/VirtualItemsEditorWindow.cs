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
        _selectedCategoryIdx = -1;
        _currentItemsInfo = new List<VirtualItemInfo>();
        _itemTypes = new string[] { "Virtual Currency", "Single Use", "LifeTime Item", "Upgrade Item", "Pack" };
        UpdateCategories();
    }

    private void UpdateItems(List<VirtualItem> items)
    {
        UpdateCategories();
        UpdateVirtualCurrencyIDs();

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
                info.VirtualCurrencyIndexOfPrimaryPurchase = purchasable.PurchaseInfo.Count >= 1 ? 
                    GetVirtualCurrencyIndexById(purchasable.PurchaseInfo[0].AssociatedID) : 0;
                info.VirtualCurrencyIndexOfSecondaryPurchase = purchasable.PurchaseInfo.Count >= 2 ? 
                    GetVirtualCurrencyIndexById(purchasable.PurchaseInfo[1].AssociatedID) : 0;
            }

            _currentItemsInfo.Add(info);
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

    private int GetVirtualCurrencyIndexById(string virtualCurrencyId)
    {
        for(int i = 0; i < _virtualCurrencyIds.Length; i++)
        {
            if (_virtualCurrencyIds[i].Equals(virtualCurrencyId))
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

    private void OnGUI()
    {
        int newSelectedCategoryIdx = Mathf.Max(0, GUILayout.SelectionGrid(_selectedCategoryIdx, _itemTypes, _itemTypes.Length));
        if (newSelectedCategoryIdx != _selectedCategoryIdx)
        {
            var items = new List<VirtualItem>();
            switch (newSelectedCategoryIdx)
            {
                case 0:
                    foreach(var item in _config.VirtualCurrencies)
                    {
                        items.Add(item);
                    }
                    break;
                case 1:
                    foreach(var item in _config.SingleUseItems)
                    {
                        items.Add(item);
                    }
                    break;
                default:
                    break;
            }
            UpdateItems(items);
        }
        switch (newSelectedCategoryIdx)
        {
            case 0:
                DrawVirtualCurrencies();
                break;
            case 1:
                DrawSingleUses();
                break;
            default:
                break;
        }
        _selectedCategoryIdx = newSelectedCategoryIdx;
    }

    private void DrawVirtualCurrencies()
    {
        foreach(var info in _currentItemsInfo)
        {
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(800));

            DrawDeleteButton(info);
            DrawID(info);
            DrawName(info);
            DrawDescription(info);
            DrawCategory(info);

            GUILayout.EndHorizontal();
        }
    }

    private void DrawSingleUses()
    {
        foreach(var info in _currentItemsInfo)
        {
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(800));

            DrawDeleteButton(info);
            DrawID(info);
            DrawName(info);
            DrawDescription(info);
            DrawCategory(info);
            DrawPurchaseInfo(info);

            GUILayout.EndHorizontal();
        }
    }

    private void DrawDeleteButton(VirtualItemInfo info)
    {
        if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20)))
        {
            //AssetDatabase.DeleteAsset
        }
    }

    private void DrawID(VirtualItemInfo info)
    {
        info.Item.ID = GUILayout.TextField(info.Item.ID, GUILayout.MinWidth(30), GUILayout.Height(20));
    }

    private void DrawName(VirtualItemInfo info)
    {
        info.Item.Name = GUILayout.TextField(info.Item.Name, GUILayout.MinWidth(30), GUILayout.Height(20));
    }

    private void DrawDescription(VirtualItemInfo info)
    {
        info.Item.Description = GUILayout.TextField(info.Item.Description, GUILayout.MinWidth(150), GUILayout.Height(20));
    }

    private void DrawCategory(VirtualItemInfo info)
    {
        GUI.changed = false;
        info.CategoryIndex = EditorGUILayout.Popup(info.CategoryIndex, _categories, GUILayout.MaxWidth(100), GUILayout.Height(20));
        if (GUI.changed == true)
        {
            info.UpdateItemCategory(_config, _categories);
            EditorUtility.SetDirty(info.Item);
        }
    }

    private void DrawPurchaseInfo(VirtualItemInfo info)
    {
        PurchasableItem purchasable = info.Item as PurchasableItem;
        if (purchasable != null)
        {
            if (purchasable.PurchaseInfo.Count >= 1)
            {
                DrawOnePurchase(info, purchasable.PurchaseInfo[0], true);

                if (purchasable.PurchaseInfo.Count >= 2)
                {
                    DrawOnePurchase(info, purchasable.PurchaseInfo[1], false);
                }
            }
        }
    }

    private void DrawOnePurchase(VirtualItemInfo info, Purchase purchase, bool isPrimaryPurchase)
    {
        purchase.Type = (PurchaseType)EditorGUILayout.EnumPopup(purchase.Type, GUILayout.MinWidth(100), GUILayout.Height(20));
        purchase.Price = EditorGUILayout.FloatField(purchase.Price, GUILayout.Height(20));
        if (purchase.Type == PurchaseType.PurchaseWithMarket)
        {
            purchase.AssociatedID = EditorGUILayout.TextField(purchase.AssociatedID, GUILayout.Height(20));
        }
        else
        {
            GUI.changed = false;
            if (isPrimaryPurchase)
            {
                info.VirtualCurrencyIndexOfPrimaryPurchase = 
                    EditorGUILayout.Popup(info.VirtualCurrencyIndexOfPrimaryPurchase, _virtualCurrencyIds, GUILayout.Height(20));
            }
            else
            {
                info.VirtualCurrencyIndexOfSecondaryPurchase = 
                    EditorGUILayout.Popup(info.VirtualCurrencyIndexOfSecondaryPurchase, _virtualCurrencyIds, GUILayout.Height(20));
            }
            if (GUI.changed == true)
            {
                info.UpdatePurchase(_config, _virtualCurrencyIds);
                EditorUtility.SetDirty(info.Item);
            }
        }
    }

    private class VirtualItemInfo
    {
        public VirtualItem Item;
        public int CategoryIndex;
        public int VirtualCurrencyIndexOfPrimaryPurchase;
        public int VirtualCurrencyIndexOfSecondaryPurchase;

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
                if (purchasable.PurchaseInfo.Count >= 1)
                {
                    purchasable.PurchaseInfo[0].AssociatedID = 
                        config.GetItemByID(virtualCurrencies[VirtualCurrencyIndexOfPrimaryPurchase]).ID;
                }
                if (purchasable.PurchaseInfo.Count >= 2)
                {
                    purchasable.PurchaseInfo[1].AssociatedID = 
                        config.GetItemByID(virtualCurrencies[VirtualCurrencyIndexOfSecondaryPurchase]).ID;
                }
            }
        }
    }

    private VirtualItemsConfig _config;
    private int _selectedCategoryIdx;
    private string[] _itemTypes;
    private string[] _categories;
    private string[] _virtualCurrencyIds;
    private List<VirtualItemInfo> _currentItemsInfo;
}
