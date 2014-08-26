using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;

public class SingleUseItemListView : IView
{
    public SingleUseItemListView(List<SingleUseItem> list)
    {
        _list = list;
        _listAdaptor = new GenericClassListAdaptor<SingleUseItem>(list, 22, 
            CreateSingleUseItem, DrawItem);
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

    public SingleUseItem CreateSingleUseItem()
    {
        return VirtualItemsEditUtil.CreateNewVirtualItem<SingleUseItem>();
    }

    private void DrawTitle(Rect position)
    {
        VirtualItemsDrawUtil.BeginDrawTitle();
        VirtualItemsDrawUtil.DrawVirtualItemTitle(position.x, position.y, position.height);
        VirtualItemsDrawUtil.EndDrawTitle();
    }

    public SingleUseItem DrawItem(Rect position, SingleUseItem item, int index)
    {
        VirtualItemsDrawUtil.DrawVirtualItemInfo(position.x, position.y, position.height, item, index, _categoryIndices);
        return item;
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

    private List<SingleUseItem> _list;
    private ReorderableListControl _listControl;
    private GenericClassListAdaptor<SingleUseItem> _listAdaptor;
    private List<int> _categoryIndices;

    private const float IdWidth = 0.2f;
    private const float NameWidth = 0.2f;
    private const float DescriptionWidth = 0.4f;
    private const float CategoryWidth = 0.2f;
    private Vector2 _scrollPosition;
}