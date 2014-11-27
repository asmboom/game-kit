using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System;

namespace Beetle23
{
    public class VirtualItemsTreeExplorer
    {
        public Action<IItem> OnSelectionChange = delegate { };
        public IItem CurrentSelectedItem { get; private set; }

        public VirtualItemsTreeExplorer(GameKitConfig config)
        {
            _config = config;

            _virtualCurrencyListAdaptor = new GenericClassListAdaptor<VirtualCurrency>(config.VirtualCurrencies, 20,
                                    () => { return new VirtualCurrency(); },
                                    DrawItem<VirtualCurrency>);
            _virtualCurrencyListAdaptor.OnItemRemoved += VirtualItemsEditUtil.UpdateDisplayedOptions;
            _virtualCurrencyListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand);
            _virtualCurrencyListControl.ItemRemoving += OnItemRemoving<VirtualCurrency>;
            _virtualCurrencyListControl.ItemInserted += OnItemInsert<VirtualCurrency>;

            _singleuseItemListAdaptor = new GenericClassListAdaptor<SingleUseItem>(config.SingleUseItems, 20,
                                    () => { return new SingleUseItem(); },
                                    DrawItem<SingleUseItem>);
            _singleuseItemListAdaptor.OnItemRemoved += VirtualItemsEditUtil.UpdateDisplayedOptions;
            _singleuseItemListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand);
            _singleuseItemListControl.ItemRemoving += OnItemRemoving<SingleUseItem>;
            _singleuseItemListControl.ItemInserted += OnItemInsert<SingleUseItem>;

