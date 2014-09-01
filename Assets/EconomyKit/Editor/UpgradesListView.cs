using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

public class UpgradesListView
{
    private class UpgradeItemListAdaptor : GenericClassListAdaptor<UpgradeItem>
    {
        public UpgradeItemListAdaptor(List<UpgradeItem> list, float itemHeight,
            GenericListAdaptorDelegate.ItemCreator<UpgradeItem> itemCreator,
            GenericListAdaptorDelegate.ClassItemDrawer<UpgradeItem> itemDrawer)
            : base(list, itemHeight, itemCreator, itemDrawer, null)
        {
        }

        public override bool CanDrag(int index)
        {
            return false;
        }

        public override bool CanRemove(int index)
        {
            return index == Count - 1;
        }
    }

    public UpgradesListView(VirtualItem item)
    {
        _currentItem = item;
        _listControl = new ReorderableListControl();
        _listControl.ItemInserted += OnItemInsert;
        _listControl.ItemRemoving += OnItemRemoving;

        if (_currentItem != null && _currentItem.HasUpgrades)
        {
            SelectItem(_currentItem.Upgrades[0]);
        }

        _purchaseListView = new PurchaseInfoListView(_currentSelectedItem);
    }

    public void UpdateDisplayItem(VirtualItem item)
    {
        _currentItem = item;
        if (item != null)
        {
            _listAdaptor = new UpgradeItemListAdaptor(item.Upgrades, 18,
                CreateUpgradeItem, DrawUpgradeItem);

            if (_currentItem.HasUpgrades)
            {
                SelectItem(_currentItem.Upgrades[0]);
            }
            else
            {
                SelectItem(null);
            }
        }
    }

    public void Draw()
    {
        if (Event.current.type == EventType.Repaint)
        {
            _upgradeInfoRect = GUILayoutUtility.GetLastRect();
            if (!_rectInitialized)
            {
                _rectInitialized = true;
                return;
            }
        }

        if (GUI.changed == false && _rectInitialized && _listAdaptor != null)
        {
            GUILayout.BeginArea(new Rect(0, _upgradeInfoRect.y + _upgradeInfoRect.height,
                250, 200), string.Empty, "Box");
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            _listControl.Draw(_listAdaptor);

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            if (_currentSelectedItem != null)
            {
                GUILayout.BeginArea(new Rect(270, _upgradeInfoRect.y + _upgradeInfoRect.height,
                    500, 200), string.Empty, "Box");
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

                GUILayout.Label("Upgrade price");
                EditorGUI.BeginChangeCheck();
                _purchaseListView.Draw();
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(_currentSelectedItem);   
                }

                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
        }
    }

    private void OnItemRemoving(object sender, ItemRemovingEventArgs args)
    {
        if (EditorUtility.DisplayDialog("Confirm to delete",
                "Confirm to delete upgrade [" + _listAdaptor[args.itemIndex].Name + "]?", "OK", "Cancel"))
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
        string prefix = (args.itemIndex + 1) < 10 ? "0" + (args.itemIndex + 1) : (args.itemIndex + 1).ToString();
        _listAdaptor[args.itemIndex].ID = string.Format("{0}Upgrade0{1}", _currentItem.ID, prefix);
        string oldAssetFileName = AssetDatabase.GetAssetPath(_listAdaptor[args.itemIndex]);
        string directoryName = System.IO.Path.GetDirectoryName(oldAssetFileName);
        string newAssetFileName = directoryName + "/" + _listAdaptor[args.itemIndex].ID + ".asset";
        if (!AssetDatabase.GenerateUniqueAssetPath(newAssetFileName).Equals(newAssetFileName))
        {
            Debug.LogWarning("Upgrade item with same name [" + newAssetFileName + "] already exists, deleted old one");
            AssetDatabase.DeleteAsset(newAssetFileName);
        }
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_listAdaptor[args.itemIndex]), _listAdaptor[args.itemIndex].ID);
        _listAdaptor[args.itemIndex].Name = string.Format("Upgrade {0} to level {1}", _currentItem.Name, args.itemIndex + 2);
        _listAdaptor[args.itemIndex].Description = _listAdaptor[args.itemIndex].Name;
        EditorUtility.SetDirty(_listAdaptor[args.itemIndex]);
    }

    private UpgradeItem CreateUpgradeItem()
    {
        return VirtualItemsEditUtil.CreateNewVirtualItem<UpgradeItem>();
    }

    private UpgradeItem DrawUpgradeItem(Rect position, UpgradeItem item, int index)
    {
        if (item == null) return null;

        GUIStyle labelStyle = GUI.skin.GetStyle("Label");
        labelStyle.richText = true;
        string caption = item.ID;
        if (item == _currentSelectedItem)
        {
            caption = "<color=red>" + caption + "</color>";
        }
        if (GUI.Button(position, caption, "Label"))
        {
            SelectItem(item);
        }
        labelStyle.richText = false;

        return item;
    }

    private void SelectItem(UpgradeItem item)
    {
        if (item != _currentSelectedItem)
        {
            _currentSelectedItem = item;
            _purchaseListView.UpdateDisplayItem(_currentSelectedItem);
        }
    }

    private VirtualItem _currentItem;
    private ReorderableListControl _listControl;
    private UpgradeItemListAdaptor _listAdaptor;
    private List<int> _virtualCurrencyIndicesForPurchase;
    private UpgradeItem _currentSelectedItem;
    private Rect _upgradeInfoRect;
    private bool _rectInitialized = false;

    private PurchaseInfoListView _purchaseListView;

    private const float PurchaseTypeWidth = 0.33f;
    private const float PurchaseAssociatedWidth = 0.33f;
    private const float PurchasePriceWidth = 0.33f;
    private Vector2 _scrollPosition;
}
