using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
{
    public class WorldPropertyInspector
    {
        public WorldPropertyInspector(World currentDisplayWorld)
        {
            _currentDisplayWorld = currentDisplayWorld;
            /*
            _purchaseListView = new PurchaseInfoListView(currentDisplayedItem as PurchasableItem);
            _packListView = new PackInfoListView(currentDisplayedItem as VirtualItemPack);
            _upgradesListView = new UpgradesListView(currentDisplayedItem as VirtualItem);
            _categoryPropertyView = new CategoryPropertyView(currentDisplayedItem as VirtualCategory);
            */
        }

        public void OnExplorerSelectionChange(World world)
        {
            _currentDisplayWorld = world;

            /*
            if (world is VirtualItem)
            {
                GUI.FocusControl(string.Empty);

                if (world is SingleUseItem || world is LifeTimeItem)
                {
                    _upgradesListView.UpdateDisplayItem(world as VirtualItem);
                }
                if (world is PurchasableItem)
                {
                    if (world is VirtualItemPack)
                    {
                        _packListView.UpdateDisplayItem(world as VirtualItemPack);
                    }
                    _purchaseListView.UpdateDisplayItem(world as PurchasableItem);
                }
            }
            else if (world is VirtualCategory)
            {
                _categoryPropertyView.UpdateDisplayItem(world as VirtualCategory);
            }
            */
        }

        public void Draw(Rect position)
        {
            GUI.BeginGroup(position, string.Empty, "Box");
            if (_currentDisplayWorld != null)
            {
                DrawWorld(new Rect(10, 0, position.width - 10, position.height), _currentDisplayWorld);
            }
            GUI.EndGroup();
        }

        private void DrawWorld(Rect position, World world)
        {
            GUI.BeginGroup(position);
            _scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height),
                _scrollPosition, new Rect(0, 0, position.width - 20, _currentYOffset));

            float yOffset = 0;
            bool showScrollbar = position.height < _currentYOffset;
            float width = position.width - (showScrollbar ? 20 : 10);
            _isBasicPropertiesExpanded = EditorGUI.Foldout(new Rect(0, 0, width, 20),
                _isBasicPropertiesExpanded, "Basic Property");
            yOffset += 20;
            if (_isBasicPropertiesExpanded)
            {
                DrawID(new Rect(0, yOffset, width, 20), world);
                yOffset += 20;
                world.Name = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Name", world.Name);
                yOffset += 20;
                world.Description = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Desription", world.Description);
                yOffset += 20;
                world.Extend = EditorGUI.ObjectField(new Rect(0, yOffset, width, 20), "Extend",
                    world.Extend, typeof(ScriptableObject), false) as ScriptableObject;
                yOffset += 20;
            }

            _currentYOffset = yOffset;

            GUI.EndScrollView();
            GUI.EndGroup();
        }

        private void DrawID(Rect position, World world)
        {
            EditorGUI.LabelField(position, "ID", world.ID);
        }

        private bool _isBasicPropertiesExpanded = true;
        private bool _isPackInfoExpanded = true;
        private bool _isPurchaseInfoExpanded = true;
        private bool _isUpgradeInfoExpanded = false;

        private World _currentDisplayWorld;

        //private PurchaseInfoListView _purchaseListView;
        //private PackInfoListView _packListView;
        //private UpgradesListView _upgradesListView;
        //private CategoryPropertyView _categoryPropertyView;

        private Vector2 _scrollPosition;
        private float _currentYOffset;
    }
}