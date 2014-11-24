using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class Reward : SerializableItem
    {
        public RewardType Type;
        public string RelatedItemID;
        public int RewardNumber;

        public IItem RelatedItem
        {
            get
            {
                return string.IsNullOrEmpty(RelatedItemID) ? null : 
                    GameKit.Config.GetVirtualItemByID(RelatedItemID);
            }
        }

        public void Give()
        {
            RewardDelegateFactory.Get(Type).Give(this);
        }

        public void Take()
        {
            RewardDelegateFactory.Get(Type).Take(this);
        }
    }
}