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
                _itemIndices.Add(VirtualItemsEditUtil.GetItemIndexById(_listAdaptor[i].ItemID));
            }
        }
    }

    private void DrawVirtualItem(Rect position, PackElement packElement, int index)
    {
        GUI.changed = false;
        _itemIndices[index] = EditorGUI.Popup(new Rect(position.x, position.y, position.width * 0.6f, position.height),
            _itemIndices[index], VirtualItemsEditUtil.DisplayedItemIDs);
        if (GUI.changed)
        {
            VirtualItemsEditUtil.UpdatePackElementItemByIndex(packElement, _itemIndices[index]);
        }
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
