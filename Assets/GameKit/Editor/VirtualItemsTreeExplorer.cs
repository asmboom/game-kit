using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System;

namespace Beetle23
{
    public class VirtualItemsTreeExplorer
    {
        public Action<object> OnSelectionChange = delegate { };
        public IItem CurrentSelectedItem { get; private set; }

        public VirtualItemsTreeExplorer(VirtualItemsConfig config)
        {
            _config = config;

            _virtualCurrencyListAdaptor = CreateVirtualItemListAdaptor<VirtualCurrency>(config.VirtualCurrencies);
            _virtualCurrencyListAdaptor.OnOrderChagne += OnListOrderChange<VirtualCurrency>;
            _virtualCurrencyListAdaptor.OnItemRemoved += VirtualItemsEditUtil.UpdateDisplayedOptions;
            _virtualCurrencyListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand);
            _virtualCurrencyListControl.ItemRemoving += OnItemRemoving<VirtualCurrency>;
            _virtualCurrencyListControl.ItemInserted += OnItemInsert<VirtualCurrency>;

            _singleuseItemListAdaptor = CreateVirtualItemListAdaptor<SingleUseItem>(config.SingleUseItems);
            _singleuseItemListAdaptor.OnOrderChagne += OnListOrderChange<SingleUseItem>;
            _singleuseItemListAdaptor.OnItemRemoved += VirtualItemsEditUtil.UpdateDisplayedOptions;
            _singleuseItemListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand);
            _singleuseItemListControl.ItemRemoving += OnItemRemoving<SingleUseItem>;
            _singleuseItemListControl.ItemInserted += OnItemInsert<SingleUseItem>;

