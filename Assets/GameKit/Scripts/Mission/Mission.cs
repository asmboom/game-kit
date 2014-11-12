using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Beetle23
{
    public class Mission : ScriptableItem
    {
        public Action OnCompleted = delegate { };
    }
}
