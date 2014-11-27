using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
{
    public class VirtualItemsPropertyInspector : ItemPropertyInspector
    {
        public VirtualItemsPropertyInspector(VirtualItemsTreeExplorer treeExplorer)
            :base(treeExplorer)
        {
            _purchaseListView = new PurchaseInfoListView(_currentDisplayItem as PurchasableItem);
            _packListView = new PackInfoListView(_currentDisplayItem as VirtualItemPack);
            _upgradesListView = new UpgradesListView(_currentDisplayItem as VirtualItem);
            _categoryPropertyView = new CategoryPropertyView(_currentDisplayItem as VirtualCategory);
        }

        protected override void DoOnExplorerSelectionChange(IItem item)
        {
            if (item is VirtualItem)
            {
                GUI.FocusControl(string.Empty);

                if (item is SingleUseItem || item is LifeTimeItem)
                {
                    _upgradesListView.UpdateDisplayItem(item as VirtualItem);
                }
                if (item is PurchasableItem)
                {
                    if (item is VirtualItemPack)
                    {
                        _packListView.UpdateDisplayItem(item as VirtualItemPack);
                    }
                    _purchaseListView.UpdateDisplayItem(item as PurchasableItem);
                }
            }
            else if (item is VirtualCategory)
            {
                _categoryPropertyView.UpdateDisplayItem(item as VirtualCategory);
            }
        }

        protected override float DoDrawItem(Rect position, IItem item)
        {
            VirtualItem virtualItem = item as VirtualItem;
            if (virtualItem != null)
            {
                return DrawVirtualItem(position, virtualItem);
            }
            else
            {
                VirtualCategory category = item as VirtualCategory;
                if (category != null)
                {
                    GUILayout.BeginArea(new Rect(position.x, position.y + 5, position.width, position.height - 10));
                    _categoryPropertyView.Draw(position, category);
                    GUILayout.EndArea();
                    return position.height;
                }
            }
            return 0;
        }

        protected override IItem GetItemFromConfig(string id)
        {
            return GameKit.Config.GetVirtualItemByID(id);
        }

        private float DrawVirtualItem(Rect position, VirtualItem item)
        {
            float yOffset = 0;
            float width = position.width;

            _isVirtualItemPropertiesExpanded = EditorGUI.Foldout(new Rect(0, 0, width, 20),
                _isVirtualItemPropertiesExpanded, "Basic Property");
            yOffset += 20;
            if (_isVirtualItemPropertiesExpanded)
            {
                DrawVirtualItemID(new Rect(0, yOffset, width, 20), item);
                yOffset += 20;
                item.Name = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Name", item.Name);
                yOffset += 20;
                item.Description = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Desription", item.Description);
                yOffset += 20;
                EditorGUI.LabelField(new Rect(0, yOffset, width, 20), "Category", item.Category == null ? "None" : item.Category.ID);
                yOffset += 20;
                item.Icon = EditorGUI.ObjectField(new Rect(0, yOffset, width, 20), "Icon", item.Icon, typeof(Sprite), false) as Sprite;
                yOffset += 20;
                item.Extend = EditorGUI.ObjectField(new Rect(0, yOffset, width, 20), "Extend",
                    item.Extend, typeof(ScriptableObject), false) as ScriptableObject;
                yOffset += 20;
            }

            if (item is LifeTimeItem)
            {
                yOffset += 20;
                LifeTimeItem lifetimeItem = item as LifeTimeItem;
                lifetimeItem.IsEquippable = EditorGUI.Toggle(new Rect(0, yOffset, width, 20), "Is Equippable", lifetimeItem.IsEquippable);
                yOffset += 20;
            }
            else if (item is VirtualItemPack)
            {
                yOffset += 20;
                _isPackInfoExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20), _isPackInfoExpanded, "Pack Info");
                yOffset += 20;
                if (_isPackInfoExpanded)
                {
                    _packListView.Draw(new Rect(0, yOffset, width, 100));
                    yOffset += 100;
                }
            }
            if (item is PurchasableItem)
            {
                yOffset += 20;
                _isPurchaseInfoExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20), _isPurchaseInfoExpanded, "Purchase Info");
                yOffset += 20;
                if (_isPurchaseInfoExpanded)
                {
                    _purchaseListView.Draw(new Rect(0, yOffset, width, 100));
                    yOffset += 100;
                }
            }
            if (item is SingleUseItem || item is LifeTimeItem)
            {
                yOffset += 20;
                _isUpgradeInfoExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20),
                    _isUpgradeInfoExpanded, "Upgrade Info (" + item.Upgrades.Count + " levels)");
                yOffset += 20;
                if (_isUpgradeInfoExpanded)
                {
                    _upgradesListView.Draw(new Rect(0, yOffset, width, 200));
                    yOffset += 200;
                }
            }
            return yOffset;
        }

        private void DrawVirtualItemID(Rect position, VirtualItem item)
        {
            DrawIDTextField(position, item);
        }

        private bool _isVirtualItemPropertiesExpanded = true;
        private bool _isPackInfoExpanded = true;
        private bool _isPurchaseInfoExpanded = true;
        private bool _isUpgradeInfoExpanded = false;

        private PurchaseInfoListView _purchaseListView;
        private PackInfoListView _packListView;
        private UpgradesListView _upgradesListView;
        private CategoryPropertyView _categoryPropertyView;
    }
}
