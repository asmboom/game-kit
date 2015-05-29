using UnityEngine;

namespace Codeplay
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
                    Delegate.GetRelatedItem(RelatedItemID);
            }
        }

        public void Give()
        {
            Delegate.Give(this);
        }

        public void Take()
        {
            Delegate.Take(this);
        }

        private IRewardDelegate Delegate
        {
            get
            {
                if (_delegate == null)
                {
                    _delegate = RewardDelegateFactory.Get(Type);
                }
                return _delegate;
            }
        }

        private IRewardDelegate _delegate;
    }
}