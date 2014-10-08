using UnityEngine;
using UnityEditor;

namespace Beetle23
{
    [CustomEditor(typeof(VirtualItemPack))]
    public class VirtualItemPackEditor : VirtualCurrencyEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.LabelField("Pack info", (target as VirtualItemPack).ToString());
            VirtualCurrencyEditor.DrawPurchaseInspector(target as PurchasableItem);
        }
    }
}