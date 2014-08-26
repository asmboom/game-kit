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

    public static void DrawID(Rect position, bool drawTitle, VirtualItem item)
    {
        if (drawTitle)
        {
            GUI.Label(position, "ID");
        }
        else
        {
            string controlName = item.GetInstanceID() +"_input_field";
            GUI.SetNextControlName(controlName);
            if (EditorGUI.TextField(position, item.ID).KeyPressed<string>(controlName, KeyCode.Return, out item.ID))
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), item.ID);
            }
        }
    }

    public static void DrawName(Rect position, bool drawTitle, VirtualItem item)
    {
        if (drawTitle)
        {
            GUI.Label(position, "Name");
        }
        else
        {
            item.Name = EditorGUI.TextField(position, item.Name);
        }
    }

    public static void DrawDescription(Rect position, bool drawTitle, VirtualItem item)
    {
        if (drawTitle)
        {
            GUI.Label(position, "Description");
        }
        else
        {
            item.Description = EditorGUI.TextField(position, item.Description);
        }
    }

    public static int DrawCategory(Rect position, bool drawTitle, int categoryIndex)
    {
        if (drawTitle)
        {
            GUI.Label(position, "Category");
            return -1;
        }
        else
        {
            return EditorGUI.Popup(position, categoryIndex, VirtualItemsEditUtil.DisplayedCategories);
        }
    }
}
