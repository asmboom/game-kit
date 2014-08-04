using System;
using UnityEngine;

public static class StoreKit
{
    public static void Init(IStoreKitFactory factory)
    {
        _factory = factory;
    }

    public static StoreConfig Config
    {
        get
        {
            if (_config == null)
            {
                _config = Resources.Load<StoreConfig>("StoreConfig");
                if (_config == null)
                {
                    Debug.LogWarning("Create empty StoreConfig at runtime");
                    _config = ScriptableObject.CreateInstance<StoreConfig>();
#if UNITY_EDITOR
                    string fullPath = "Assets/StoreKit/Resources/StoreConfig.asset";
                    UnityEditor.AssetDatabase.CreateAsset(_config, fullPath);
#endif
                }
            }
            return _config;
        }
    }

    internal static Market CreateMarket()
    {
        if (_factory != null)
        {
            return _factory.CreateMarket();
        }
        else
        {
            Debug.LogError("You need to call Init function first!!!");
            return null;
        }
    }

    internal static IStoreStorage CreateInventory()
    {
        if (_factory != null)
        {
            return _factory.CreateStorage();
        }
        else
        {
            Debug.LogError("You need to call Init function first!!!");
            return null;
        }
    }

    private static StoreConfig _config;
    private static IStoreKitFactory _factory;
}