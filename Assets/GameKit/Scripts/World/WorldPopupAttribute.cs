using UnityEngine;
using System.Collections;

namespace Codeplay
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