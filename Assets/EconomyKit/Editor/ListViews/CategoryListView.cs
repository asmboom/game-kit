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

        DrawTitle();

        EditorGUI.BeginChangeCheck();
        _listControl.Draw(_listAdaptor);
        if (EditorGUI.EndChangeCheck() && ReorderableListGUI.indexOfChangedItem != -1)
        {
            EditorUtility.SetDirty(_listAdaptor[ReorderableListGUI.indexOfChangedItem]);
        }
    }

    private void DrawTitle()
    {
        VirtualItemsDrawUtil.BeginDrawTitle();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Category ID");
        GUILayout.Label("Items");
        GUILayout.EndHorizontal();
        VirtualItemsDrawUtil.EndDrawTitle();
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

    public VirtualCategory DrawItem(Rect position, VirtualCategory category, int index)
    {
        string controlName = category.GetInstanceID() + "_input_field";
        GUI.SetNextControlName(controlName);

        if (EditorGUI.TextField(new Rect(position.x, position.y, position.width * 0.2f, position.height),
            category.ID).KeyPressed<string>(controlName, KeyCode.Return, out category.ID))
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(category), string.Format("Category{0}", category.ID));
        }

        GUI.Label(new Rect(position.x + position.width * 0.25f, position.y, position.width * 0.8f, position.height),
            GetCharacterItemsString(category));
        return category;
    }

    private string GetCharacterItemsString(VirtualCategory category)
    {
        if (category.Items.Count > 0)
        {
            string final = string.Empty;
            for (int i = 0; i < category.Items.Count; i++)
            {
                final += category.Items[i].Name;
                if (i < category.Items.Count - 1)
                {
                    final += ", ";
                }
            }
            return final;
        }
        else
        {
            return string.Empty;
        }
    }

    private ReorderableListControl _listControl;
    private GenericClassListAdaptor<VirtualCategory> _listAdaptor;
    private Vector2 _scrollPosition;
}