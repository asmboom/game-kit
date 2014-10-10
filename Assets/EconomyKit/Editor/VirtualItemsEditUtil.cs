using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Beetle23
{
    public static class VirtualItemsEditUtil
    {
        public static readonly string DefaultVirtualItemDataPath = "Assets/Resources";

        public static string[] DisplayedCategories { get; private set; }
        public static string[] DisplayedVirtualCurrencyIDs { get; private set; }
        public static string[] DisplayedItemIDs { get; private set; }

        public static T CreateAsset<T>(string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return asset;
        }

        public static T CreateNewVirtualItem<T>(string path = null) where T : VirtualItem
        {
            T asset = ScriptableObject.CreateInstance<T>();
            path = string.IsNullOrEmpty(path) ? DefaultVirtualItemDataPath : path;
            AssetDatabase.CreateAsset(asset, path + "/VirtualItems/NewVirtualItem" + EconomyKit.Config.ItemsCount + ".asset");
            AssetDatabase.SaveAssets();
            return asset;
        }

        public static void UpdateDisplayedOptions()
        {
            UpdateDisplayedVirtualCurrencyIDs();
            UpdateDisplayedItemIDs();
        }

        public static void UpdatePurchaseByIndex(Purchase purchase, int newCurrencyIndex)
        {
            purchase.VirtualCurrency =
                EconomyKit.Config.GetItemByID(DisplayedVirtualCurrencyIDs[newCurrencyIndex]) as VirtualCurrency;
        }

        public static void UpdateRelatedItemByIndex(VirtualItem item, int newItemIndex)
        {
            if (item is UpgradeItem)
            {
                UpgradeItem upgradeItem = item as UpgradeItem;
                upgradeItem.RelatedItem = EconomyKit.Config.GetItemByID(DisplayedItemIDs[newItemIndex]);
            }
        }

        public static void UpdatePackElementItemByIndex(PackElement element, int newItemIndex)
        {
            if (element != null)
            {
                element.Item = EconomyKit.Config.GetItemByID(DisplayedItemIDs[newItemIndex]);
            }
        }

        public static int GetCategoryIndexById(string categoryId)
        {
            for (int i = 0; i < DisplayedCategories.Length; i++)
            {
                if (DisplayedCategories[i].Equals(categoryId))
                {
                    return i;
                }
            }
            Debug.LogError("Failed to find categogy id: [" + categoryId + "]");
            return 0;
        }

        public static int GetVirtualCurrencyIndexById(string virtualCurrencyId)
        {
            for (int i = 0; i < DisplayedVirtualCurrencyIDs.Length; i++)
            {
                if (DisplayedVirtualCurrencyIDs[i].Equals(virtualCurrencyId))
                {
                    return i;
                }
            }
            return 0;
        }

        public static int GetItemIndexById(string itemId)
        {
            for (int i = 0; i < DisplayedItemIDs.Length; i++)
            {
                if (DisplayedItemIDs[i].Equals(itemId))
                {
                    return i;
                }
            }
            return 0;
        }

        private static void UpdateDisplayedVirtualCurrencyIDs()
        {
            List<string> ids = new List<string>();
            foreach (var item in EconomyKit.Config.VirtualCurrencies)
            {
                ids.Add(GetIDString(item));
            }
            DisplayedVirtualCurrencyIDs = ids.ToArray();
        }

        private static void UpdateDisplayedItemIDs()
        {
            List<string> ids = new List<string>();
            foreach (var item in EconomyKit.Config.VirtualCurrencies)
            {
                ids.Add(GetIDString(item));
            }
            foreach (var item in EconomyKit.Config.SingleUseItems)
            {
                ids.Add(GetIDString(item));
            }
            foreach (var item in EconomyKit.Config.LifeTimeItems)
            {
                ids.Add(GetIDString(item));
            }
            DisplayedItemIDs = ids.ToArray();
        }

        private static string GetIDString(VirtualItem item)
        {
            return item.ID;
        }
    }
}
