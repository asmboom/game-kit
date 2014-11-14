using UnityEngine;
using UnityEditor;

namespace Beetle23
{
    [CustomEditor(typeof(World))]
    public class WorldEditor : Editor
    {
        [MenuItem("Assets/Create/World")]
        public static void CreateWorldMenuItem()
        {
            string configFilePath = VirtualItemsEditUtil.DefaultVirtualItemDataPath + "/New World.asset";
            VirtualItemsEditUtil.CreateAsset<World>(configFilePath);
        }
    }
}
