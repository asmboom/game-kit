using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
{
    public class GatePropertyInspector : ItemPropertyInspector
    {
        public GatePropertyInspector(GateTreeExplorer treeExplorer)
            :base(treeExplorer)
        {
        }

        protected override void DoOnExplorerSelectionChange(IItem item)
        {
        }

        protected override float DoDrawItem(Rect rect, IItem item)
        {
        	return 0;
        }

        protected override IItem GetItemWithConflictingID(IItem item, string id)
        {
        	// TODO
        	return null;
        }
    }
}
