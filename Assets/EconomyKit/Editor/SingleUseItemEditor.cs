using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SingleUseItem))]
public class SingleUseItemEditor : VirtualCurrencyEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}