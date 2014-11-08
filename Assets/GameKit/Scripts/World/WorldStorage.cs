using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Beetle23
{
    public static class WorldStorage
    {
        public static bool IsCompleted(string id)
        {
            return Storage.Instance.GetInt(string.Format("{0}{1}", KeyPrefixWorldCompleted, id), 0) == 1;
        }

        public static void SetCompleted(string id, bool value)
        {
        	Storage.Instance.SetInt(string.Format("{0}{1}", KeyPrefixWorldCompleted, id), value ? 1 : 0);
        }

        private const string KeyPrefixWorldCompleted = "game_kit_world_completed";
    }
}
