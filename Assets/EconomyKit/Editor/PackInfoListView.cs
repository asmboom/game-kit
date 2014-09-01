using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

public class PackInfoListView
{
    public PackInfoListView(VirtualItemPack pack)
    {
        _listControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand | ReorderableListFlags.ShowIndices);

        UpdateItemIndices();
    }

    public void UpdateDisplayItem(VirtualItemPack pack)
    {
        if (pack != null)
        {
            _listAdaptor = new GenericClassListAdaptor<PackElement>(pack.PackElements, 18,
                CreatePackElement, DrawPackElement);

            UpdateItemIndices();
        }
    }

    public void Draw(Rect position)
    {
        GUI.BeginGroup(position, string.Empty, "Box");
        _scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), _scrollPosition, 
            new Rect(0, 0, position.width - 20, _listControl.CalculateListHeight(_listAdaptor) + 20));

        float xOffset = 0;
        GUI.Label(new Rect(0, 0, position.width * 0.5f, 20), 
            "Item", VirtualItemsDrawUtil.TitleStyle);
        xOffset += position.width * 0.5f;
        GUI.Label(new Rect(xOffset, 0, position.width * 0.5f, 20), 
            "Amount", VirtualItemsDrawUtil.TitleStyle);

        if (_listAdaptor != null)
        {
            _listControl.Draw(new Rect(0, 20, position.width, position.height - 20), _listAdaptor);
        }

        GUI.EndScrollView();
        GUI.EndGroup();
    }

    private PackElement CreatePackElement()
    {
        return new PackElement();
    }

    public PackElement DrawPackElement(Rect position, PackElement element, int index)
    {
        DrawVirtualItem(position, element, index);
        DrawAmount(position, element);
        return element;
    }

    public void UpdateItemIndices()
    {
        if (_listAdaptor != null)
        {
            _itemIndices = new List<int>();

            for (var i = 0; i < _listAdaptor.Count; i++)
            {
                _itemIndices.Add(_listAdaptor[i].Item != null ? 
                    VirtualItemsEditUtil.GetItemIndexById(_listAdaptor[i].Item.ID) : 0);
            }
        }
    }

    private void DrawVirtualItem(Rect position, PackElement packElement, int index)
    {
        int newIndex = EditorGUI.Popup(new Rect(position.x, position.y, position.width * 0.5f, position.height),
            _itemIndices[index], VirtualItemsEditUtil.DisplayedItemIDs);
        if (newIndex != _itemIndices[index])
        {
            VirtualItemsEditUtil.UpdatePackElementItemByIndex(packElement, newIndex);
        }
        _itemIndices[index] = newIndex;
    }

    private void DrawAmount(Rect position, PackElement packElement)
    {
        Rect rect = new Rect(position.x + position.width * 0.5f,
                position.y, position.width * 0.4f, position.height);
        if (packElement.Item is LifeTimeItem)
        {
            packElement.Amount = 1;
            EditorGUI.LabelField(rect, packElement.Amount.ToString());
        }
        else
        {
            packElement.Amount = EditorGUI.IntField(rect, packElement.Amount);
        }
    }

    private ReorderableListControl _listControl;
    private GenericClassListAdaptor<PackElement> _listAdaptor;
    private List<int> _itemIndices;
    private Vector2 _scrollPosition;
}