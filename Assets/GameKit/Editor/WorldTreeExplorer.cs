using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System;

namespace Beetle23
{
    public class WorldTreeExplorer : ItemTreeExplorer
    {
        public WorldTreeExplorer(GameKitConfig config)
            : base(config)
        {
            _worldToExpanded = new Dictionary<World, bool>();
            InitWorldToExpanded(_config.RootWorld);
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
            foreach (var subworld in world.SubWorlds)
            {
                InitWorldToExpanded(subworld);
            }
        }

        private void ExpandWorld(World world, bool resursive = false)
        {
            if (_worldToExpanded.ContainsKey(world))
            {
                _worldToExpanded[world] = true;
                if (resursive)
                {
                    foreach (var subworld in world.SubWorlds)
                    {
                        ExpandWorld(subworld, true);
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
                    foreach (var subworld in world.SubWorlds)
                    {
                        ExpandWorld(subworld, false);
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

                if (world.SubWorlds.Count > 0)
                {
                    _worldToExpanded[world] = EditorGUILayout.Foldout(_worldToExpanded[world],
                        new GUIContent(string.Empty, Resources.Load("WorldIcon") as Texture), GameKitEditorDrawUtil.FoldoutStyle);

                    y += 20;

                    if (_worldToExpanded[world])
                    {
                        foreach (var subworld in world.SubWorlds)
                        {
                            y += DrawWorld(new Rect(x, y,
                                position.width - 20, position.height), subworld);
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
            ExpandWorld(_config.RootWorld, true);
        }

        protected override void DoCollapseAll()
        {
            CollapseWorld(_config.RootWorld, true);
        }

        protected override void DoDraw(Rect position)
        {
            DrawWorld(position, _config.RootWorld);
        }

        private Dictionary<World, bool> _worldToExpanded;
    }
}