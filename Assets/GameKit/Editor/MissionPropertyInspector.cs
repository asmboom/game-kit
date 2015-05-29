using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Codeplay
{
    public class MissionPropertyInspector : ItemPropertyInspector
    {
        public MissionPropertyInspector(MissionTreeExplorer treeExplorer)
            : base(treeExplorer)
        {
            _listControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand |
                ReorderableListFlags.ShowIndices);
            _listControl.ItemInserted += OnItemInsert;
            _listControl.ItemRemoving += OnItemRemoving;

            Gate currentGate = treeExplorer.CurrentSelectedItem != null ?
                (treeExplorer.CurrentSelectedItem as Mission).Gate : null;
            _gateDrawer = new GatePropertyView(currentGate, true);
        }

        public override IItem[] GetAffectedItems(string itemID)
        {
            return new IItem[] { };
        }

        protected override void DoOnExplorerSelectionChange(IItem item)
        {
            if (item == null)
            {
                _currentWorldOfMission = null;
            }
            else if (item is Mission)
            {
                Mission mission = item as Mission;
                _currentWorldOfMission = GameKit.Config.FindWorldThatMissionBelongsTo(mission);
                _gateDrawer.UpdateDisplayItem(mission.Gate);
                _listAdaptor = new GenericClassListAdaptor<Reward>((item as Mission).Rewards, 18,
                    () =>
                    {
                        return new Reward();
                    },
                    (rect, reward, index) =>
                    {
                        RewardPropertyView.Draw(rect, reward);
                        return reward;
                    },
                    (reward) =>
                    {
                        return RewardPropertyView.CalculateHeight(reward);
                    });
            }
        }

        protected override float DoDrawItem(Rect rect, IItem item)
        {
            float yOffset = 0;
            float width = rect.width;
            Mission mission = item as Mission;

            _isBasicPropertiesExpanded = EditorGUI.Foldout(new Rect(0, 0, width, 20),
                _isBasicPropertiesExpanded, "Item");
            yOffset += 20;
            if (_isBasicPropertiesExpanded)
            {
                DrawIDField(new Rect(0, yOffset, width, 20), mission, true, true);
                yOffset += 20;
                mission.Name = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Name", mission.Name);
                yOffset += 20;
                mission.Description = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Desription", mission.Description);
                yOffset += 20;
                mission.BadgeIcon = EditorGUI.ObjectField(new Rect(0, yOffset, width, 50), "Badge Icon",
                    mission.BadgeIcon, typeof(Texture2D), false) as Texture2D;
                yOffset += 55;
            }
            yOffset += 20;

            if (_currentWorldOfMission != null)
            {
                EditorGUI.LabelField(new Rect(0, yOffset, 250, 20), "Belong to World",
                    _currentWorldOfMission == null ? "NULL" : _currentWorldOfMission.ID);
                if (GUI.Button(new Rect(255, yOffset, 50, 20), "Edit"))
                {
                    WorldTreeExplorer worldTreeExplorer = (GameKitEditorWindow.GetInstance().GetTreeExplorer(
                        GameKitEditorWindow.TabType.Worlds) as WorldTreeExplorer);
                    GameKitEditorWindow.GetInstance().SelectTab(GameKitEditorWindow.TabType.Worlds);
                    worldTreeExplorer.SelectItem(_currentWorldOfMission);
                }
                yOffset += 20;
            }
            yOffset += 20;

            UpdateGateID(mission.Gate);
            _isGatePropertiesExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20),
                _isGatePropertiesExpanded, "Gate");
            yOffset += 20;
            if (_isGatePropertiesExpanded)
            {
                float height = _gateDrawer.CalculateHeight(mission.Gate);
                yOffset += _gateDrawer.Draw(new Rect(0, yOffset, width, height), mission.Gate);
                yOffset += 20;
            }

            UpdateRewardsID();
            _isRewardsExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20),
                _isRewardsExpanded, "Rewards");
            yOffset += 20;
            if (_isRewardsExpanded)
            {
                float height = _listControl.CalculateListHeight(_listAdaptor);
                _listControl.Draw(new Rect(0, yOffset, width, height), _listAdaptor);
                yOffset += height;
            }

            return yOffset;
        }

        protected override IItem GetItemWithConflictingID(IItem item, string id)
        {
            return GameKit.Config.GetScoreByID(id);
        }

        private void UpdateRewardsID()
        {
            Mission mission = _currentDisplayItem as Mission;
            for (int i = 0; i < mission.Rewards.Count; i++)
            {
                mission.Rewards[i].ID = string.Format("reward_{0}_{1}", mission.ID, i);
            }
        }

        private void OnItemInsert(object sender, ItemInsertedEventArgs args) { }

        private void OnItemRemoving(object sender, ItemRemovingEventArgs args) { }

        private bool _isBasicPropertiesExpanded = true;
        private bool _isGatePropertiesExpanded = true;
        private bool _isRewardsExpanded = true;
        private World _currentWorldOfMission;
        private GatePropertyView _gateDrawer;

        private ReorderableListControl _listControl;
        private GenericClassListAdaptor<Reward> _listAdaptor;
    }
}
