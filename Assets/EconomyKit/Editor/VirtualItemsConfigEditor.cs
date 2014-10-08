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

                if (GUILayout.Button("Check Errors"))
                {
                    CheckIfErrors(config);
                }
            }
        }

        private static void CheckIfErrors(VirtualItemsConfig virtualItemsConfig)
        {
            virtualItemsConfig.UpdateMaps();
            CheckIfAnyInvalidRef(virtualItemsConfig);
        }

        private static void CheckIfAnyInvalidRef(VirtualItemsConfig config)
        {
            foreach (var pack in config.ItemPacks)
            {
                foreach (var element in pack.PackElements)
                {
                    if (element.ItemID == null)
                    {
                        Debug.LogError("Pack [" + pack.ID + "]'s element item is null.");
                    }
                }
            }
        }
    }
}
