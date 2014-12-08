using UnityEngine;
using System.Collections;

namespace Beetle23
{
	public class MissionTreeExplorer : ItemTreeExplorer
	{
		public MissionTreeExplorer(GameKitConfig config)
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