            _lifetimeItemListAdaptor = CreateVirtualItemListAdaptor<LifeTimeItem>(config.LifeTimeItems);
            _lifetimeItemListAdaptor.OnOrderChagne += OnListOrderChange<LifeTimeItem>;
            _lifetimeItemListAdaptor.OnItemRemoved += VirtualItemsEditUtil.UpdateDisplayedOptions;
            _lifetimeItemListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand);
            _lifetimeItemListControl.ItemRemoving += OnItemRemoving<LifeTimeItem>;
            _lifetimeItemListControl.ItemInserted += OnItemInsert<LifeTimeItem>;

            _packListAdaptor = CreateVirtualItemListAdaptor<VirtualItemPack>(config.ItemPacks);
            _packListAdaptor.OnOrderChagne += OnListOrderChange<VirtualItemPack>;
            _packListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand);
            _packListControl.ItemRemoving += OnItemRemoving<VirtualItemPack>;
            _packListControl.ItemInserted += OnItemInsert<VirtualItemPack>;

            _categoryListAdaptor = CreateVirtualCategoryListAdaptor(config.Categories);
            _categoryListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand);
            _categoryListControl.ItemInserted += OnItemInsert<VirtualCategory>;
            _categoryListControl.ItemRemoving += OnItemRemoving<VirtualCategory>;
        }

        public void Draw(Rect position)
        {
            GUILayout.BeginArea(position, string.Empty, "Box");

            if (GUILayout.Button("Check Errors"))
            {
                VirtualItemsConfigEditor.CheckIfAnyInvalidRef(_config);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+Expand All", GUILayout.Width(90)))
            {
                _isVirtualCurrencyExpanded = true;
                _isSingleUseItemExpanded = true;
                _isLifeTimeItemExpanded = true;
                _isPackExpanded = true;
                _isCategoryExpanded = true;
            }
            if (GUILayout.Button("-Collapse All", GUILayout.Width(90)))
            {
                _isVirtualCurrencyExpanded = false;
                _isSingleUseItemExpanded = false;
                _isLifeTimeItemExpanded = false;
                _isPackExpanded = false;
                _isCategoryExpanded = false;
            }
            GUILayout.EndHorizontal();

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            EditorGUI.BeginChangeCheck();

            _isVirtualCurrencyExpanded = EditorGUILayout.Foldout(_isVirtualCurrencyExpanded,
                new GUIContent(" Virtual Currencies", Resources.Load("VirtualCurrencyIcon") as Texture,
                    "Virtual currency can be used to purchase other items, e.g. coin, gem"),
                VirtualItemsDrawUtil.FoldoutStyle);
            if (_isVirtualCurrencyExpanded)
            {
                _virtualCurrencyListControl.Draw(_virtualCurrencyListAdaptor);
            }
            _isSingleUseItemExpanded = EditorGUILayout.Foldout(_isSingleUseItemExpanded,
                new GUIContent(" Single Use Items", Resources.Load("SingleUseItemIcon") as Texture,
                    "Items that use can buy multiple times and use multiple times, e.g. magic spells."),
                VirtualItemsDrawUtil.FoldoutStyle);
            if (_isSingleUseItemExpanded)
            {
                _singleuseItemListControl.Draw(_singleuseItemListAdaptor);
            }
            _isLifeTimeItemExpanded = EditorGUILayout.Foldout(_isLifeTimeItemExpanded,
                new GUIContent(" Lifetime Items", Resources.Load("LifetimeItemIcon") as Texture,
                    "Items that bought only once and kept forever, e.g. no ads, characters, weapons"),
                VirtualItemsDrawUtil.FoldoutStyle);
            if (_isLifeTimeItemExpanded)
            {
                _lifetimeItemListControl.Draw(_lifetimeItemListAdaptor);
            }
            _isPackExpanded = EditorGUILayout.Foldout(_isPackExpanded,
                new GUIContent(" Packs", Resources.Load("PackIcon") as Texture,
                    "A pack contains a list of various virtual items"),
                VirtualItemsDrawUtil.FoldoutStyle);
            if (_isPackExpanded)
            {
                _packListControl.Draw(_packListAdaptor);
            }
            _isCategoryExpanded = EditorGUILayout.Foldout(_isCategoryExpanded,
                new GUIContent(" Categories", Resources.Load("CategoryIcon") as Texture),
                VirtualItemsDrawUtil.FoldoutStyle);
            if (_isCategoryExpanded)
            {
                _categoryListControl.Draw(_categoryListAdaptor);
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_config);
            }

            GUILayout.Space(30);

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        private T DrawItem<T>(Rect position, T item, int index) where T : IItem
        {
            if (item == null)
            {
                GUI.Label(position, "NULL");
                return item;
            }

            if (GUI.Button(position, item.ID,
                    (!string.IsNullOrEmpty(item.ID) && CurrentSelectedItem != null && item.ID == CurrentSelectedItem.ID ?
                        VirtualItemsDrawUtil.ItemSelectedStyle : VirtualItemsDrawUtil.ItemStyle)))
            {
                SelectItem(item);
            }
            return item;
        }

        private void OnItemInsert<T>(object sender, ItemInsertedEventArgs args) where T : class
        {
            GenericClassListAdaptor<T> listAdaptor = args.adaptor as GenericClassListAdaptor<T>;
            if (listAdaptor != null)
            {
                VirtualItem item = listAdaptor[args.itemIndex] as VirtualItem;
                if (item != null)
                {
                    item.ID = item.name;
                    item.SortIndex = listAdaptor.Count - 1;

                    VirtualItemsEditUtil.UpdateDisplayedOptions();
                }
                else
                {
                    VirtualCategory category = listAdaptor[args.itemIndex] as VirtualCategory;
                    if (category != null)
                    {
                        category.ID = "New Category";
                    }
                }
            }
        }

        private void OnItemRemoving<T>(object sender, ItemRemovingEventArgs args) where T : class
        {
            GenericClassListAdaptor<T> listAdaptor = args.adaptor as GenericClassListAdaptor<T>;
            T item = listAdaptor[args.itemIndex];
            if (listAdaptor != null)
            {
                if (item is VirtualItem)
                {
                    VirtualItem virtualItem = item as VirtualItem;
                    if (EditorUtility.DisplayDialog("Confirm to delete",
                            "Confirm to delete asset [" + virtualItem.name + ".asset]?", "OK", "Cancel"))
                    {
                        args.Cancel = false;
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(virtualItem));
                    }
                    else
                    {
                        args.Cancel = true;
                    }
                }
                else if (item is VirtualCategory)
                {
                    VirtualCategory category = item as VirtualCategory;
                    if (EditorUtility.DisplayDialog("Confirm to delete",
                            "Confirm to delete category [" + category.ID + "]?", "OK", "Cancel"))
                    {
                        args.Cancel = false;
                    }
                    else
                    {
                        args.Cancel = true;
                    }
                }
            }
        }

        private void OnListOrderChange<T>(IList<T> list) where T : VirtualItem
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SortIndex = i;
                EditorUtility.SetDirty(list[i]);
            }
        }

        private GenericClassListAdaptor<T> CreateVirtualItemListAdaptor<T>(List<T> items) where T : VirtualItem
        {
            return new GenericClassListAdaptor<T>(items, 20,
                () =>
                {
                    return VirtualItemsEditUtil.CreateNewVirtualItem<T>();
                }, DrawItem<T>);
        }

        private GenericClassListAdaptor<VirtualCategory> CreateVirtualCategoryListAdaptor(List<VirtualCategory> categories)
        {
            return new GenericClassListAdaptor<VirtualCategory>(categories, 20,
                                    () =>
                                    {
                                        return new VirtualCategory();
                                    }, DrawItem<VirtualCategory>);
        }

        private void SelectItem(IItem item)
        {
            if (item != CurrentSelectedItem)
            {
                CurrentSelectedItem = item;
                OnSelectionChange(item);
            }
        }

        private VirtualItemsConfig _config;
        private bool _isVirtualCurrencyExpanded = true;
        private bool _isSingleUseItemExpanded = true;
        private bool _isLifeTimeItemExpanded = true;
        private bool _isPackExpanded = true;
        private bool _isCategoryExpanded = true;

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

        private Vector2 _scrollPosition;
    }
}
