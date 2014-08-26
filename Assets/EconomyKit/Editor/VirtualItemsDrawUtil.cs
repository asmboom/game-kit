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

    public static float DrawVirtualItemTitle(float x, float y, float height)
    {
        float xOffset = x;
        DrawID(new Rect(xOffset, y, IdWidth - 1, height), true, null);
        xOffset += IdWidth;

        DrawName(new Rect(xOffset, y, NameWidth - 1, height), true, null);
        xOffset += NameWidth;

        DrawDescription(new Rect(xOffset, y, DescriptionWidth - 1, height), true, null);
        xOffset += DescriptionWidth;

        DrawCategory(
            new Rect(xOffset, y, CategoryWidth - 1, height), true, 0);

        return xOffset;
    }

    public static float DrawVirtualItemInfo(float x, float y, float height, VirtualItem item, int index, 
        System.Collections.Generic.List<int> categoryIndices)
    {
        float xOffset = x;
        DrawID(new Rect(xOffset, y, IdWidth - 1, height), false, item);
        xOffset += IdWidth;

        DrawName(new Rect(xOffset, y, NameWidth - 1, height), false, item);
        xOffset += NameWidth;

        DrawDescription(new Rect(xOffset, y, DescriptionWidth - 1, height), false, item);
        xOffset += DescriptionWidth;

        if (index < categoryIndices.Count)
        {
            GUI.changed = false;
            categoryIndices[index] = DrawCategory(
                new Rect(xOffset, y, CategoryWidth - 1, height), false, categoryIndices[index]);
            if (GUI.changed)
            {
                VirtualItemsEditUtil.UpdateItemCategoryByIndex(item, categoryIndices[index]);
            }
        }

        return xOffset;
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

    public static void BeginDrawTitle()
    {
        _oldAlignment = _labelStyle.alignment;
        _oldFontSize = _labelStyle.fontSize;
        _labelStyle.alignment = TextAnchor.UpperCenter;
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
    private const float CategoryWidth = 150;
}
