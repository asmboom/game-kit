using Rotorz.ReorderableList;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PackElementsEditorWindow : EditorWindow
{
    public void Init(VirtualItemPack pack)
    {
        _currentEditPack = pack;
        _listAdaptor = new GenericClassListAdaptor<PackElement>(_currentEditPack.PackElements, 22, 
            CratePackElement, DrawItem);

        UpdateItemIndices();
    }

    private void OnEnable()
    {
        _listControl = new ReorderableListControl();
        _listControl.ItemInserted += OnItemInsert;
        _listControl.ItemRemoving += OnItemRemoving;
    }

    private void OnDisable()
    {
        if (_listControl != null)
        {
            _listControl.ItemInserted -= OnItemInsert;
            _listControl.ItemRemoving -= OnItemRemoving;
        }
    }

    private void OnDestroy()
    {
        for(var i = 0; i < _listAdaptor.Count; i++)
        {
            VirtualItemsEditUtil.UpdatePackElementItemByIndex(_listAdaptor[i], _itemIndices[i]);
        }
    }

    private void OnItemRemoving(object sender, ItemRemovingEventArgs args)
    {
        UpdateItemIndices();
    }

    private void OnItemInsert(object sender, ItemInsertedEventArgs args)
    {
        UpdateItemIndices();
    }

    private void OnGUI()
    {
        if (_currentEditPack == null || _listAdaptor == null) return;

        var centeredStyle = GUI.skin.GetStyle("Label");

        centeredStyle.richText = true;
        GUILayout.Label("<size=15><color=lightblue>" + _currentEditPack.Name + "</color> content</size>");
        GUILayout.Space(5);

        var oldAlignment = centeredStyle.alignment;
        var oldFontSize = centeredStyle.fontSize;
        centeredStyle.alignment = TextAnchor.UpperCenter;
        centeredStyle.fontSize = 12;
        centeredStyle.fontStyle = FontStyle.Bold;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Item", GUILayout.Width(this.position.width * 0.6f));
        GUILayout.Label("Amount", GUILayout.Width(this.position.width * 0.3f));
        GUILayout.EndHorizontal();

        centeredStyle.alignment = oldAlignment;
        centeredStyle.fontSize = oldFontSize;
        centeredStyle.fontStyle = FontStyle.Normal;
        centeredStyle.richText = false;

        _listControl.Draw(_listAdaptor);
    }

    public PackElement CratePackElement()
    {
        return new PackElement();
    }

    public PackElement DrawItem(Rect position, PackElement element, int index)
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
        int newIndex = EditorGUI.Popup(new Rect(position.x, position.y, position.width * 0.6f, position.height),
            _itemIndices[index], VirtualItemsEditUtil.DisplayedItemIDs);
        if (newIndex != _itemIndices[index])
        {
            VirtualItemsEditUtil.UpdatePackElementItemByIndex(packElement, newIndex);
        }
        _itemIndices[index] = newIndex;
    }

    private void DrawAmount(Rect position, PackElement packElement)
    {
        Rect rect = new Rect(position.x + position.width * 0.6f,
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

    private VirtualItemPack _currentEditPack;
    private ReorderableListControl _listControl;
    private GenericClassListAdaptor<PackElement> _listAdaptor;
    private List<int> _itemIndices;
}
