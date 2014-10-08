namespace Beetle23
{
    public class VirtualItemRewardDelegate : IRewardDelegate
    {
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
    }
}