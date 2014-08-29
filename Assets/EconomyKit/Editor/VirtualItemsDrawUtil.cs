using UnityEngine;
using UnityEditor;

public static class VirtualItemsDrawUtil
{
    public static bool KeyPressed<T>(this T s, string controlName, KeyCode key, out T fieldValue)
    {
        fieldValue = s;
        if (GUI.GetNameOfFocusedControl() == controlName)
        {
            if ((Event.current.type == EventType.KeyUp) && (Event.current.keyCode == key))
                return true;
            return false;
        }
        else
        {
            return false;
        }
    }

    private static void DrawID(Rect position, bool drawTitle, VirtualItem item)
    {
        if (drawTitle)
        {
            GUI.Label(position, "Unique ID");
        }
        else
        {
            string controlName = item.GetInstanceID() + "_input_field";
            GUI.SetNextControlName(controlName);
            if (EditorGUI.TextField(position, item.ID).KeyPressed<string>(controlName, KeyCode.Return, out item.ID))
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), item.ID);
            }
        }
    }

    public static void BeginDrawTitle()
    {
        _oldAlignment = _labelStyle.alignment;
        _oldFontSize = _labelStyle.fontSize;
        _labelStyle.alignment = TextAnchor.MiddleCenter;
        _labelStyle.fontSize = 12;
        _labelStyle.fontStyle = FontStyle.Bold;
    }

    public static void EndDrawTitle()
    {
        _labelStyle.alignment = _oldAlignment;
        _labelStyle.fontSize = _oldFontSize;
        _labelStyle.fontStyle = FontStyle.Normal;
    }

    private static GUIStyle _labelStyle = GUI.skin.GetStyle("Label");
    private static TextAnchor _oldAlignment;
    private static int _oldFontSize;

    private const float IdWidth = 150;
    private const float NameWidth = 150;
    private const float DescriptionWidth = 300;
    private const float PurchaseInfoWidth = 200;
    private const float EquippableWidth = 80;
    private const float PackDetailWidth = 200;
    private const float UpgradesWidth = 150;
}
