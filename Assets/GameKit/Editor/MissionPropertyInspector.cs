using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
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

            _rewardItemDrawers = new List<ItemPopupDrawer>();
        }

        protected override void DoOnExplorerSelectionChange(IItem item)
        {
            if (item == null)
            {
                _currentWorldOfMission = null;
            }
            else
            {
                _currentWorldOfMission = GameKit.Config.FindWorldThatMissionBelongsTo(item as Mission);
                _gatePopupDrawer = new ItemPopupDrawer(ItemType.Gate, false, true);
                _listAdaptor = new GenericClassListAdaptor<Reward>((item as Mission).Rewards, 18,
                    () =>
                    {
                        return new Reward();
                    }, DrawOneReward);

                UpdateRewardItemPopupDrawers();
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

            _isMissionPropertiesExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20),
                _isMissionPropertiesExpanded, "Mission Property");
            yOffset += 20;
            if (_isMissionPropertiesExpanded)
            {
                mission.RelatedGateID = _gatePopupDrawer.Draw(new Rect(0, yOffset, width, 20), 
                    mission.RelatedGateID, new GUIContent("Related Gate"));
                yOffset += 20;

                EditorGUI.LabelField(new Rect(0, yOffset, width, yOffset), "Rewards");
                yOffset += 20;
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

        private Reward DrawOneReward(Rect position, Reward reward, int index)
        {
            if (reward == null) return null;

            float xOffset = position.x;

            reward.Type = (RewardType)EditorGUI.EnumPopup(new Rect(xOffset, position.y, position.width * RewardTypeWidth - 1, position.height), 
                reward.Type);
            xOffset += position.width * RewardTypeWidth;

            reward.RelatedItemID = _rewardItemDrawers[index].Draw(new Rect(xOffset, position.y, 
                position.width * RewardRelatedItemWidth - 1, position.height), reward.RelatedItemID, GUIContent.none);

            xOffset += position.width * RewardRelatedItemWidth;
            reward.RewardNumber = Mathf.Max(0, EditorGUI.IntField(new Rect(xOffset, position.y, 
                position.width * RewardRelatedNumberWidth - 1, position.height), reward.RewardNumber));

            return reward;
        }

        private void OnItemInsert(object sender, ItemInsertedEventArgs args) 
        {
            UpdateRewardItemPopupDrawers();
        }

        private void OnItemRemoving(object sender, ItemRemovingEventArgs args) 
        {
            UpdateRewardItemPopupDrawers();
        }

        private void UpdateRewardItemPopupDrawers()
        {
            _rewardItemDrawers.Clear();
            Mission mission = _currentDisplayItem as Mission;
            for (int i = 0; i < mission.Rewards.Count; i++)
            {
                _rewardItemDrawers.Add(new ItemPopupDrawer(ItemType.VirtualItem, false, 
                    VirtualItemType.VirtualCurrency | VirtualItemType.LifeTimeItem | VirtualItemType.SingleUseItem));
            }
        }

        private bool _isBasicPropertiesExpanded = true;
        private bool _isMissionPropertiesExpanded = true;
        private World _currentWorldOfMission;
        private ItemPopupDrawer _gatePopupDrawer;

        private ReorderableListControl _listControl;
        private GenericClassListAdaptor<Reward> _listAdaptor;
        private List<ItemPopupDrawer> _rewardItemDrawers;

        private const float RewardTypeWidth = 0.4f;
        private const float RewardRelatedItemWidth = 0.4f;
        private const float RewardRelatedNumberWidth = 0.2f;
    }
}
