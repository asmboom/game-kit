using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;

public class VirtualCurrencyListView : IView
{
    public VirtualCurrencyListView(List<VirtualCurrency> list)
    {
        _list = list;
        _listAdaptor = new GenericClassListAdaptor<VirtualCurrency>(list, 22, 
            CreateVirtualCurrency, DrawItem);
        _listControl = new ReorderableListControl();

        UpdateCategoryIndices();
    }

    public void Show()
    {
        _listControl.ItemInserted += OnItemInsert;
        _listControl.ItemRemoving += OnItemRemoving;
    }

    public void Hide()
    {
        _listControl.ItemInserted -= OnItemInsert;
        _listControl.ItemRemoving -= OnItemRemoving;
    }

    public void Draw(Rect position) 
    {
        if (_list == null || _listAdaptor == null) return;

        float yOffset = 30;
        float width = 800;
        float listHeight = _listControl.CalculateListHeight(_listAdaptor);

        _scrollPosition = GUI.BeginScrollView(new Rect(0, yOffset, position.width, position.height - yOffset), 
            _scrollPosition, new Rect(0, yOffset, width, listHeight + 20));

        DrawTitle(new Rect(0, yOffset, width, 20));
        _listControl.Draw(new Rect(0, yOffset + 20, width, listHeight), _listAdaptor);

        GUI.EndScrollView();
    }

    private void OnItemRemoving(object sender, ItemRemovingEventArgs args)
    {
        if (EditorUtility.DisplayDialog("Confirm to delete",
                "Confirm to delete virtual item [" + _listAdaptor[args.itemIndex].Name + "]?", "OK", "Cancel"))
        {
            args.Cancel = false;
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_listAdaptor[args.itemIndex]));
        }
        else
        {
            args.Cancel = true;
        }
    }

    private void OnItemInsert(object sender, ItemInsertedEventArgs args)
    {
        UpdateCategoryIndices();
    }

    public VirtualCurrency CreateVirtualCurrency()
    {
        return VirtualItemsEditUtil.CreateNewVirtualItem<VirtualCurrency>();
    }

    private void DrawTitle(Rect position)
    {
        var centeredStyle = GUI.skin.GetStyle("Label");
        var oldAlignment = centeredStyle.alignment;
        var oldFontSize = centeredStyle.fontSize;
        centeredStyle.alignment = TextAnchor.UpperCenter;
        centeredStyle.fontSize = 12;
        centeredStyle.fontStyle = FontStyle.Bold;

        float xOffset = position.x;
        VirtualItemsDrawUtil.DrawID(new Rect(xOffset, position.y, position.width * IdWidth - 1, position.height), true, null);
        xOffset += position.width * IdWidth;

        VirtualItemsDrawUtil.DrawName(new Rect(xOffset, position.y, position.width * NameWidth - 1, position.height), true, null);
        xOffset += position.width * NameWidth;

        VirtualItemsDrawUtil.DrawDescription(new Rect(xOffset, position.y, position.width * DescriptionWidth - 1, position.height), true, null);
        xOffset += position.width * DescriptionWidth;

        VirtualItemsDrawUtil.DrawCategory(
            new Rect(xOffset, position.y, position.width * CategoryWidth - 1, position.height), true, 0);

        centeredStyle.alignment = oldAlignment;
        centeredStyle.fontSize = oldFontSize;
        centeredStyle.fontStyle = FontStyle.Normal;
    }

    public VirtualCurrency DrawItem(Rect position, VirtualCurrency virtualCurrency, int index)
    {
        float xOffset = position.x;
        VirtualItemsDrawUtil.DrawID(new Rect(xOffset, position.y, position.width * IdWidth - 1, position.height), false, virtualCurrency);
        xOffset += position.width * IdWidth;

        VirtualItemsDrawUtil.DrawName(new Rect(xOffset, position.y, position.width * NameWidth - 1, position.height), false, virtualCurrency);
        xOffset += position.width * NameWidth;

        VirtualItemsDrawUtil.DrawDescription(new Rect(xOffset, position.y, position.width * DescriptionWidth - 1, position.height), false, virtualCurrency);
        xOffset += position.width * DescriptionWidth;

        if (index < _categoryIndices.Count)
        {
            GUI.changed = false;
            _categoryIndices[index] = VirtualItemsDrawUtil.DrawCategory(
                new Rect(xOffset, position.y, position.width * CategoryWidth - 1, position.height), false, _categoryIndices[index]);
            if (GUI.changed)
            {
                VirtualItemsEditUtil.UpdateItemCategoryByIndex(virtualCurrency, _categoryIndices[index]);
            }
        }

        return virtualCurrency;
    }

    public void UpdateCategoryIndices()
    {
        if (_listAdaptor != null)
        {
            _categoryIndices = new List<int>();

            for (var i = 0; i < _listAdaptor.Count; i++)
            {
                var item = _listAdaptor[i];
                _categoryIndices.Add(item.Category == null ? 0 : VirtualItemsEditUtil.GetCategoryIndexById(item.Category.ID));
            }
        }
    }

    private List<VirtualCurrency> _list;
    private ReorderableListControl _listControl;
    private GenericClassListAdaptor<VirtualCurrency> _listAdaptor;
    private List<int> _categoryIndices;

    private const float IdWidth = 0.2f;
    private const float NameWidth = 0.2f;
    private const float DescriptionWidth = 0.4f;
    private const float CategoryWidth = 0.2f;
    private Vector2 _scrollPosition;
}