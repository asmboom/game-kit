using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VirtualItemsConfig))]
public class VirtualItemsConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        VirtualItemsConfig config = target as VirtualItemsConfig;
        if (config != null)
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Update & Check Errors"))
            {
                UpdateVirtualItemsConfig(config);
            }
        }
    }

    private static void UpdateVirtualItemsConfig(VirtualItemsConfig virtualItemsConfig)
    {
        ClearVirtualItemConfigLists(virtualItemsConfig);

        // update virtual items list
        virtualItemsConfig.VirtualCurrencies.AddRange(Resources.FindObjectsOfTypeAll<VirtualCurrency>());
        virtualItemsConfig.VirtualCurrencies.Sort();
        virtualItemsConfig.SingleUseItems.AddRange(Resources.FindObjectsOfTypeAll<SingleUseItem>());
        virtualItemsConfig.SingleUseItems.Sort();
        virtualItemsConfig.LifeTimeItems.AddRange(Resources.FindObjectsOfTypeAll<LifeTimeItem>());
        virtualItemsConfig.LifeTimeItems.Sort();
        virtualItemsConfig.UpgradeItems.AddRange(Resources.FindObjectsOfTypeAll<UpgradeItem>());
        virtualItemsConfig.UpgradeItems.Sort();
        virtualItemsConfig.ItemPacks.AddRange(Resources.FindObjectsOfTypeAll<VirtualItemPack>());
        virtualItemsConfig.ItemPacks.Sort();

        virtualItemsConfig.UpdateIdToItemMap();

        // update categories
        virtualItemsConfig.Categories.Clear();
        virtualItemsConfig.Categories.AddRange(Resources.FindObjectsOfTypeAll<VirtualCategory>());

        foreach (var category in virtualItemsConfig.Categories)
        {
            category.Items.Clear();
        }
        foreach (var item in virtualItemsConfig.Items)
        {
            if (item.Category != null)
            {
                item.Category.Items.Add(item);
            }
        }
        foreach (var category in virtualItemsConfig.Categories)
        {
            EditorUtility.SetDirty(category);
        }

        CheckIfAnyInvalidRef(virtualItemsConfig);

        // update upgrades in virtual items
        foreach (var item in virtualItemsConfig.Items)
        {
            item.Upgrades.Clear();
        }
        foreach (var item in virtualItemsConfig.UpgradeItems)
        {
            VirtualItem relatedItem = item.RelatedItem;
            if (item.RelatedItem != null)
            {
                relatedItem.Upgrades.Add(item);
            }
            else
            {
                Debug.LogError("upgrade item [" + item.ID +
                    "]'s associated item is null");
            }
        }
        foreach (var item in virtualItemsConfig.Items)
        {
            EditorUtility.SetDirty(item);
        }

        EditorUtility.SetDirty(virtualItemsConfig);
    }

    private static void ClearVirtualItemConfigLists(VirtualItemsConfig virtualItemsConfig)
    {
        virtualItemsConfig.VirtualCurrencies.Clear();
        virtualItemsConfig.SingleUseItems.Clear();
        virtualItemsConfig.LifeTimeItems.Clear();
        virtualItemsConfig.UpgradeItems.Clear();
        virtualItemsConfig.ItemPacks.Clear();
        virtualItemsConfig.Categories.Clear();
    }

    private static void CheckIfAnyInvalidRef(VirtualItemsConfig config)
    {
        foreach(var item in config.UpgradeItems)
        {
            if (item.RelatedItem == null)
            {
                Debug.LogError("Upgrade item [" + item.ID + "]'s related item is null.");
            }
        }
        foreach (var pack in config.ItemPacks)
        {
            foreach (var element in pack.PackElements)
            {
                if (element.Item == null)
                {
                    Debug.LogError("Pack [" + pack.ID + "]'s element item is null.");
                }
            }
        }
    }
}
