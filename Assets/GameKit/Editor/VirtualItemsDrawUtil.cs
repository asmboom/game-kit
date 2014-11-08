using UnityEngine;
using UnityEditor;

public static class VirtualItemsDrawUtil
{
    static VirtualItemsDrawUtil()
    {
        TitleStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
        TitleStyle.alignment = TextAnchor.MiddleCenter;
        TitleStyle.fontStyle = FontStyle.Bold;

        ItemStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
        ItemStyle.alignment = TextAnchor.MiddleCenter;

        ItemSelectedStyle = new GUIStyle(GUI.skin.GetStyle("Box"));
        ItemSelectedStyle.normal.textColor = Color.cyan;

        FoldoutStyle = new GUIStyle(EditorStyles.foldout);
        FoldoutStyle.fixedHeight = FoldoutStyle.fixedWidth = 0;
    }

    public static GUIStyle TitleStyle { get; private set; }
    public static GUIStyle ItemStyle { get; private set; }
    public static GUIStyle ItemSelectedStyle { get; private set; }
    public static GUIStyle FoldoutStyle { get; private set; }

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
