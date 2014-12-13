using UnityEngine;
using System.Collections;
using Rotorz.ReorderableList;
using UnityEditor;

namespace Beetle23
{
    public class GateTreeExplorer : ItemTreeExplorer
    {
        public GateTreeExplorer(GameKitConfig config)
            : base(config)
        {
            _gateListAdaptor = new GenericClassListAdaptor<Gate>(config.Gates, 20,
                                    () => { return new Gate(); },
                                    DrawItem<Gate>);
            _gateListAdaptor.OnItemRemoved += VirtualItemsEditUtil.UpdateDisplayedOptions;
            _gateListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand);
            _gateListControl.ItemRemoving += OnItemRemoving<Gate>;
            _gateListControl.ItemInserted += OnItemInsert<Gate>;
        }

        protected override void DoOnSelectItem(IItem item) { }

        protected override void DoExpandAll()
        {
        }

        protected override void DoCollapseAll()
        {
        }

        protected override void DoDraw(Rect position, string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                _gateListControl.Draw(_gateListAdaptor);
            }
            else
            {
                foreach (var gate in _config.Gates)
                {
                    DrawItemIfMathSearch(searchText, gate, position.width);
                }
            }
        }

        private T DrawItem<T>(Rect position, T item, int index) where T : SerializableItem
        {
            if (item == null)
            {
                GUI.Label(position, "NULL");
                return item;
            }

            if (GUI.Button(position, item.ID, GetItemCenterStyle(item)))
            {
                SelectItem(item);
            }
            return item;
        }

        private void OnItemInsert<T>(object sender, ItemInsertedEventArgs args) where T : SerializableItem
        {
            GenericClassListAdaptor<T> listAdaptor = args.adaptor as GenericClassListAdaptor<T>;
            if (listAdaptor != null)
            {
                SelectItem(listAdaptor[args.itemIndex]);
                GameKitEditorWindow.GetInstance().Repaint();
            }
        }

        private void OnItemRemoving<T>(object sender, ItemRemovingEventArgs args) where T : SerializableItem
        {
            GenericClassListAdaptor<T> listAdaptor = args.adaptor as GenericClassListAdaptor<T>;
            T item = listAdaptor[args.itemIndex];
            if (listAdaptor != null)
            {
                if (EditorUtility.DisplayDialog("Confirm to delete",
                        "Confirm to delete item [" + item.ID + "]?", "OK", "Cancel"))
                {
                    args.Cancel = false;
                    SelectItem(null);
                    GameKitEditorWindow.GetInstance().Repaint();
                }
                else
                {
                    args.Cancel = true;
                }
            }
        }

        private ReorderableListControl _gateListControl;
        private GenericClassListAdaptor<Gate> _gateListAdaptor;
    }
}
