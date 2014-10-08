using UnityEngine;
using UnityEditor;

namespace Beetle23
{
    [CustomEditor(typeof(SingleUseItem))]
    public class SingleUseItemEditor : VirtualCurrencyEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            VirtualCurrencyEditor.DrawPurchaseInspector(target as PurchasableItem);
        }
    }
}