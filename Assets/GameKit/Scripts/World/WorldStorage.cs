namespace Codeplay
{
    internal static class WorldStorage
    {
        public static bool IsUnlocked(string id)
        {
            return Storage.Instance.GetInt(string.Format("{0}{1}", KeyPrefixWorldUnlocked, id), 0) == 1;
        }

        public static void SetUnlocked(string id, bool value)
        {
        	Storage.Instance.SetInt(string.Format("{0}{1}", KeyPrefixWorldUnlocked, id), value ? 1 : 0);
        }
        public static bool IsCompleted(string id)
        {
            return Storage.Instance.GetInt(string.Format("{0}{1}", KeyPrefixWorldCompleted, id), 0) == 1;
        }

        public static void SetCompleted(string id, bool value)
        {
        	Storage.Instance.SetInt(string.Format("{0}{1}", KeyPrefixWorldCompleted, id), value ? 1 : 0);
        }

        private const string KeyPrefixWorldUnlocked = "game_kit_world_unlocked_";
        private const string KeyPrefixWorldCompleted = "game_kit_world_completed_";
    }
}
