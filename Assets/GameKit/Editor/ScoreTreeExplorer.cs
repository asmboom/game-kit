using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Beetle23
{
    public class ScoreTreeExplorer : ItemTreeExplorer
    {
        public ScoreTreeExplorer(GameKitConfig config)
            : base(config)
        {
            _worldToExpanded = new Dictionary<World, bool>();
            InitWorldToExpanded(config.RootWorld);
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
            _worldToExpanded.Add(world, false);
            foreach (var subworld in world.SubWorlds)
            {
                InitWorldToExpanded(subworld);
            }
        }

        protected override void DoOnSelectItem(IItem item) 
        {
            World w = _config.FindWorldThatScoreBelongsTo(item as Score);
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
            DrawWorldScores(position, _config.RootWorld);
        }

        private float DrawWorldScores(Rect position, World world)
        {
            GUILayout.BeginArea(position);
            if (world == null)
            {
                GUILayout.Label("NULL World");
                GUILayout.EndArea();
                return 20;
            }

            float x = 0;
            float y = 0;
            if (_worldToExpanded.ContainsKey(world))
            {
                _worldToExpanded[world] = EditorGUILayout.Foldout(_worldToExpanded[world],
                    world.ID, GameKitEditorDrawUtil.FoldoutStyle);
                if (_worldToExpanded[world])
                {
                    var size = GameKitEditorDrawUtil.ItemSelectedLeftStyle.CalcSize(new GUIContent(world.ID));
                    size.x = Mathf.Max(100, size.x);
                    for (int i = 0; i < world.Scores.Count; i++)
                    {
                        Score score = world.Scores[i];
                        if (GUILayout.Button(score.ID,
                                (!string.IsNullOrEmpty(score.ID) && score == CurrentSelectedItem ?
                                    GameKitEditorDrawUtil.ItemSelectedCenterStyle : GameKitEditorDrawUtil.ItemCenterLabelStyle),
                                GUILayout.Width(size.x), GUILayout.Height(20)))
                        {
                            SelectItem(score);
                        }
                    }

                    x += size.x;
                    y += 20;

                    if (world.SubWorlds.Count > 0)
                    {
                        foreach (var subworld in world.SubWorlds)
                        {
                            y += DrawWorldScores(new Rect(x, y,
                                    position.width - 100, position.height), subworld);
                        }
                    }
                }
                else
                {
                    y += 20;
                }
            }

            GUILayout.EndArea();
            return y;
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

        private Dictionary<World, bool> _worldToExpanded;
    }
}
