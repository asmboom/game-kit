using UnityEngine;

namespace Beetle23
{
    public class Entity : ScriptableObject
    {
        [SerializeField]
        public string ID;
        [SerializeField]
        public string Name;
        [SerializeField]
        public string Description;
    }
}
