using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Codeplay
{
    [CustomEditor(typeof(GameKitConfig))]
    public class GameKitConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GameKitConfig config = target as GameKitConfig;
            if (config != null)
            {
                DrawDefaultInspector();
                GUILayout.Label("Please Edit in GameKit Window.");

                if (GUILayout.Button("Check References"))
                {
                    config.UpdateMapsAndTree();
                    CheckIfAnyInvalidRef(config);
                }
            }
        }

        public static void CheckIfAnyInvalidRef(GameKitConfig config)
        {
            foreach (var item in config.LifeTimeItems)
            {
                for (int i = 0; i < item.PurchaseInfo.Count; i++)
                {
                    CheckPurchase("Life-time item", item.ID, item.PurchaseInfo[i], i);
                }
                for (int i = 0; i < item.Upgrades.Count; i++)
                {
                    UpgradeItem upgrade = item.Upgrades[i];
                    for (int j = 0; j < upgrade.PurchaseInfo.Count; j++)
                    {
                        CheckPurchase(item.ID + " upgrade", upgrade.ID, upgrade.PurchaseInfo[j], j);
                    }
                }
            }
            foreach (var item in config.SingleUseItems)
            {
                for (int i = 0; i < item.PurchaseInfo.Count; i++)
                {
                    CheckPurchase("Single use item", item.ID, item.PurchaseInfo[i], i);
                }
                for (int i = 0; i < item.Upgrades.Count; i++)
                {
                    UpgradeItem upgrade = item.Upgrades[i];
                    for (int j = 0; j < upgrade.PurchaseInfo.Count; j++)
                    {
                        CheckPurchase(item.ID + " upgrade", upgrade.ID, upgrade.PurchaseInfo[j], j);
                    }
                }
            }
            foreach (var pack in config.ItemPacks)
            {
                for (int i = 0; i < pack.PackElements.Count; i++)
                {
                    PackElement element = pack.PackElements[i];

                    if (element.Item == null)
                    {
                        Debug.LogError("Pack [" + pack.ID + "]'s [" + (i + 1) + "] element item is null.");
                    }
                }
                for (int i = 0; i < pack.PurchaseInfo.Count; i++)
                {
                    CheckPurchase("Pack", pack.ID, pack.PurchaseInfo[i], i);
                }
            }
            foreach (var category in config.Categories)
            {
                List<VirtualItem> items = category.GetItems(true);
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] == null)
                    {
                        Debug.LogError("Category [" + category.ID + "]'s [" + (i + 1) + "] item is null.");
                    }
                }
            }
        }

        private static void CheckPurchase(string type, string itemID, Purchase purchase, int purchaseIndex)
        {
            if (purchase.IsMarketPurchase)
            {
                if (string.IsNullOrEmpty(purchase.MarketID))
                {
                    Debug.LogError(type + " [" + itemID +
                        "]'s [" + (purchaseIndex + 1) + "] purchase's market id is empty.");
                }
            }
            else
            {
                if (purchase.VirtualCurrency == null)
                {
                    Debug.LogError(type + " [" + itemID +
                        "]'s [" + (purchaseIndex + 1) + "] purchase's related virtual currency is null.");
                }
            }
        }
    }
}
