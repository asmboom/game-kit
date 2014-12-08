using UnityEngine;
using System.Collections;

namespace Beetle23
{
    public class GatePopupAttribute : PropertyAttribute
    {
        public int SelectedValue = 0;
        public bool AllowNone = true;
        public bool AllowGroup = true;

        public GatePopupAttribute(bool allowNone, bool allowGroup)
        {
            this.AllowNone = allowNone;
            this.AllowGroup = allowGroup;
        }
    }
}