using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

public class PurchaseInfoListView
{
    public PurchaseInfoListView(PurchasableItem item)
    {
        _listControl = new ReorderableListControl();

        UpdateVirtualCurrencyIndices();
    }

    public void UpdateDisplayItem(PurchasableItem item)
    {
        if (item != null)
        {
            _listAdaptor = new GenericClassListAdaptor<Purchase>(item.PurchaseInfo, 18,
                CreatePurchase, DrawOnePurchase);

            UpdateVirtualCurrencyIndices();
        }
    }

    public void Draw()
    {
        GUILayout.BeginHorizontal();
        VirtualItemsDrawUtil.BeginDrawTitle();
        GUILayout.Label("Purchase Type");
        GUILayout.Label("Associated ID/Item");
        GUILayout.Label("Price");
        VirtualItemsDrawUtil.EndDrawTitle();
        GUILayout.EndHorizontal();

        if (_listAdaptor != null)
        {
            _listControl.Draw(_listAdaptor);
        }
    }

    private Purchase CreatePurchase()
    {
        return new Purchase();
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
        if (newType != purchase.Type && purchase.Type == PurchaseType.PurchaseWithVirtualCurrency)
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
            int newIndex = EditorGUI.Popup(position, _virtualCurrencyIndicesForPurchase[index], VirtualItemsEditUtil.DisplayedVirtualCurrencyIDs);
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

    private void UpdateVirtualCurrencyIndices()
    {
        if (_listAdaptor != null)
        {
            _virtualCurrencyIndicesForPurchase = new List<int>();

            for (var i = 0; i < _listAdaptor.Count; i++)
            {
                _virtualCurrencyIndicesForPurchase.Add(_listAdaptor[i].VirtualCurrency == null ? 0 :
                    VirtualItemsEditUtil.GetVirtualCurrencyIndexById(_listAdaptor[i].VirtualCurrency.ID));
            }
        }
    }

    private ReorderableListControl _listControl;
    private GenericClassListAdaptor<Purchase> _listAdaptor;
    private List<int> _virtualCurrencyIndicesForPurchase;

    private const float PurchaseTypeWidth = 0.33f;
    private const float PurchaseAssociatedWidth = 0.33f;
    private const float PurchasePriceWidth = 0.33f;
}