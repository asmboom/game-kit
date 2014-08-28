using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;

public class VirtualItemsTreeExplorer
{
    public VirtualItemsTreeExplorer(VirtualItemsConfig config)
    {
        _virtualCurrencyListAdaptor = CreateVirtualItemListAdaptor<VirtualCurrency>(config.VirtualCurrencies);
        _virtualCurrencyListControl = new ReorderableListControl();
        _singleuseItemListAdaptor = CreateVirtualItemListAdaptor<SingleUseItem>(config.SingleUseItems);
        _singleuseItemListControl = new ReorderableListControl();
        _lifetimeItemListAdaptor = CreateVirtualItemListAdaptor<LifeTimeItem>(config.LifeTimeItems);
        _lifetimeItemListControl = new ReorderableListControl();
        _packListAdaptor = CreateVirtualItemListAdaptor<VirtualItemPack>(config.ItemPacks);
        _packListControl = new ReorderableListControl();
        _categoryListAdaptor = CreateVirtualCategoryListAdaptor(config.Categories);
        _categoryListControl = new ReorderableListControl();
    }

    public void Draw(Rect position)
    {
        GUILayout.BeginArea(position);
        _isVirtualCurrencyExpanded = EditorGUILayout.Foldout(_isVirtualCurrencyExpanded, "Virtual Currencies");
        if (_isVirtualCurrencyExpanded)
        {
            _virtualCurrencyListControl.Draw(_virtualCurrencyListAdaptor);
        }
        _isSingleUseItemExpanded = EditorGUILayout.Foldout(_isSingleUseItemExpanded, "Single Use Items");
        if (_isSingleUseItemExpanded)
        {
            _singleuseItemListControl.Draw(_singleuseItemListAdaptor);
        }
        _isLifeTimeItemExpanded = EditorGUILayout.Foldout(_isLifeTimeItemExpanded, "Lifetime Items");
        if (_isLifeTimeItemExpanded)
        {
            _lifetimeItemListControl.Draw(_lifetimeItemListAdaptor);
        }
        _isPackExpanded = EditorGUILayout.Foldout(_isPackExpanded, "Packs");
        if (_isPackExpanded)
        {
            _packListControl.Draw(_packListAdaptor);
        }
        _isCategoryExpanded = EditorGUILayout.Foldout(_isCategoryExpanded, "Category");
        if (_isCategoryExpanded)
        {
            _categoryListControl.Draw(_categoryListAdaptor);
        }
        GUILayout.EndArea();
    }

    private T DrawItem<T>(Rect position, T item, int index) where T : ScriptableObject
    {
        string id = string.Empty;
        if (item is VirtualItem)
        {
            id = (item as VirtualItem).ID;
        }
        else if (item is VirtualCategory)
        {
            id = (item as VirtualCategory).ID;
        }
        else
        {
            return item;
        }

        GUIStyle labelStyle = GUI.skin.GetStyle("Label");
        labelStyle.richText = true;
        string caption = Tab + id;
        if (item == _currentSelectedItem)
        {
            caption = "<color=red>" + caption + "</color>";
        }
        if (GUI.Button(position, caption, "Label"))
        {
            _currentSelectedItem = item;
        }
        labelStyle.richText = false;
        return item;
    }

    private void OnItemRemoving(object sender, ItemRemovingEventArgs args)
    {
        /*
        GenericClassListAdaptor adaptor = 
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
        */
    }

    private GenericClassListAdaptor<T> CreateVirtualItemListAdaptor<T>(List<T> items) where T : VirtualItem
    {
        return new GenericClassListAdaptor<T>(items, 15, 
            ()=>{
                return VirtualItemsEditUtil.CreateNewVirtualItem<T>();
            }, DrawItem<T>);
    }

    private GenericClassListAdaptor<VirtualCategory> CreateVirtualCategoryListAdaptor(List<VirtualCategory> categories)
    {
        return new GenericClassListAdaptor<VirtualCategory>(categories, 15, 
                                ()=>{
                                    return VirtualItemsEditUtil.CreateNewCategory();
                                }, DrawItem<VirtualCategory>);
    }

    private void SelectItem(VirtualItem item)
    {
        _currentSelectedItem = item;
    }

    private bool _isVirtualCurrencyExpanded;
    private bool _isSingleUseItemExpanded;
    private bool _isLifeTimeItemExpanded;
    private bool _isPackExpanded;
    private bool _isCategoryExpanded;

    private ReorderableListControl _virtualCurrencyListControl;
    private GenericClassListAdaptor<VirtualCurrency> _virtualCurrencyListAdaptor;
    private ReorderableListControl _singleuseItemListControl;
    private GenericClassListAdaptor<SingleUseItem> _singleuseItemListAdaptor;
    private ReorderableListControl _lifetimeItemListControl;
    private GenericClassListAdaptor<LifeTimeItem> _lifetimeItemListAdaptor;
    private ReorderableListControl _packListControl;
    private GenericClassListAdaptor<VirtualItemPack> _packListAdaptor;
    private ReorderableListControl _categoryListControl;
    private GenericClassListAdaptor<VirtualCategory> _categoryListAdaptor;

    private ScriptableObject _currentSelectedItem = null;

    private const string Tab = "        ";
}
