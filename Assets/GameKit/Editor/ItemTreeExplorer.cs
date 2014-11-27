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

        public ItemTreeExplorer(GameKitConfig config)
        {
            _config = config;
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
                GameKitConfigEditor.CheckIfAnyInvalidRef(_config);
            }

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

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            DoDraw(new Rect(0, 0, position.width, position.height - 50));
            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        protected abstract void DoOnSelectItem(IItem item);
        protected abstract void DoExpandAll();
        protected abstract void DoCollapseAll();
        protected abstract void DoDraw(Rect position);

        protected GameKitConfig _config;
        private Vector2 _scrollPosition;
    }
}