using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Codeplay
{
    public class ScorePropertyInspector : ItemPropertyInspector
    {
        public ScorePropertyInspector(ScoreTreeExplorer treeExplorer)
            : base(treeExplorer)
        {
        }

        public override IItem[] GetAffectedItems(string itemID)
        {
            List<IItem> items = new List<IItem>();
            foreach (var world in GameKit.Config.Worlds)
            {
                if (world.Gate.IsGroup)
                {
					foreach (var subGateID in world.Gate.SubGatesID)
                    {
						Gate subGate = GameKit.Config.GetSubGateByID(subGateID);
						if (subGate.Type == GateType.ScoreGate && 
							subGate.RelatedItemID.Equals(itemID))
                        {
							items.Add(subGate);
                        }
                    }
                }
                else if (world.Gate.Type == GateType.ScoreGate &&
                         world.Gate.RelatedItemID.Equals(itemID))
                {
                    items.Add(world.Gate);
                }
            }
            return items.ToArray();
        }

        protected override void DoOnExplorerSelectionChange(IItem item)
        {
            if (item == null)
            {
                _currentWorldOfScore = null;
                _relatedVirtualItemIDProperty = null;
            }
            else
            {
                _currentWorldOfScore = GameKit.Config.FindWorldThatScoreBelongsTo(item as Score);

                string path = GameKitEditorWindow.GetInstance().FindScorePropertyPath(item as Score);
                SerializedProperty scoreProperty = GameKitEditorWindow.SerializedConfig.FindProperty(path);
                if (scoreProperty != null)
                {
                    _relatedVirtualItemIDProperty = scoreProperty.FindPropertyRelative("RelatedVirtualItemID");
                }
                else
                {
                    _relatedVirtualItemIDProperty = null;
                    Debug.LogError("Couldn't find score property from config, maybe the change is not applied yet.");
                }
            }
        }

        protected override float DoDrawItem(Rect rect, IItem item)
        {
            float yOffset = 0;
            float width = rect.width;
            Score score = item as Score;

            _isBasicPropertiesExpanded = EditorGUI.Foldout(new Rect(0, 0, width, 20),
                _isBasicPropertiesExpanded, "Item");
            yOffset += 20;
            if (_isBasicPropertiesExpanded)
            {
                DrawIDField(new Rect(0, yOffset, width, 20), score, true, true);
                yOffset += 20;
                score.Name = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Name", score.Name);
                yOffset += 20;
            }
            yOffset += 20;

            EditorGUI.LabelField(new Rect(0, yOffset, 250, 20), "Belong to World", 
                _currentWorldOfScore == null ? "NULL" : _currentWorldOfScore.ID);
            if (_currentWorldOfScore != null)
            {
                if (GUI.Button(new Rect(255, yOffset, 50, 20), "Edit"))
                {
                    WorldTreeExplorer worldTreeExplorer = (GameKitEditorWindow.GetInstance().GetTreeExplorer(
                        GameKitEditorWindow.TabType.Worlds) as WorldTreeExplorer);
                    GameKitEditorWindow.GetInstance().SelectTab(GameKitEditorWindow.TabType.Worlds);
                    worldTreeExplorer.SelectItem(_currentWorldOfScore);
                }
                yOffset += 20;
            }
            yOffset += 20;

            _isScorePropertiesExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20),
                _isScorePropertiesExpanded, "Score Property");
            yOffset += 20;
            if (_isScorePropertiesExpanded)
            {
                score.EnableClamp = EditorGUI.Toggle(new Rect(0, yOffset, width, 20),
                    "Clamp Score", score.EnableClamp);
                yOffset += 20;
                if (score.EnableClamp)
                {
                    score.Min = EditorGUI.FloatField(new Rect(10, yOffset, width, 20),
                        "Mininum Value", score.Min);
                    yOffset += 20;
                    score.Max = EditorGUI.FloatField(new Rect(10, yOffset, width, 20),
                        "Maximum Value", score.Max);
                    yOffset += 20;
                    score.DefaultValue = EditorGUI.Slider(new Rect(0, yOffset, width, 20),
                        "Default Score", score.DefaultValue, score.Min, score.Max);
                }
                else
                {
                    score.DefaultValue = EditorGUI.FloatField(new Rect(0, yOffset, width, 20),
                        "Default Score", score.DefaultValue);
                }
                yOffset += 20;
                score.IsHigherBetter = EditorGUI.Toggle(new Rect(0, yOffset, width, 20),
                    "Is Higher Better", score.IsHigherBetter);
                yOffset += 20;
                bool isVirtualItemScore = !string.IsNullOrEmpty(score.RelatedVirtualItemID);
                isVirtualItemScore = EditorGUI.Toggle(new Rect(0, yOffset, width, 20), "Is Virtual Item Score", isVirtualItemScore);
                yOffset += 20;
                if (isVirtualItemScore)
                {
                    if (_relatedVirtualItemIDProperty != null)
                    {
                        EditorGUI.PropertyField(new Rect(0, yOffset, width, 20), _relatedVirtualItemIDProperty,
                            new GUIContent("Related Virtual Item"));
                        score.RelatedVirtualItemID = _relatedVirtualItemIDProperty.stringValue;
                    }
                    else
                    {
                        EditorGUI.LabelField(new Rect(0, yOffset, width, 20), "ERROR");
                    }
                    yOffset += 20;
                }
                else
                {
                    score.RelatedVirtualItemID = string.Empty;
                }
                //EditorGUI.LabelField(new Rect(0, yOffset, width, 20), "Related Virtual Item ID", score.RelatedVirtualItemID);
            }

            return yOffset;
        }

        protected override IItem GetItemWithConflictingID(IItem item, string id)
        {
            return GameKit.Config.GetScoreByID(id);
        }

        private bool _isBasicPropertiesExpanded = true;
        private bool _isScorePropertiesExpanded = true;
        private World _currentWorldOfScore;
        private SerializedProperty _relatedVirtualItemIDProperty;
    }
}