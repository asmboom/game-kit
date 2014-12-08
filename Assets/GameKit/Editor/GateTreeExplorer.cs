using UnityEngine;
using System.Collections;

namespace Beetle23
{
	public class GateTreeExplorer : ItemTreeExplorer
	{
		public GateTreeExplorer(GameKitConfig config)
            : base(config)
		{
		}
		
        protected override void DoOnSelectItem(IItem item) { }

        protected override void DoExpandAll()
        {
        }

        protected override void DoCollapseAll()
        {
        }

        protected override void DoDraw(Rect position)
        {
        }
	}
}
