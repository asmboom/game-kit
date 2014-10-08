using UnityEngine;
using UnityEditor;

namespace Beetle23
{
    [CustomEditor(typeof(UpgradeItem))]
    public class UpgradeItemEditor : VirtualCurrencyEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            VirtualCurrencyEditor.DrawPurchaseInspector(target as PurchasableItem);
        }
    }
}