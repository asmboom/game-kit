using UnityEngine;

namespace Beetle23
{
    [System.Serializable]
    public class SerializableItem : IItem
    {
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        [SerializeField]
        private string _id;

        [SerializeField]
        private string _name;
    }
}
