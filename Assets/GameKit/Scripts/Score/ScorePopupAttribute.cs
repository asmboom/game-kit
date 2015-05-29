using UnityEngine;
using System.Collections;

namespace Codeplay
{
    public class ScorePopupAttribute : PropertyAttribute
    {
        public int SelectedValue = 0;
        public bool AllowNone = true;

        public ScorePopupAttribute(bool allowNone)
        {
            this.AllowNone = allowNone;
        }
    }
}