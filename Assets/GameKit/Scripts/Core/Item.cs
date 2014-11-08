using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beetle23
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField]
        public string Name;

        [SerializeField]
        public string Description;

        [SerializeField]
        public string ID;

        [SerializeField]
        public Sprite Icon;
    }
}
