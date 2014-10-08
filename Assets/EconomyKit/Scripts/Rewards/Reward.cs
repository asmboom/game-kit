using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class Reward
    {
        public RewardType Type;
        public string RelatedEntityID;
        public int RewardNumber;

        public void Give()
        {
            RewardDelegateFactory.Get(Type).Give(this);
        }
    }
}