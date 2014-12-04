using UnityEngine;
using System.Collections;

namespace Beetle23
{
    public class VirtualItemPopupAttritube : PropertyAttribute
    {
        public int SelectedValue = 0;
        public bool AllowNone = true;

        public VirtualItemPopupAttritube(bool allowNone = true)
        {
            this.AllowNone = allowNone;
        }
    }
}
