using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
{
    public static class RewardPropertyView
    {
        public static float CalculateHeight(Reward reward)
        {
            return DrawReward(reward, 0, true);
        }

        public static float Draw(Rect rect, Reward reward)
        {
            GUI.BeginGroup(rect);
            float yOffset = DrawReward(reward, rect.width, false);
            GUI.EndGroup();
            return yOffset;
        }

        private static float DrawReward(Reward reward, float width, bool calculateHeight)
        {
            float xOffset = 10;
            float yOffset = 0;
            if (!calculateHeight)
            {
                EditorGUI.LabelField(new Rect(xOffset, yOffset, width - xOffset, 20), new GUIContent("ID"), new GUIContent(reward.ID));
            }
            yOffset += 20;
            if (!calculateHeight)
            {
                reward.Type = (RewardType)EditorGUI.EnumPopup(new Rect(xOffset, yOffset, width - xOffset, 20), "Type", reward.Type);
            }
            yOffset += 20;
            if (!calculateHeight)
            {
                reward.RelatedItemID = _itemPopupDrawer.Draw(new Rect(xOffset, yOffset, width - xOffset, 20), 
                    reward.RelatedItemID, new GUIContent("Virtual Item"));
            }
            yOffset += 20;
            if (!calculateHeight)
            {
                reward.RewardNumber = Mathf.Max(0, EditorGUI.IntField(new Rect(xOffset, yOffset, width - xOffset, 20), "Count", reward.RewardNumber));
            }
            yOffset += 20;
            return yOffset;
        }

        private static ItemPopupDrawer _itemPopupDrawer = new ItemPopupDrawer(ItemType.VirtualItem, false,
                    VirtualItemType.VirtualCurrency | VirtualItemType.LifeTimeItem | VirtualItemType.SingleUseItem);
    }
}