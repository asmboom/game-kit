using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System;

namespace Codeplay
{
    public class WorldTreeExplorer : ItemTreeExplorer
    {
        public WorldTreeExplorer()
        {
            _worldToExpanded = new Dictionary<World, bool>();
            InitWorldToExpanded(GameKit.Config.RootWorld);
        }

        public void AddWorld(World world)
        {
            if (!_worldToExpanded.ContainsKey(world))
            {
                _worldToExpanded.Add(world, false);
            }
        }

        public void RemoveWorld(World world)
        {
            if (_worldToExpanded.ContainsKey(world))
            {
                _worldToExpanded.Remove(world);
            }
        }

        private void InitWorldToExpanded(World world)
        {
            _worldToExpanded.Add(world, true);
			foreach (var subWorldID in world.SubWorldsID)
            {
				InitWorldToExpanded(GameKit.Config.GetWorldByID(subWorldID));
            }
        }

        private void ExpandWorld(World world, bool resursive = false)
        {
            if (_worldToExpanded.ContainsKey(world))
            {
                _worldToExpanded[world] = true;
                if (resursive)
                {
					foreach (var subWorldID in world.SubWorldsID)
                    {
						ExpandWorld(GameKit.Config.GetWorldByID(subWorldID), true);
                    }
                }
            }
        }

        private void CollapseWorld(World world, bool resursive = false)
        {
            if (_worldToExpanded.ContainsKey(world))
            {
                _worldToExpanded[world] = false;
                if (resursive)
                {
					foreach (var subWorldID in world.SubWorldsID)
                    {
						ExpandWorld(GameKit.Config.GetWorldByID(subWorldID), false);
                    }
                }
            }
        }

        private float DrawWorld(Rect position, World world)
        {
            GUILayout.BeginArea(position);
            if (world == null)
            {
                GUILayout.Label("NULL");
                GUILayout.EndArea();
                return 20;
            }

            float x = 0;
            float y = 0;
            if (_worldToExpanded.ContainsKey(world))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                if (GUILayout.Button(" " + world.ID, GetItemLeftStyle(world),
                        GUILayout.Width(position.width - 15f), GUILayout.Height(20)))
                {
                    SelectItem(world);
                }
                GUILayout.EndHorizontal();

                x += 20;
                y += 20;

				if (world.SubWorldsID.Count > 0)
                {
                    _worldToExpanded[world] = EditorGUILayout.Foldout(_worldToExpanded[world],
                        new GUIContent(string.Empty, Resources.Load("WorldIcon") as Texture), GameKitEditorDrawUtil.FoldoutStyle);

                    y += 20;

                    if (_worldToExpanded[world])
                    {
						foreach (var subWorldID in world.SubWorldsID)
                        {
                            y += DrawWorld(new Rect(x, y,
								position.width - 20, position.height), GameKit.Config.GetWorldByID(subWorldID));
                        }
                    }
                }
                y += 5;
            }
            GUILayout.EndArea();
            return y;
        }

        protected override void DoOnSelectItem(IItem item)
        {
            World w = (item as World).Parent;
            while (w != null)
            {
                if (_worldToExpanded.ContainsKey(w))
                {
                    _worldToExpanded[w] = true;
                }
                else
                {
                    break;
                }
                w = w.Parent;
            }
        }

        protected override void DoExpandAll()
        {
            ExpandWorld(GameKit.Config.RootWorld, true);
        }

        protected override void DoCollapseAll()
        {
            CollapseWorld(GameKit.Config.RootWorld, true);
        }

        protected override void DoDraw(Rect position, string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                DrawWorld(position, GameKit.Config.RootWorld);
            }
            else
            {
                foreach (var world in GameKit.Config.Worlds)
                {
                    DrawItemIfMathSearch(searchText, world, position.width);
                }
            }
        }

        private Dictionary<World, bool> _worldToExpanded;
    }
}