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
        xOffset += CategoryWidth;

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
            int newIndex = DrawCategory(
                new Rect(xOffset, y, CategoryWidth - 1, height), false, categoryIndices[index]);
            if (newIndex != categoryIndices[index])
            {
                VirtualItemsEditUtil.UpdateItemCategoryByIndex(item, newIndex);
            }
            categoryIndices[index] = newIndex;
        }
        xOffset += CategoryWidth;

        return xOffset;
    }

    public static float DrawPurchase(float x, float y, float height, bool drawTitle, PurchasableItem item)
    {
        float xOffset = x;
        Rect position = new Rect(xOffset, y, PurchaseInfoWidth - 1, height);

        if (drawTitle)
        {
            GUI.Label(new Rect(position.x + 30, position.y, position.width, position.height), "Purchase Info");
        }
        else
        {
            _oldAlignment = _labelStyle.alignment;
            _labelStyle.alignment = TextAnchor.UpperCenter;
            GUI.Label(position, GetPurchaseInfoString(item));
            _labelStyle.alignment = _oldAlignment;

            if (GUI.Button(new Rect(position.x + position.width - 30, position.y, 30, position.height), "..."))
            {
                PurchaseInfoEditorWindow window = EditorWindow.GetWindow<PurchaseInfoEditorWindow>();
                window.Init(item);
            }
        }

        xOffset += PurchaseInfoWidth;
        return xOffset;
    }

    public static float DrawIsEquippable(float x, float y, float height, bool drawTitle, LifeTimeItem item)
    {
        float xOffset = x;
        Rect position = new Rect(xOffset, y, EquippableWidth - 1, height);
        if (drawTitle)
        {
            GUI.Label(new Rect(position.x + 20, position.y, position.width, position.height), "Equippable");
        }
        else
        {
            item.IsEquippable = EditorGUI.Toggle(new Rect(position.x + position.width / 2 - 10, position.y, 20, position.height)
                , item.IsEquippable);
        }
        xOffset += EquippableWidth;
        return xOffset;
    }

    public static float DrawPackElements(float x, float y, float height, bool drawTitle, VirtualItemPack pack)
    {
        float xOffset = x;
        Rect position = new Rect(xOffset, y, PackDetailWidth - 1, height);

        if (drawTitle)
        {
            GUI.Label(new Rect(position.x + 20, position.y, position.width, position.height), "Pack Detail");
        }
        else
        {
            _oldAlignment = _labelStyle.alignment;
            _labelStyle.alignment = TextAnchor.UpperCenter;
            GUI.Label(position, pack.ToString());
            _labelStyle.alignment = _oldAlignment;

            if (GUI.Button(new Rect(position.x + position.width - 30, position.y, 30, position.height), "..."))
            {
                PackElementsEditorWindow window = EditorWindow.GetWindow<PackElementsEditorWindow>();
                window.Init(pack);
            }
        }

        xOffset += PackDetailWidth;
        return xOffset;
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

    private static void DrawName(Rect position, bool drawTitle, VirtualItem item)
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

    private static void DrawDescription(Rect position, bool drawTitle, VirtualItem item)
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

    private static int DrawCategory(Rect position, bool drawTitle, int categoryIndex)
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

    private static string GetPurchaseInfoString(PurchasableItem item)
    {
        string final = string.Empty;
        for (int i = 0; i < item.PurchaseInfo.Count; i++)
        {
            Purchase purchase = item.PurchaseInfo[i];
            if (purchase != null)
            {
                final += GetOnePurchaseString(purchase);
                if (i < item.PurchaseInfo.Count - 1)
                {
                    final += "\nor ";
                }
            }
        }
        return final;
    }

    private static string GetOnePurchaseString(Purchase purchase)
    {
        return purchase.Type == PurchaseType.PurchaseWithVirtualCurrency ?
            string.Format("{0}x{1}", purchase.VirtualCurrency != null ? purchase.VirtualCurrency.ID : "Null", purchase.Price) : 
            string.Format("Market({0}):{1}", purchase.MarketID, purchase.Price);
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
    private const float CategoryWidth = 150;
    private const float PurchaseInfoWidth = 200;
    private const float EquippableWidth = 80;
    private const float PackDetailWidth = 200;
}
