using UnityEngine;

namespace Beetle23
{
    public static class ScoreStorage
    {
    	public static float GetRecordScore(string itemId)
    	{
            return Storage.Instance.GetFloat(string.Format("{0}{1}", KeyPrefixScore, itemId), 0);
    	}

    	public static void SetRecordScore(string itemId, float value)
    	{
    		Storage.Instance.SetFloat(string.Format("{0}{1}", KeyPrefixScore, itemId), value);
    	}

        private const string KeyPrefixScore = "game_kit_score_";
    }
}
