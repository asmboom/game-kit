using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System;

namespace Beetle23
{
    public class WorldTreeExplorer
    {
        public Action<World> OnSelectionChange = delegate { };
        public World CurrentSelectedWorld { get; private set; }

        public WorldTreeExplorer(GameKitConfig config)
        {
            _config = config;
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

        public void SelectWorld(World world)
        {
            if (world != CurrentSelectedWorld)
            {
                CurrentSelectedWorld = world;
                World w = CurrentSelectedWorld.Parent;
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
                OnSelectionChange(world);
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
                ExpandWorld(_config.RootWorld, true);
            }
            if (GUILayout.Button("-Collapse All", GUILayout.Width(90)))
            {
                CollapseWorld(_config.RootWorld, true);
            }
            GUILayout.EndHorizontal();

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            DrawWorld(new Rect(0, 0, position.width, position.height - 50), _config.RootWorld);
            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        private void InitWorldToExpanded(World world)
        {
            _worldToExpanded.Add(world, false);
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
                var size = GameKitEditorDrawUtil.ItemSelectedLeftStyle.CalcSize(new GUIContent(world.ID));
                size.x = Mathf.Max(100, size.x);
                if (GUILayout.Button(world.ID,
                        (!string.IsNullOrEmpty(world.ID) && world == CurrentSelectedWorld ?
                            GameKitEditorDrawUtil.ItemSelectedCenterStyle : GameKitEditorDrawUtil.ItemCenterLabelStyle),
                        GUILayout.Width(size.x), GUILayout.Height(20)))
                {
                    SelectWorld(world);
                }

                x += size.x;
                y += 20;

                if (world.SubWorlds.Count > 0)
                {
                    _worldToExpanded[world] = EditorGUILayout.Foldout(_worldToExpanded[world],
                        "child worlds", GameKitEditorDrawUtil.FoldoutStyle);

                    GUILayout.EndHorizontal();

                    if (_worldToExpanded[world])
                    {
                        foreach (var subworld in world.SubWorlds)
                        {
                            y += DrawWorld(new Rect(x, y,
                                position.width - 100, position.height), subworld);
                        }
                    }
                }
                else
                {
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndArea();
            return y;
        }

        private GameKitConfig _config;
        private Vector2 _scrollPosition;
        private Dictionary<World, bool> _worldToExpanded;
    }
}