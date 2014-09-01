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
            {
                return true;
            }
            return false;
        }
        else
        {
            return false;
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
}
