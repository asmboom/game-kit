using UnityEngine;

namespace Codeplay
{
    internal static class VirtualItemStorage
    {
        public static int GetItemBalance(string itemId)
        {
            return Storage.Instance.GetInt(string.Format("{0}{1}", KeyPrefixItemBalance, itemId), 0);
        }

        public static void SetItemBalance(string itemId, int balance)
        {
            balance = Mathf.Max(0, balance);
            Storage.Instance.SetInt(string.Format("{0}{1}", KeyPrefixItemBalance, itemId), balance);
        }

        public static void EquipVirtualGood(string itemId)
        {
            Storage.Instance.SetBool(string.Format("{0}{1}", KeyPrefixItemEquip, itemId), true);
        }

        public static void UnEquipVirtualGood(string itemId)
        {
            Storage.Instance.SetBool(string.Format("{0}{1}", KeyPrefixItemEquip, itemId), false);
        }

        public static bool IsVertualGoodEquipped(string itemId)
        {
            return Storage.Instance.GetBool(string.Format("{0}{1}", KeyPrefixItemEquip, itemId), false);
        }

        public static int GetGoodCurrentLevel(string goodItemId)
        {
            return Storage.Instance.GetInt(string.Format("{0}{1}", KeyPrefixItemLevel, goodItemId), 0);
        }

        public static void SetGoodCurrentLevel(string itemId, int level)
        {
            level = Mathf.Max(0, level);
            Storage.Instance.SetInt(string.Format("{0}{1}", KeyPrefixItemLevel, itemId), level);
        }

        private const string KeyPrefixItemBalance = "economy_kit_item_balance_";
        private const string KeyPrefixItemEquip = "economy_kit_item_equip_";
        private const string KeyPrefixItemLevel = "economy_kit_item_level_";
    }
}
