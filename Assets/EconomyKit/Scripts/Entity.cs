using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class Entity
    {
        public string ID;
        public string Name;
        public string Description;
        public ScriptableObject Extend;

        public Entity(string id, string name, string description)
        {
            ID = id;
            Name = name;
            Description = description;
        }

        public T GetExtend<T>() where T : ScriptableObject
        {
            return Extend as T;
        }
    }
}
