using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

public class PackInfoListView
{
    public PackInfoListView(VirtualItemPack pack)
    {
        _listControl = new ReorderableListControl();

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

    public void Draw()
    {
        GUILayout.BeginHorizontal();
        VirtualItemsDrawUtil.BeginDrawTitle();
        GUILayout.Label("Item");
        GUILayout.Label("Amount");
        VirtualItemsDrawUtil.EndDrawTitle();
        GUILayout.EndHorizontal();

        if (_listAdaptor != null)
        {
            _listControl.Draw(_listAdaptor);
        }
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

    private const float PurchaseTypeWidth = 0.33f;
    private const float PurchaseAssociatedWidth = 0.33f;
    private const float PurchasePriceWidth = 0.33f;
}