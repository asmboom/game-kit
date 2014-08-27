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
        EditorGUI.BeginChangeCheck();
        _listControl.Draw(new Rect(0, yOffset + 20, width, listHeight), _listAdaptor);
        if (EditorGUI.EndChangeCheck() && ReorderableListGUI.indexOfChangedItem != -1)
        {
            EditorUtility.SetDirty(_listAdaptor[ReorderableListGUI.indexOfChangedItem]);
        }

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
        VirtualItemsDrawUtil.BeginDrawTitle();
        VirtualItemsDrawUtil.DrawVirtualItemTitle(position.x, position.y, position.height);
        VirtualItemsDrawUtil.EndDrawTitle();
    }

    public VirtualCurrency DrawItem(Rect position, VirtualCurrency virtualCurrency, int index)
    {
        VirtualItemsDrawUtil.DrawVirtualItemInfo(position.x, position.y, position.height, virtualCurrency, index, _categoryIndices);
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
    private Vector2 _scrollPosition;
}