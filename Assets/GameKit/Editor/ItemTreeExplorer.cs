using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System;

namespace Beetle23
{
    public abstract class ItemTreeExplorer
    {
        public Action<IItem> OnSelectionChange = delegate { };
        public IItem CurrentSelectedItem { get; protected set; }

        public ItemTreeExplorer()
        {
            _searchText = string.Empty;
        }

        public void SelectItem(IItem item)
        {
            if (item != CurrentSelectedItem)
            {
                CurrentSelectedItem = item;
                DoOnSelectItem(item);
                OnSelectionChange(item);
            }
        }

        public void Draw(Rect position)
        {
            GUILayout.BeginArea(position, string.Empty, "Box");

            if (GUILayout.Button("Check References", GUILayout.Width(185)))
            {
                GameKitConfigEditor.CheckIfAnyInvalidRef(GameKit.Config);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search", GUILayout.Width(position.width * 0.17f));
            _searchText = GUILayout.TextField(_searchText, 50, GUILayout.Width(position.width * 0.7f));
            if (GUILayout.Button("x", GUILayout.Height(15), GUILayout.Width(20)))
            {
                _searchText = string.Empty;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            /*
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+Expand All", GUILayout.Width(90)))
            {
                DoExpandAll();
            }
            if (GUILayout.Button("-Collapse All", GUILayout.Width(90)))
            {
                DoCollapseAll();
            }
            GUILayout.EndHorizontal();
            */

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
            DoDraw(new Rect(0, 0, position.width - 15, position.height - 50), _searchText);
            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        protected GUIStyle GetItemCenterStyle(IItem item)
        {
            return !string.IsNullOrEmpty(item.ID) && item == CurrentSelectedItem ?
                        GameKitEditorDrawUtil.ItemSelectedCenterStyle : GameKitEditorDrawUtil.ItemCenterLabelStyle;
        }

        protected GUIStyle GetItemLeftStyle(IItem item)
        {
            return !string.IsNullOrEmpty(item.ID) && item == CurrentSelectedItem ?
                        GameKitEditorDrawUtil.ItemSelectedLeftStyle : GameKitEditorDrawUtil.ItemLeftLabelStyle;
        }

        protected void DrawItemIfMathSearch(string searchText, IItem item, float width)
        {
            if (item.ID.Contains(searchText) && GUILayout.Button(" " + item.ID, GetItemLeftStyle(item),
                    GUILayout.Height(22), GUILayout.Width(width)))
            {
                SelectItem(item);
            }
        }

        protected abstract void DoOnSelectItem(IItem item);
        protected abstract void DoExpandAll();
        protected abstract void DoCollapseAll();
        protected abstract void DoDraw(Rect position, string search);

        private Vector2 _scrollPosition;
        private string _searchText;
    }
}