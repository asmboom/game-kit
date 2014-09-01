using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LifeTimeItem))]
public class LifeTimeItemEditor : VirtualCurrencyEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField("Is Equippable", (target as LifeTimeItem).IsEquippable.ToString());
        VirtualCurrencyEditor.DrawPurchaseInspector(target as PurchasableItem);
    }
}