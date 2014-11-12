using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
{
    public abstract class ScriptableItem : ScriptableObject, IItem
    {
        [SerializeField]
        public string _name;

        [SerializeField]
        public string _description;

        [SerializeField]
        public string _id;

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

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
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
    }
}
