using UnityEngine;
using System.Collections;

namespace Beetle23
{
    public class WorldPopupAttribute : PropertyAttribute
    {
        public int SelectedValue = 0;
        public bool AllowNone = true;

        public WorldPopupAttribute(bool allowNone)
        {
            this.AllowNone = allowNone;
        }
    }
}