using UnityEngine;

namespace Beetle23
{
    public class ScoreTreeExplorer : ItemTreeExplorer
    {
        public ScoreTreeExplorer(GameKitConfig config)
            : base(config)
        {
        }

        protected override void DoOnSelectItem(IItem item) { }

        protected override void DoExpandAll() { }

        protected override void DoCollapseAll() { }

        protected override void DoDraw(Rect position) { }
    }
}
