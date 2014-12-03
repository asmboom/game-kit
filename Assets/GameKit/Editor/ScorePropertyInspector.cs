using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
{
    public class ScorePropertyInspector : ItemPropertyInspector
    {
        public ScorePropertyInspector(ScoreTreeExplorer treeExplorer)
            : base(treeExplorer)
        {
        }

        protected override void DoOnExplorerSelectionChange(IItem item)
        {
            _currentWorldOfScore = GameKit.Config.FindWorldThatScoreBelongsTo(item as Score);
        }

        protected override float DoDrawItem(Rect rect, IItem item)
        {
            float yOffset = 0;
            float width = rect.width;
            Score score = item as Score;
            _isBasicPropertiesExpanded = EditorGUI.Foldout(new Rect(0, 0, width, 20),
                _isBasicPropertiesExpanded, "Basic Property");
            yOffset += 20;
            if (_isBasicPropertiesExpanded)
            {
                DrawIDTextField(new Rect(0, yOffset, width, 20), score);
                yOffset += 20;
                score.Name = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Name", score.Name);
                yOffset += 20;
            }

            yOffset += 20;
            EditorGUI.LabelField(new Rect(0, yOffset, 250, 20), "Belong to World", 
                _currentWorldOfScore == null ? "NULL" : _currentWorldOfScore.ID);
            if (_currentWorldOfScore != null)
            {
                if (GUI.Button(new Rect(255, yOffset, 50, 20), "Go"))
                {
                    WorldTreeExplorer worldTreeExplorer = (GameKitEditorWindow.GetInstance().GetTreeExplorer(
                        GameKitEditorWindow.TabType.Worlds) as WorldTreeExplorer);
                    GameKitEditorWindow.GetInstance().SelectTab(GameKitEditorWindow.TabType.Worlds);
                    worldTreeExplorer.SelectItem(_currentWorldOfScore);
                }
            }
            yOffset += 20;

            return yOffset;
        }

        protected override IItem GetItemFromConfig(string id)
        {
            return GameKit.Config.GetWorldByID(id);
        }

        private bool _isBasicPropertiesExpanded = true;
        private World _currentWorldOfScore;
    }
}