using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Codeplay
{
    public class PackInfoListView
    {
        public PackInfoListView(VirtualItemPack pack)
        {
            _listControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand |
                ReorderableListFlags.ShowIndices);
            _listControl.ItemInserted += OnItemInsert;
            _listControl.ItemRemoving += OnItemRemoving;

            _itemPopupDrawers = new List<ItemPopupDrawer>();
            UpdateItemPopupDrawers();
        }

        public void UpdateDisplayItem(VirtualItemPack pack)
        {
            _currentDisplayedPack = pack;
            if (pack != null)
            {
                _listAdaptor = new GenericClassListAdaptor<PackElement>(pack.PackElements, 18,
                    CreatePackElement, DrawPackElement);

                UpdateItemPopupDrawers();
            }
        }

        public void Draw(Rect position)
        {
            GUI.BeginGroup(position, string.Empty, "Box");
            float listHeight = _listControl.CalculateListHeight(_listAdaptor);
            bool hasScrollBar = listHeight + 20 > position.height;
            _scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), _scrollPosition,
                new Rect(0, 0, position.width - 20, listHeight + 20));

            float xOffset = 0;
            GUI.Label(new Rect(0, 0, position.width * 0.5f, 20),
                "Item", GameKitEditorDrawUtil.TitleStyle);
            xOffset += position.width * 0.5f;
            GUI.Label(new Rect(xOffset, 0, position.width * 0.5f, 20),
                "Amount", GameKitEditorDrawUtil.TitleStyle);

            if (_listAdaptor != null)
            {
                _listControl.Draw(new Rect(0, 20,
                    position.width - (hasScrollBar ? 10 : 0), listHeight), _listAdaptor);
            }

            GUI.EndScrollView();
            GUI.EndGroup();
        }

        private PackElement CreatePackElement()
        {
            return new PackElement();
        }

        private void OnItemRemoving(object sender, ItemRemovingEventArgs args)
        {
            UpdateItemPopupDrawers();
        }

        private void OnItemInsert(object sender, ItemInsertedEventArgs args)
        {
            UpdateItemPopupDrawers();
        }

        public PackElement DrawPackElement(Rect position, PackElement element, int index)
        {
            DrawVirtualItem(position, element, index);
            DrawAmount(position, element);
            return element;
        }

        private void DrawVirtualItem(Rect position, PackElement packElement, int index)
        {
            ItemPopupDrawer drawer = index < _itemPopupDrawers.Count ? _itemPopupDrawers[index] : null;
            if (drawer != null)
            {
                packElement.ItemID = drawer.Draw(new Rect(position.x, position.y, position.width * 0.5f - 1, position.height), 
                    packElement.ItemID, GUIContent.none);
            }
        }

        private void DrawAmount(Rect position, PackElement packElement)
        {
            Rect rect = new Rect(position.x + position.width * 0.5f,
                    position.y, position.width * 0.5f - 1, position.height);
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

        private void UpdateItemPopupDrawers()
        {
            _itemPopupDrawers.Clear();
            if (_currentDisplayedPack != null)
            {
                for (int i = 0; i < _currentDisplayedPack.PackElements.Count; i++)
                {
                    _itemPopupDrawers.Add(new ItemPopupDrawer(ItemType.VirtualItem,
                        false, VirtualItemType.VirtualCurrency | VirtualItemType.SingleUseItem | VirtualItemType.LifeTimeItem));
                }
            }
        }

        private VirtualItemPack _currentDisplayedPack;
        private ReorderableListControl _listControl;
        private GenericClassListAdaptor<PackElement> _listAdaptor;
        private List<ItemPopupDrawer> _itemPopupDrawers;
        private Vector2 _scrollPosition;
    }
}