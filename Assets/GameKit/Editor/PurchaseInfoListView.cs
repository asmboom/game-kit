using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
{
    public class PurchaseInfoListView
    {
        public PurchaseInfoListView(PurchasableItem item)
        {
            _listControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand |
                ReorderableListFlags.ShowIndices);
            _listControl.ItemInserted += OnItemInsert;
            _listControl.ItemRemoving += OnItemRemoving;

            UpdateVirtualCurrencyIndices(true);
        }

        public void UpdateDisplayItem(PurchasableItem item)
        {
            _currentPurchasableItem = item;
            if (item != null)
            {
                _listAdaptor = new GenericClassListAdaptor<Purchase>(item.PurchaseInfo, 18,
                    CreatePurchase, DrawOnePurchase);
            }

            UpdateVirtualCurrencyIndices(true);
        }

        public void Draw(Rect position)
        {
            GUI.BeginGroup(position, string.Empty, "Box");
            float listHeight = _listControl.CalculateListHeight(_listAdaptor);
            bool hasScrollBar = listHeight + 20 > position.height;
            _scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), _scrollPosition,
                new Rect(0, 0, position.width - 20, listHeight + 20));

            float xOffset = 0;
            GUI.Label(new Rect(0, 0, position.width * PurchaseTypeWidth, 20),
                "Purchase Type", GameKitEditorDrawUtil.TitleStyle);
            xOffset += position.width * PurchaseTypeWidth;
            GUI.Label(new Rect(xOffset, 0, position.width * PurchaseAssociatedWidth, 20),
                "Associated ID/Item", GameKitEditorDrawUtil.TitleStyle);
            xOffset += position.width * PurchaseAssociatedWidth;
            GUI.Label(new Rect(xOffset, 0, position.width * PurchasePriceWidth, 20),
                "Price", GameKitEditorDrawUtil.TitleStyle);

            if (_listAdaptor != null)
            {
                _listControl.Draw(new Rect(0, 20,
                    position.width - (hasScrollBar ? 10 : 0), listHeight), _listAdaptor);
            }

            GUI.EndScrollView();
            GUI.EndGroup();
        }

        private Purchase CreatePurchase()
        {
            return new Purchase();
        }

        private void OnItemRemoving(object sender, ItemRemovingEventArgs args)
        {
            UpdateVirtualCurrencyIndices(true);
        }

        private void OnItemInsert(object sender, ItemInsertedEventArgs args)
        {
            UpdateVirtualCurrencyIndices(false);
        }

        private Purchase DrawOnePurchase(Rect position, Purchase purchase, int index)
        {
            if (purchase == null) return null;

            float xOffset = position.x;
            DrawType(new Rect(xOffset, position.y, position.width * PurchaseTypeWidth - 1, position.height),
                purchase, index);
            xOffset += position.width * PurchaseTypeWidth;
            if (purchase.Type == PurchaseType.PurchaseWithMarket)
            {
                DrawMarketID(new Rect(xOffset, position.y, position.width * PurchaseAssociatedWidth - 1, position.height),
                    purchase);
            }
            else
            {
                DrawVirtualCurrencyPopup(new Rect(xOffset, position.y, position.width * PurchaseAssociatedWidth - 1, position.height),
                    purchase, index);
            }
            xOffset += position.width * PurchaseAssociatedWidth;
            DrawPrice(new Rect(xOffset, position.y, position.width * PurchasePriceWidth - 1, position.height), purchase);

            return purchase;
        }

        private void DrawType(Rect position, Purchase purchase, int index)
        {
            PurchaseType newType = (PurchaseType)EditorGUI.EnumPopup(position, purchase.Type);
            if (newType != purchase.Type && newType == PurchaseType.PurchaseWithVirtualCurrency)
            {
                VirtualItemsEditUtil.UpdatePurchaseByIndex(purchase, _virtualCurrencyIndicesForPurchase[index]);
            }
            purchase.Type = newType;
        }

        private void DrawMarketID(Rect position, Purchase purchase)
        {
            purchase.MarketID = EditorGUI.TextField(position, purchase.MarketID);
        }

        private void DrawVirtualCurrencyPopup(Rect position, Purchase purchase, int index)
        {
            if (index < _virtualCurrencyIndicesForPurchase.Count)
            {
                int newIndex = EditorGUI.Popup(position,
                    _virtualCurrencyIndicesForPurchase[index], VirtualItemsEditUtil.DisplayedVirtualCurrencyIDs);
                if (newIndex != _virtualCurrencyIndicesForPurchase[index])
                {
                    VirtualItemsEditUtil.UpdatePurchaseByIndex(purchase, newIndex);
                }
                _virtualCurrencyIndicesForPurchase[index] = newIndex;
            }
        }

        private void DrawPrice(Rect position, Purchase purchase)
        {
            if (purchase.IsMarketPurchase)
            {
                purchase.Price = EditorGUI.FloatField(position, purchase.Price);
            }
            else
            {
                purchase.Price = EditorGUI.IntField(position, (int)purchase.Price);
            }
        }

        private void UpdateVirtualCurrencyIndices(bool needWarning)
        {
            if (_listAdaptor != null)
            {
                _virtualCurrencyIndicesForPurchase = new List<int>();

                for (var i = 0; i < _listAdaptor.Count; i++)
                {
                    if (_listAdaptor[i].VirtualCurrency == null && VirtualItemsEditUtil.DisplayedVirtualCurrencyIDs.Length > 0)
                    {
                        VirtualItem item = GameKit.Config.GetVirtualItemByID(VirtualItemsEditUtil.DisplayedVirtualCurrencyIDs[0]);
                        if (item != null && needWarning)
                        {
                            Debug.LogWarning("[" + _currentPurchasableItem.ID + "]'s purchase [" + (i + 1) + 
                                "] is null, correct it with default virtual currency [" + item.ID + "].");
                        }
                        _listAdaptor[i].VirtualCurrencyID = item.ID;
                    }
                    _virtualCurrencyIndicesForPurchase.Add(_listAdaptor[i].VirtualCurrency == null ? 0 :
                        VirtualItemsEditUtil.GetVirtualCurrencyIndexById(_listAdaptor[i].VirtualCurrency.ID));
                }
            }
        }

        private ReorderableListControl _listControl;
        private GenericClassListAdaptor<Purchase> _listAdaptor;
        private List<int> _virtualCurrencyIndicesForPurchase;
        private PurchasableItem _currentPurchasableItem;

        private const float PurchaseTypeWidth = 0.4f;
        private const float PurchaseAssociatedWidth = 0.4f;
        private const float PurchasePriceWidth = 0.2f;
        private Vector2 _scrollPosition;
    }
}
