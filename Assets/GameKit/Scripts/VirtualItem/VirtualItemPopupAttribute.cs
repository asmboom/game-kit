using UnityEngine;
using System.Collections;

namespace Codeplay
{
    public class VirtualItemPopupAttritube : PropertyAttribute
    {
        public int SelectedValue = 0;
        public bool AllowNone = true;
        public VirtualItemType TypeInclude = VirtualItemType.None;

        public VirtualItemPopupAttritube(VirtualItemType typeInclude, bool allowNone = true)
        {
            this.TypeInclude = typeInclude;
            this.AllowNone = allowNone;
        }
    }
}
