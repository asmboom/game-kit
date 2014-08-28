using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;

public class CategoryListView : IView
{
    public CategoryListView(List<VirtualCategory> list)
    {
        _listAdaptor = new GenericClassListAdaptor<VirtualCategory>(list, 22, 
            CreateCategory, DrawItem);
        _listControl = new ReorderableListControl();
    }

    public void Show()
    {
        _listControl.ItemInserted += OnItemInsert;
        _listControl.ItemRemoving += OnItemRemoving;

        VirtualItemsEditUtil.UpdateDisplayedOptions();
    }

    public void Hide()
    {
        _listControl.ItemInserted -= OnItemInsert;
        _listControl.ItemRemoving -= OnItemRemoving;
    }

    public void Draw(Rect position) 
    {
        if (_listAdaptor == null) return;

        EditorGUI.BeginChangeCheck();
        _listControl.Draw(_listAdaptor);
        if (EditorGUI.EndChangeCheck() && ReorderableListGUI.indexOfChangedItem != -1)
        {
            EditorUtility.SetDirty(_listAdaptor[ReorderableListGUI.indexOfChangedItem]);
        }
    }

    private void OnItemRemoving(object sender, ItemRemovingEventArgs args)
    {
        if (EditorUtility.DisplayDialog("Confirm to delete",
                "Confirm to delete virtual category [" + _listAdaptor[args.itemIndex].ID + "]?", "OK", "Cancel"))
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
        VirtualItemsEditUtil.UpdateDisplayedOptions();
    }

    public VirtualCategory CreateCategory()
    {
        return VirtualItemsEditUtil.CreateNewCategory();
    }

    public VirtualCategory DrawItem(Rect position, VirtualCategory item, int index)
    {
        string controlName = item.GetInstanceID() + "_input_field";
        GUI.SetNextControlName(controlName);

        if (EditorGUI.TextField(position, item.ID).KeyPressed<string>(controlName, KeyCode.Return, out item.ID))
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), string.Format("Category{0}", item.ID));
        }
        return item;
    }

    private ReorderableListControl _listControl;
    private GenericClassListAdaptor<VirtualCategory> _listAdaptor;
    private Vector2 _scrollPosition;
}