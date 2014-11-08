using UnityEngine;

namespace Beetle23
{
    public static class GateStorage
    {
        public static bool IsOpen(string itemId)
        {
            return Storage.Instance.GetInt(string.Format("{0}{1}", KeyPrefixGateOpen, itemId), 0) == 1;
        }

        public static void SetOpen(string itemId, bool value)
        {
        	Storage.Instance.SetInt(string.Format("{0}{1}", KeyPrefixGateOpen, itemId), value ? 1 : 0);
        }

        private const string KeyPrefixGateOpen = "game_kit_gate_open";
    }
}
