using UnityEngine;

public class PlayerPrefsEconomyStorage : IEconomyStorage
{
    public const string KeyPrefixItemBalance = "economy_kit_item_balance_";
    public const string KeyPrefixItemEquip = "economy_kit_item_equip_";
    public const string KeyPrefixItemLevel = "economy_kit_item_level_";

    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;
    }

    public int GetItemBalance(string itemId)
    {
        return PlayerPrefs.GetInt(string.Format("{0}{1}", KeyPrefixItemBalance, itemId), 0);
    }

    public void SetItemBalance(string itemId, int balance)
    {
        balance = Mathf.Max(0, balance);
        PlayerPrefs.SetInt(string.Format("{0}{1}", KeyPrefixItemBalance, itemId), balance);
    }

    public void EquipVirtualGood(string itemId)
    {
        SetBool(string.Format("{0}{1}", KeyPrefixItemEquip, itemId), true);
    }

    public void UnEquipVirtualGood(string itemId)
    {
        SetBool(string.Format("{0}{1}", KeyPrefixItemEquip, itemId), false);
    }

    public bool IsVertualGoodEquipped(string itemId)
    {
        return GetBool(string.Format("{0}{1}", KeyPrefixItemEquip, itemId), false);
    }

    public int GetGoodCurrentLevel(string goodItemId)
    {
        return PlayerPrefs.GetInt(string.Format("{0}{1}", KeyPrefixItemLevel, goodItemId), 0);
    }

    public void SetGoodCurrentLevel(string itemId, int level)
    {
        level = Mathf.Max(0, level);
        PlayerPrefs.SetInt(string.Format("{0}{1}", KeyPrefixItemLevel, itemId), level);
    }
}