            _lifetimeItemListAdaptor = new GenericClassListAdaptor<LifeTimeItem>(config.LifeTimeItems, 20,
                                    () => { return new LifeTimeItem(); },
                                    DrawItem<LifeTimeItem>);
            _lifetimeItemListAdaptor.OnItemRemoved += VirtualItemsEditUtil.UpdateDisplayedOptions;
            _lifetimeItemListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand);
            _lifetimeItemListControl.ItemRemoving += OnItemRemoving<LifeTimeItem>;
            _lifetimeItemListControl.ItemInserted += OnItemInsert<LifeTimeItem>;

            _packListAdaptor = new GenericClassListAdaptor<VirtualItemPack>(config.ItemPacks, 20,
                                    () => { return new VirtualItemPack(); },
                                    DrawItem<VirtualItemPack>);
            _packListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand);
            _packListControl.ItemRemoving += OnItemRemoving<VirtualItemPack>;
            _packListControl.ItemInserted += OnItemInsert<VirtualItemPack>;

            _categoryListAdaptor = new GenericClassListAdaptor<VirtualCategory>(config.Categories, 20,
                                    () => { return new VirtualCategory(); },
                                    DrawItem<VirtualCategory>);
            _categoryListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand);
            _categoryListControl.ItemInserted += OnItemInsert<VirtualCategory>;
            _categoryListControl.ItemRemoving += OnItemRemoving<VirtualCategory>;
        }

        public void Draw(Rect position)
        {
            GUILayout.BeginArea(position, string.Empty, "Box");

            if (GUILayout.Button("Check References", GUILayout.Width(185)))
            {
                GameKitConfigEditor.CheckIfAnyInvalidRef(_config);
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

            _isVirtualCurrencyExpanded = EditorGUILayout.Foldout(_isVirtualCurrencyExpanded,
                new GUIContent(" Virtual Currencies", Resources.Load("VirtualCurrencyIcon") as Texture,
                    "Virtual currency can be used to purchase other items, e.g. coin, gem"),
                GameKitEditorDrawUtil.FoldoutStyle);
            if (_isVirtualCurrencyExpanded)
            {
                _virtualCurrencyListControl.Draw(_virtualCurrencyListAdaptor);
            }
            _isSingleUseItemExpanded = EditorGUILayout.Foldout(_isSingleUseItemExpanded,
                new GUIContent(" Single Use Items", Resources.Load("SingleUseItemIcon") as Texture,
                    "Items that use can buy multiple times and use multiple times, e.g. magic spells."),
                GameKitEditorDrawUtil.FoldoutStyle);
            if (_isSingleUseItemExpanded)
            {
                _singleuseItemListControl.Draw(_singleuseItemListAdaptor);
            }
            _isLifeTimeItemExpanded = EditorGUILayout.Foldout(_isLifeTimeItemExpanded,
                new GUIContent(" Lifetime Items", Resources.Load("LifetimeItemIcon") as Texture,
                    "Items that bought only once and kept forever, e.g. no ads, characters, weapons"),
                GameKitEditorDrawUtil.FoldoutStyle);
            if (_isLifeTimeItemExpanded)
            {
                _lifetimeItemListControl.Draw(_lifetimeItemListAdaptor);
            }
            _isPackExpanded = EditorGUILayout.Foldout(_isPackExpanded,
                new GUIContent(" Packs", Resources.Load("PackIcon") as Texture,
                    "A pack contains a list of various virtual items"),
                GameKitEditorDrawUtil.FoldoutStyle);
            if (_isPackExpanded)
            {
                _packListControl.Draw(_packListAdaptor);
            }
            _isCategoryExpanded = EditorGUILayout.Foldout(_isCategoryExpanded,
                new GUIContent(" Categories", Resources.Load("CategoryIcon") as Texture),
                GameKitEditorDrawUtil.FoldoutStyle);
            if (_isCategoryExpanded)
            {
                _categoryListControl.Draw(_categoryListAdaptor);
            }

            GUILayout.Space(30);

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        private T DrawItem<T>(Rect position, T item, int index) where T : SerializableItem
        {
            if (item == null)
            {
                GUI.Label(position, "NULL");
                return item;
            }

            if (GUI.Button(position, item.ID,
                    (!string.IsNullOrEmpty(item.ID) && item == CurrentSelectedItem ?
                        GameKitEditorDrawUtil.ItemSelectedCenterStyle : GameKitEditorDrawUtil.ItemCenterLabelStyle)))
            {
                SelectItem(item);
            }
            return item;
        }

        private void OnItemInsert<T>(object sender, ItemInsertedEventArgs args) where T : SerializableItem
        {
            GenericClassListAdaptor<T> listAdaptor = args.adaptor as GenericClassListAdaptor<T>;
            if (listAdaptor != null)
            {
                SelectItem(listAdaptor[args.itemIndex]);
                if (listAdaptor[args.itemIndex] is VirtualItem)
                {
                    ShowInputDialogForId<T>(CurrentSelectedItem.ID);
                }
            }
        }

        private void ShowInputDialogForId<T>(string defaultId) where T : SerializableItem
        {
            SingleInputDialog.Show("Enter id for the new item", defaultId, "OK", OnGetNewId<T>);
        }

        private void OnGetNewId<T>(string id) where T : SerializableItem
        {
            GameKit.Config.UpdateMapsAndTree();
            SerializableItem itemWithID = GameKit.Config.GetVirtualItemByID(id);
            if (itemWithID != null && itemWithID != CurrentSelectedItem)
            {
                Debug.LogWarning("Id [" + id + "] is already used by [" + 
                    itemWithID.Name + "], please change one.");
                ShowInputDialogForId<T>(id);
            }
            else
            {
                CurrentSelectedItem.ID = id;
                GameKitEditorWindow.GetInstance().Repaint();
            }
        }

        private void OnItemRemoving<T>(object sender, ItemRemovingEventArgs args) where T : SerializableItem
        {
            GenericClassListAdaptor<T> listAdaptor = args.adaptor as GenericClassListAdaptor<T>;
            T item = listAdaptor[args.itemIndex];
            if (listAdaptor != null)
            {
                if (EditorUtility.DisplayDialog("Confirm to delete",
                        "Confirm to delete item [" + item.ID + "]?", "OK", "Cancel"))
                {
                    args.Cancel = false;
                    SelectItem(null);
                    GameKitEditorWindow.GetInstance().Repaint();
                }
                else
                {
                    args.Cancel = true;
                }
            }
        }

        private void SelectItem(IItem item)
        {
            if (item != CurrentSelectedItem)
            {
                CurrentSelectedItem = item;
                OnSelectionChange(item);
            }
        }

        private GameKitConfig _config;
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
