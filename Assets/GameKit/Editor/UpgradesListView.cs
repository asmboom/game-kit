using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
{
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
            _listControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand |
                ReorderableListFlags.ShowIndices | ReorderableListFlags.DisableReordering);
            _listControl.ItemInserted += OnItemInsert;
            _listControl.ItemRemoving += OnItemRemoving;

            if (_currentItem != null && _currentItem.HasUpgrades)
            {
                SelecteUpgradeItem(_currentItem.Upgrades[0]);
            }

            _purchaseListView = new PurchaseInfoListView(_currentSelectedUpgrade);
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
                    SelecteUpgradeItem(_currentItem.Upgrades[0]);
                }
                else
                {
                    SelecteUpgradeItem(null);
                }
            }
        }

        public void Draw(Rect position)
        {
            GUI.BeginGroup(position, string.Empty, "Box");

            float listHeight = _listControl.CalculateListHeight(_listAdaptor);
            bool hasScrollBar = listHeight + 20 > position.height;
            _scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width * 0.3f, position.height), _scrollPosition,
                new Rect(0, 0, position.width * 0.3f - 20, listHeight));

            float xOffset = 0;
            if (_listAdaptor != null)
            {
                _listControl.Draw(new Rect(0, 0,
                    position.width * 0.3f - (hasScrollBar ? 10 : 0), listHeight), _listAdaptor);
            }

            GUI.EndScrollView();

            xOffset += position.width * 0.3f;

            if (_currentSelectedUpgrade != null)
            {
                GUI.Label(new Rect(xOffset, 0, position.width * 0.7f, 20), "Upgrade price", GameKitEditorDrawUtil.TitleStyle);
                _purchaseListView.Draw(new Rect(xOffset, 20, position.width * 0.7f, position.height - 20));
            }

            GUI.EndGroup();
        }

        private void OnItemRemoving(object sender, ItemRemovingEventArgs args)
        {
            if (EditorUtility.DisplayDialog("Confirm to delete",
                    "Confirm to delete upgrade [" + _listAdaptor[args.itemIndex].Name + "]?", "OK", "Cancel"))
            {
                //TODO
                args.Cancel = false;
            }
            else
            {
                args.Cancel = true;
            }
        }

        private void OnItemInsert(object sender, ItemInsertedEventArgs args)
        {
            //TODO
            /*
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
            _listAdaptor[args.itemIndex].RelatedItem = _currentItem;
            EditorUtility.SetDirty(_listAdaptor[args.itemIndex]);
            */
        }

        private UpgradeItem CreateUpgradeItem()
        {
            return new UpgradeItem();
        }

        private UpgradeItem DrawUpgradeItem(Rect position, UpgradeItem item, int index)
        {
            if (item == null) return null;
            if (GUI.Button(position, item.ID, item == _currentSelectedUpgrade ?
                    GameKitEditorDrawUtil.ItemSelectedStyle : GameKitEditorDrawUtil.ItemStyle))
            {
                SelecteUpgradeItem(item);
            }
            return item;
        }

        private void SelecteUpgradeItem(UpgradeItem item)
        {
            if (item != _currentSelectedUpgrade)
            {
                _currentSelectedUpgrade = item;
                _purchaseListView.UpdateDisplayItem(_currentSelectedUpgrade);
            }
        }

        private VirtualItem _currentItem;
        private ReorderableListControl _listControl;
        private UpgradeItemListAdaptor _listAdaptor;
        private List<int> _virtualCurrencyIndicesForPurchase;
        private UpgradeItem _currentSelectedUpgrade;
        private PurchaseInfoListView _purchaseListView;
        private Vector2 _scrollPosition;
        private Vector2 _purchaseScrollPosition;
    }
}
