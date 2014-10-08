using System;
using UnityEngine;

namespace Beetle23
{
    public static class EconomyKit
    {
        public static Action<PurchasableItem> OnPurchaseStarted = delegate { };
        public static Action<PurchasableItem> OnPurchaseSucceeded = delegate { };
        public static Action<PurchasableItem> OnPurchaseFailed = delegate { };

        public static void Init(IEconomyKitFactory factory)
        {
            _factory = factory;
        }

        public static VirtualItemsConfig Config
        {
            get
            {
                if (_config == null)
                {
                    _config = Resources.Load<VirtualItemsConfig>("VirtualItemsConfig");
                    if (_config == null)
                    {
                        Debug.LogWarning("Create empty VirtualItemsConfig at runtime");
                        _config = ScriptableObject.CreateInstance<VirtualItemsConfig>();
#if UNITY_EDITOR
                        string fullPath = "Assets/StoreKit/Resources/VirtualItemsConfig.asset";
                        UnityEditor.AssetDatabase.CreateAsset(_config, fullPath);
#endif
                    }
                }
                return _config;
            }
        }

        public static void LogDebug(string tag, string message)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log(string.Format("{0} {1}", tag, message));
            }
        }

        public static void LogError(string tag, string message)
        {
            Debug.LogError(string.Format("{0} {1}", tag, message));
        }

        internal static Market CreateMarket()
        {
            if (_factory != null)
            {
                return _factory.CreateMarket();
            }
            else
            {
                Debug.LogError("You need to call EconomyKit::Init function first!!!");
                return null;
            }
        }

        internal static IStorage CreateStorage()
        {
            if (_factory != null)
            {
                return _factory.CreateStorage();
            }
            else
            {
                Debug.LogError("You need to call EconomyKit::Init function first!!!");
                return null;
            }
        }

        private static VirtualItemsConfig _config;
        private static IEconomyKitFactory _factory;
    }
}