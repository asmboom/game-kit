using System;
using UnityEngine;

namespace Beetle23
{
    public static class GameKit
    {
#if UNITY_EDITOR
        public const string DefaultConfigDataPath = "Assets/GameKit/Resources";
#endif

        public static void Init(IGameKitFactory factory)
        {
            _factory = factory;
        }

        public static GameKitConfig Config
        {
            get
            {
                if (_config == null)
                {
                    _config = Resources.Load<GameKitConfig>("GameKitConfig");
                    if (_config == null)
                    {
                        Debug.LogWarning("Create empty GameKitConfig at runtime");
                        _config = ScriptableObject.CreateInstance<GameKitConfig>();
#if UNITY_EDITOR
                        string fullPath = DefaultConfigDataPath + "/GameKitConfig.asset";
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

        private static GameKitConfig _config;
        private static IGameKitFactory _factory;
    }
}