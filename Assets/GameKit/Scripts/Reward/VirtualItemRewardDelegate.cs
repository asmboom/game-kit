namespace Beetle23
{
    public class VirtualItemRewardDelegate : IRewardDelegate
    {
        public IItem GetRelatedItem(string itemId)
        {
            return GameKit.Config.GetVirtualItemByID(itemId);
        }

        public void Give(Reward reward)
        {
            VirtualItem item = reward.RelatedItem as VirtualItem;
            if (item != null)
            {
                item.Give(reward.RewardNumber);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Virtual item's reward item is not a virtual item.");
            }
        }

        public void Take(Reward reward)
        {
            VirtualItem item = reward.RelatedItem as VirtualItem;
            if (item != null)
            {
                item.Take(reward.RewardNumber);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Virtual item's reward item is not a virtual item.");
            }
        }
    }
}