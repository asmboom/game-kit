using UnityEngine;

[System.Serializable]
public class Reward
{
    public RewardType Type;
    public ScriptableObject RelatedItem;
    public int RewardNumber;

    public void Give()
    {
        RewardDelegateFactory.Get(Type).Give(this);
    }
}