using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
{
    public class WorldPropertyInspector : ItemPropertyInspector
    {
        public WorldPropertyInspector(WorldTreeExplorer treeExplorer)
            :base(treeExplorer)
        {
            _subWorldListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand |
                ReorderableListFlags.ShowIndices);
            _subWorldListControl.ItemInserted += OnInsertSubworld;
            _subWorldListControl.ItemRemoving += OnRemoveSubworld;

            _scoreListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand |
                ReorderableListFlags.ShowIndices);
            _scoreListControl.ItemInserted += OnInsertScore;
            _scoreListControl.ItemRemoving += OnRemoveScore;
        }

        protected override void DoOnExplorerSelectionChange(IItem item)
        {
            World world = item as World;
            _subWorldListAdaptor = new GenericClassListAdaptor<World>(world.SubWorlds, 20,
                () => { return new World(); },
                (position, theItem, index) =>
                {
                    var size = GUI.skin.GetStyle("label").CalcSize(new GUIContent(theItem.ID));
                    GUI.Label(new Rect(position.x, position.y, size.x, position.height), theItem.ID);
                    if (GUI.Button(new Rect(position.x + size.x + 10, position.y, 50, position.height), "Edit"))
                    {
                        _treeExplorer.SelectItem(theItem);
                    }
                    return theItem;
                });
            _scoreListAdaptor = new GenericClassListAdaptor<Score>(world.Scores, 20,
                () => { return new Score(); },
                (position, theItem, index) =>
                {
                    var size = GUI.skin.GetStyle("label").CalcSize(new GUIContent(theItem.ID));
                    GUI.Label(new Rect(position.x, position.y, size.x, position.height), theItem.ID);
                    if (GUI.Button(new Rect(position.x + size.x + 10, position.y, 50, position.height), "Edit"))
                    {
                        ScoreTreeExplorer scoreTreeExplorer = (GameKitEditorWindow.GetInstance().GetTreeExplorer(
                            GameKitEditorWindow.TabType.Scores) as ScoreTreeExplorer);
                        GameKitEditorWindow.GetInstance().SelectTab(GameKitEditorWindow.TabType.Scores);
                        scoreTreeExplorer.SelectItem(theItem);
                    }
                    return theItem;
                });
        }

        protected override float DoDrawItem(Rect rect, IItem item)
        {
            float yOffset = 0;
            float width = rect.width;
            World world = item as World;
            _isBasicPropertiesExpanded = EditorGUI.Foldout(new Rect(0, 0, width, 20),
                _isBasicPropertiesExpanded, "Item");
            yOffset += 20;
            if (_isBasicPropertiesExpanded)
            {
                DrawIDField(new Rect(0, yOffset, width, 20), world, world.Parent != null, true);
                yOffset += 20;
                world.Name = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Name", world.Name);
                yOffset += 20;
                world.Description = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Desription", world.Description);
                yOffset += 20;
                world.Extend = EditorGUI.ObjectField(new Rect(0, yOffset, width, 20), "Extend",
                    world.Extend, typeof(ScriptableObject), false) as ScriptableObject;
                yOffset += 20;
            }

            yOffset += 20;
            EditorGUI.LabelField(new Rect(0, yOffset, 250, 20), "Parent World", 
                world.Parent == null ? "NULL" : world.Parent.ID);
            if (world.Parent != null)
            {
                if (GUI.Button(new Rect(255, yOffset, 50, 20), "Edit"))
                {
                    _treeExplorer.SelectItem(world.Parent);
                }
            }
            yOffset += 20;

            yOffset += 20;
            _isSubWorldExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20), _isSubWorldExpanded, "Child Worlds");
            yOffset += 20;
            if (_isSubWorldExpanded)
            {
                float height = _subWorldListControl.CalculateListHeight(_subWorldListAdaptor);
                _subWorldListControl.Draw(new Rect(0, yOffset, width, height), _subWorldListAdaptor);
                yOffset += height;
            }

            yOffset += 20;
            _isScoreInfoExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20), _isScoreInfoExpanded, "Scores");
            yOffset += 20;
            if (_isScoreInfoExpanded)
            {
                float height = _scoreListControl.CalculateListHeight(_scoreListAdaptor);
                _scoreListControl.Draw(new Rect(0, yOffset, width, height), _scoreListAdaptor);
                yOffset += height;
            }

            yOffset += 20;
            _isMissionInfoExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20), _isMissionInfoExpanded, "Missions");
            yOffset += 20;
            if (_isMissionInfoExpanded)
            {
            }

            yOffset += 20;
            _isGateInfoExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20), _isGateInfoExpanded, "Gate");
            yOffset += 20;
            if (_isGateInfoExpanded)
            {
                world.Gate.Type = (GateType)EditorGUI.EnumPopup(
                    new Rect(0, yOffset, width, 20), "Group Type", world.Gate.Type);
                yOffset += 20;
            }

            return yOffset;
        }

        protected override IItem GetItemWithConflictingID(IItem item, string id)
        {
            return GameKit.Config.GetWorldByID(id);
        }

        private void OnInsertSubworld(object sender, ItemInsertedEventArgs args)
        {
            GameKit.Config.UpdateMapsAndTree();
            GenericClassListAdaptor<World> listAdaptor = args.adaptor as GenericClassListAdaptor<World>;
            World world = listAdaptor[args.itemIndex];
            (_treeExplorer as WorldTreeExplorer).AddWorld(world);
            ScoreTreeExplorer scoreTreeExplorer = (GameKitEditorWindow.GetInstance().GetTreeExplorer(
                GameKitEditorWindow.TabType.Scores) as ScoreTreeExplorer);
            scoreTreeExplorer.AddWorld(world);
        }

        private void OnRemoveSubworld(object sender, ItemRemovingEventArgs args)
        {
            GenericClassListAdaptor<World> listAdaptor = args.adaptor as GenericClassListAdaptor<World>;
            World world = listAdaptor[args.itemIndex];
            if (listAdaptor != null)
            {
                if (EditorUtility.DisplayDialog("Confirm to delete",
                        "Confirm to delete world [" + world.ID + "]?", "OK", "Cancel"))
                {
                    args.Cancel = false;
                    (_treeExplorer as WorldTreeExplorer).RemoveWorld(world);
                    ScoreTreeExplorer scoreTreeExplorer = (GameKitEditorWindow.GetInstance().GetTreeExplorer(
                        GameKitEditorWindow.TabType.Scores) as ScoreTreeExplorer);
                    scoreTreeExplorer.RemoveWorld(world);
                    GameKit.Config.UpdateMapsAndTree();
                    GameKitEditorWindow.GetInstance().Repaint();
                }
                else
                {
                    args.Cancel = true;
                }
            }
        }

        private void OnInsertScore(object sender, ItemInsertedEventArgs args)
        {
            GameKit.Config.UpdateMapsAndTree();
        }

        private void OnRemoveScore(object sender, ItemRemovingEventArgs args)
        {
            GenericClassListAdaptor<Score> listAdaptor = args.adaptor as GenericClassListAdaptor<Score>;
            Score score = listAdaptor[args.itemIndex];
            if (listAdaptor != null)
            {
                if (EditorUtility.DisplayDialog("Confirm to delete",
                        "Confirm to delete score [" + score.ID + "]?", "OK", "Cancel"))
                {
                    args.Cancel = false;
                    GameKit.Config.UpdateMapsAndTree();
                    GameKitEditorWindow.GetInstance().Repaint();

                    ScoreTreeExplorer scoreTreeExplorer = GameKitEditorWindow.GetInstance().GetTreeExplorer(
                        GameKitEditorWindow.TabType.Scores) as ScoreTreeExplorer;
                    if (scoreTreeExplorer.CurrentSelectedItem == score)
                    {
                        scoreTreeExplorer.SelectItem(null);
                    }
                }
                else
                {
                    args.Cancel = true;
                }
            }
        }

        private bool _isBasicPropertiesExpanded = true;
        private bool _isSubWorldExpanded = true;
        private bool _isScoreInfoExpanded = true;
        private bool _isMissionInfoExpanded = true;
        private bool _isGateInfoExpanded = true;

        private ReorderableListControl _subWorldListControl;
        private GenericClassListAdaptor<World> _subWorldListAdaptor;
        private ReorderableListControl _scoreListControl;
        private GenericClassListAdaptor<Score> _scoreListAdaptor;

        private Vector2 _scrollPosition;
        private float _currentYOffset;

        private const string IDInputControlName = "world_id_field";
    }
}