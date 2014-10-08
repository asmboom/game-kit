using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Beetle23
{
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
            virtualItemsConfig.ItemPacks.AddRange(Resources.FindObjectsOfTypeAll<VirtualItemPack>());
            virtualItemsConfig.ItemPacks.Sort();

            virtualItemsConfig.UpdateIdToItemMap();
            CheckIfAnyInvalidRef(virtualItemsConfig);
            EditorUtility.SetDirty(virtualItemsConfig);
        }

        private static void ClearVirtualItemConfigLists(VirtualItemsConfig virtualItemsConfig)
        {
            virtualItemsConfig.VirtualCurrencies.Clear();
            virtualItemsConfig.SingleUseItems.Clear();
            virtualItemsConfig.LifeTimeItems.Clear();
            virtualItemsConfig.ItemPacks.Clear();
        }

        private static void CheckIfAnyInvalidRef(VirtualItemsConfig config)
        {
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
}
