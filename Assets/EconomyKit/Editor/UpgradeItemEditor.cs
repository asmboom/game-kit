using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpgradeItem))]
public class UpgradeItemEditor : VirtualCurrencyEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        VirtualCurrencyEditor.DrawPurchaseInspector(target as PurchasableItem);
    }
}