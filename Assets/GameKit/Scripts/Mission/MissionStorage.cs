using UnityEngine;

namespace Codeplay
{
    internal static class MissionStorage
    {
        public static bool IsCompleted(string itemId)
        {
            return Storage.Instance.GetInt(string.Format("{0}{1}", KeyPrefixMissionCompleted, itemId), 0) == 1;
        }

        public static void SetCompleted(string itemId, bool value)
        {
        	Storage.Instance.SetInt(string.Format("{0}{1}", KeyPrefixMissionCompleted, itemId), value ? 1 : 0);
        }

        private const string KeyPrefixMissionCompleted = "game_kit_mission_completed_";
    }
}