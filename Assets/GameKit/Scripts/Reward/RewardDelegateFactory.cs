using System.Collections.Generic;

namespace Codeplay
{
    public static class RewardDelegateFactory
    {
        public static IRewardDelegate Get(RewardType type)
        {
            if (!_typeToDelegate.ContainsKey(type))
            {
                switch (type)
                {
                    case RewardType.VirtualItemReward:
                        _typeToDelegate.Add(type, new VirtualItemRewardDelegate());
                        break;
                    default:
                        return null;
                }
            }

            return _typeToDelegate[type];
        }

        static RewardDelegateFactory()
        {
            _typeToDelegate = new Dictionary<RewardType, IRewardDelegate>();
        }

        private static Dictionary<RewardType, IRewardDelegate> _typeToDelegate;
    }
}