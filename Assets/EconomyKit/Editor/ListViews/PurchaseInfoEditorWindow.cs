using Rotorz.ReorderableList;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PurchaseInfoEditorWindow : EditorWindow
{
    public void Init(PurchasableItem item)
    {
        _currentEditItem = item;
        _listAdaptor = new GenericClassListAdaptor<Purchase>(_currentEditItem.PurchaseInfo, 22, 
            CreatePurchase, DrawItem);

        UpdateVirtualCurrencyIndices();
    }

    private void OnEnable()
    {
        _listControl = new ReorderableListControl();
        _listControl.ItemInserted += OnItemInsert;
        _listControl.ItemRemoving += OnItemRemoving;
    }

    private void OnDisable()
    {
        if (_listControl != null)
        {
            _listControl.ItemInserted -= OnItemInsert;
            _listControl.ItemRemoving -= OnItemRemoving;
        }
    }

    private void OnItemRemoving(object sender, ItemRemovingEventArgs args)
    {
        UpdateVirtualCurrencyIndices();
    }

    private void OnItemInsert(object sender, ItemInsertedEventArgs args)
    {
        UpdateVirtualCurrencyIndices();
    }

    private void OnGUI()
    {
        if (_currentEditItem == null || _listAdaptor == null) return;

        var centeredStyle = GUI.skin.GetStyle("Label");

        centeredStyle.richText = true;
        GUILayout.Label("<size=15><color=lightblue>" + _currentEditItem.Name + "</color> purchase info</size>");
        GUILayout.Space(5);

        var oldAlignment = centeredStyle.alignment;
        var oldFontSize = centeredStyle.fontSize;
        centeredStyle.alignment = TextAnchor.UpperCenter;
        centeredStyle.fontSize = 12;
        centeredStyle.fontStyle = FontStyle.Bold;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Purchase Type", GUILayout.Width(this.position.width * 0.4f));
        GUILayout.Label("Associated ID/Item", GUILayout.Width(this.position.width * 0.25f));
        GUILayout.Label("Price", GUILayout.Width(this.position.width * 0.25f));
        GUILayout.EndHorizontal();

        centeredStyle.alignment = oldAlignment;
        centeredStyle.fontSize = oldFontSize;
        centeredStyle.fontStyle = FontStyle.Normal;
        centeredStyle.richText = false;

        _listControl.Draw(_listAdaptor);
    }

    public Purchase CreatePurchase()
    {
        return new Purchase();
    }

    public Purchase DrawItem(Rect position, Purchase purchase, int index)
    {
        if (purchase == null) return null;

        float xOffset = position.x;
        DrawType(new Rect(xOffset, position.y, position.width * TypeWidth - 1, position.height), 
            purchase, index);
        xOffset += position.width * TypeWidth;
        if (purchase.Type == PurchaseType.PurchaseWithMarket)
        {
            DrawMarketID(new Rect(xOffset, position.y, position.width * AssociatedWidth - 1, position.height), 
                purchase);
        }
        else
        {
            DrawVirtualCurrencyPopup(new Rect(xOffset, position.y, position.width * AssociatedWidth - 1, position.height), 
                purchase, index);
        }
        xOffset += position.width * AssociatedWidth;
        DrawPrice(new Rect(xOffset, position.y, position.width * PriceWidth - 1, position.height), purchase);
        return purchase;
    }

    public void UpdateVirtualCurrencyIndices()
    {
        if (_listAdaptor != null)
        {
            _virtualCurrencyIndices = new List<int>();

            for (var i = 0; i < _listAdaptor.Count; i++)
            {
                _virtualCurrencyIndices.Add(VirtualItemsEditUtil.GetVirtualCurrencyIndexById(_listAdaptor[i].AssociatedID));
            }
        }
    }

    private void DrawType(Rect position, Purchase purchase, int index)
    {
        GUI.changed = false;
        purchase.Type = (PurchaseType)EditorGUI.EnumPopup(position, purchase.Type);
        if (GUI.changed && purchase.Type == PurchaseType.PurchaseWithVirtualCurrency)
        {
            VirtualItemsEditUtil.UpdatePurchaseByIndex(purchase, _virtualCurrencyIndices[index]);
        }
    }

    private void DrawMarketID(Rect position, Purchase purchase)
    {
        purchase.AssociatedID = EditorGUI.TextField(position, purchase.AssociatedID);
    }

    private void DrawVirtualCurrencyPopup(Rect position, Purchase purchase, int index)
    {
        GUI.changed = false;
        _virtualCurrencyIndices[index] = 
                EditorGUI.Popup(position, _virtualCurrencyIndices[index], VirtualItemsEditUtil.DisplayedVirtualCurrencyIDs);
        if (GUI.changed == true)
        {
            VirtualItemsEditUtil.UpdatePurchaseByIndex(purchase, _virtualCurrencyIndices[index]);
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

    private PurchasableItem _currentEditItem;
    private ReorderableListControl _listControl;
    private GenericClassListAdaptor<Purchase> _listAdaptor;
    private List<int> _virtualCurrencyIndices;

    private const float TypeWidth = 0.4f;
    private const float AssociatedWidth = 0.3f;
    private const float PriceWidth = 0.3f;
}