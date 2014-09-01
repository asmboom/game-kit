using UnityEngine;
using UnityEditor;

public static class VirtualItemsDrawUtil
{
    static VirtualItemsDrawUtil()
    {
        TitleStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
        TitleStyle.alignment = TextAnchor.MiddleCenter;
        TitleStyle.fontStyle = FontStyle.Bold;
    }

    public static GUIStyle TitleStyle { get; private set; }

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
}
