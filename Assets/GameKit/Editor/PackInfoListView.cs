using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
{
    public class PackInfoListView
    {
        public PackInfoListView(VirtualItemPack pack)
        {
            _listControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand |
                ReorderableListFlags.ShowIndices);
            _listControl.ItemInserted += OnItemInsert;
            _listControl.ItemRemoving += OnItemRemoving;

            UpdateItemIndices();
        }

        public void UpdateDisplayItem(VirtualItemPack pack)
        {
            _currentDisplayedPack = pack;
            if (pack != null)
            {
                _listAdaptor = new GenericClassListAdaptor<PackElement>(pack.PackElements, 18,
                    CreatePackElement, DrawPackElement);

                UpdateItemIndices();
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
                "Item", VirtualItemsDrawUtil.TitleStyle);
            xOffset += position.width * 0.5f;
            GUI.Label(new Rect(xOffset, 0, position.width * 0.5f, 20),
                "Amount", VirtualItemsDrawUtil.TitleStyle);

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
            UpdateItemIndices();
        }

        private void OnItemInsert(object sender, ItemInsertedEventArgs args)
        {
            UpdateItemIndices();
        }

        public PackElement DrawPackElement(Rect position, PackElement element, int index)
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
                    if (_listAdaptor[i].Item == null && VirtualItemsEditUtil.DisplayedItemIDs.Length > 0)
                    {
                        VirtualItem item = GameKit.Config.GetItemByID(VirtualItemsEditUtil.DisplayedItemIDs[0]);
                        if (item != null)
                        {
                            Debug.LogWarning("One of pack " + _currentDisplayedPack.Name + "'s items is null, correct it with default item [" + item.ID + "].");
                        }
                        _listAdaptor[i].Item = item;
                    }
                    _itemIndices.Add(_listAdaptor[i].Item != null ?
                        VirtualItemsEditUtil.GetItemIndexById(_listAdaptor[i].Item.ID) : 0);
                }
            }
        }

        private void DrawVirtualItem(Rect position, PackElement packElement, int index)
        {
            if (index < _itemIndices.Count)
            {
                int newIndex = EditorGUI.Popup(new Rect(position.x, position.y, position.width * 0.5f - 1, position.height),
                    _itemIndices[index], VirtualItemsEditUtil.DisplayedItemIDs);
                if (newIndex != _itemIndices[index])
                {
                    VirtualItemsEditUtil.UpdatePackElementItemByIndex(packElement, newIndex);
                }
                _itemIndices[index] = newIndex;
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

        private VirtualItemPack _currentDisplayedPack;
        private ReorderableListControl _listControl;
        private GenericClassListAdaptor<PackElement> _listAdaptor;
        private List<int> _itemIndices;
        private Vector2 _scrollPosition;
    }
}