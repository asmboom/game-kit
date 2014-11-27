using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

namespace Beetle23
{
    public class WorldPropertyInspector
    {
        public WorldPropertyInspector(WorldTreeExplorer treeExplorer, World currentDisplayWorld)
        {
            _treeExplorer = treeExplorer;
            _currentDisplayWorld = currentDisplayWorld;
            _subWorldListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand |
                ReorderableListFlags.ShowIndices);
            _subWorldListControl.ItemInserted += OnInsertSubworld;
            _subWorldListControl.ItemRemoving += OnRemoveSubworld;
            /*
            _purchaseListView = new PurchaseInfoListView(currentDisplayedItem as PurchasableItem);
            _packListView = new PackInfoListView(currentDisplayedItem as VirtualItemPack);
            _upgradesListView = new UpgradesListView(currentDisplayedItem as VirtualItem);
            _categoryPropertyView = new CategoryPropertyView(currentDisplayedItem as VirtualCategory);
            */
        }

        public void OnExplorerSelectionChange(World world)
        {
            _currentDisplayWorld = world;
            _currentWorldID = _currentDisplayWorld.ID;
            _subWorldListAdaptor = new GenericClassListAdaptor<World>(_currentDisplayWorld.SubWorlds, 20,
                () => { return new World(); },
                (position, item, index) =>
                {
                    var size = GUI.skin.GetStyle("label").CalcSize(new GUIContent(item.ID));
                    GUI.Label(new Rect(position.x, position.y, size.x, position.height), item.ID);
                    if (GUI.Button(new Rect(position.x + size.x + 10, position.y, 50, position.height), "Go"))
                    {
                        _treeExplorer.SelectWorld(item);
                    }
                    return item;
                });
            //_packListView.UpdateDisplayItem(world as VirtualItemPack);
            //_purchaseListView.UpdateDisplayItem(world as PurchasableItem);
        }

        public void Draw(Rect position)
        {
            GUI.BeginGroup(position, string.Empty, "Box");
            if (_currentDisplayWorld != null)
            {
                DrawWorld(new Rect(10, 0, position.width - 10, position.height), _currentDisplayWorld);
            }
            GUI.EndGroup();
        }

        private void DrawWorld(Rect position, World world)
        {
            GUI.BeginGroup(position);
            _scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height),
                _scrollPosition, new Rect(0, 0, position.width - 20, _currentYOffset));

            float yOffset = 0;
            bool showScrollbar = position.height < _currentYOffset;
            float width = position.width - (showScrollbar ? 20 : 10);
            _isBasicPropertiesExpanded = EditorGUI.Foldout(new Rect(0, 0, width, 20),
                _isBasicPropertiesExpanded, "Basic Property");
            yOffset += 20;
            if (_isBasicPropertiesExpanded)
            {
                DrawID(new Rect(0, yOffset, width, 20), world);
                yOffset += 20;
                world.Name = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Name", world.Name);
                yOffset += 20;
                world.Description = EditorGUI.TextField(new Rect(0, yOffset, width, 20), "Desription", world.Description);
                yOffset += 20;
                world.Extend = EditorGUI.ObjectField(new Rect(0, yOffset, width, 20), "Extend",
                    world.Extend, typeof(ScriptableObject), false) as ScriptableObject;
                yOffset += 20;
            }

            yOffset += 20;
            EditorGUI.LabelField(new Rect(0, yOffset, 250, 20), "Parent World", 
                _currentDisplayWorld.Parent == null ? "NULL" : _currentDisplayWorld.Parent.ID);
            if (_currentDisplayWorld.Parent != null)
            {
                if (GUI.Button(new Rect(255, yOffset, 50, 20), "Go"))
                {
                    _treeExplorer.SelectWorld(_currentDisplayWorld.Parent);
                }
            }
            yOffset += 20;

            yOffset += 20;
            _isSubWorldExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20), _isSubWorldExpanded, "Child Worlds");
            yOffset += 20;
            if (_isSubWorldExpanded)
            {
                float height = _subWorldListControl.CalculateListHeight(_subWorldListAdaptor);
                _subWorldListControl.Draw(new Rect(0, yOffset, width, height), _subWorldListAdaptor);
                yOffset += height;
            }

            yOffset += 20;
            _isScoreInfoExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20), _isScoreInfoExpanded, "Scores");
            yOffset += 20;
            if (_isScoreInfoExpanded)
            {
            }

            yOffset += 20;
            _isMissionInfoExpanded = EditorGUI.Foldout(new Rect(0, yOffset, width, 20), _isMissionInfoExpanded, "Missions");
            yOffset += 20;
            if (_isMissionInfoExpanded)
            {
            }

            _currentYOffset = yOffset;

            GUI.EndScrollView();
            GUI.EndGroup();
        }

        private void DrawID(Rect position, World world)
        {
            if (world.Parent == null)
            {
                EditorGUI.LabelField(position, "ID", world.ID);
            }
            else
            {
                GUI.SetNextControlName(IDInputControlName);
                if (EditorGUI.TextField(position, "ID",
                    _currentWorldID).KeyPressed<string>(IDInputControlName, KeyCode.Return, out _currentWorldID) ||
                    (GUI.GetNameOfFocusedControl() != IDInputControlName &&
                     _currentWorldID != world.ID))
                {
                    World worldWithID = GameKit.Config.GetWorldByID(_currentWorldID);
                    if (worldWithID != null && worldWithID != world)
                    {
                        GUIUtility.keyboardControl = 0;
                        EditorUtility.DisplayDialog("Duplicate ID", "A world with ID[" +
                            _currentWorldID + "] already exists!!!", "OK");
                        _currentWorldID = world.ID;
                    }
                    else
                    {
                        world.ID = _currentWorldID;
                        GameKitEditorWindow.GetInstance().Repaint();
                    }
                }
            }
        }

        private void OnInsertSubworld(object sender, ItemInsertedEventArgs args)
        {
            GameKit.Config.UpdateMapsAndTree();
            GenericClassListAdaptor<World> listAdaptor = args.adaptor as GenericClassListAdaptor<World>;
            World world = listAdaptor[args.itemIndex];
            _treeExplorer.AddWorld(world);
        }

        private void OnRemoveSubworld(object sender, ItemRemovingEventArgs args)
        {
            GenericClassListAdaptor<World> listAdaptor = args.adaptor as GenericClassListAdaptor<World>;
            World world = listAdaptor[args.itemIndex];
            if (listAdaptor != null)
            {
                if (EditorUtility.DisplayDialog("Confirm to delete",
                        "Confirm to delete world [" + world.ID + "]?", "OK", "Cancel"))
                {
                    args.Cancel = false;
                    _treeExplorer.RemoveWorld(world);
                    GameKit.Config.UpdateMapsAndTree();
                    GameKitEditorWindow.GetInstance().Repaint();
                }
                else
                {
                    args.Cancel = true;
                }
            }
        }

        private bool _isBasicPropertiesExpanded = true;
        private bool _isSubWorldExpanded = true;
        private bool _isScoreInfoExpanded = true;
        private bool _isMissionInfoExpanded = false;

        private World _currentDisplayWorld;
        private ReorderableListControl _subWorldListControl;
        private GenericClassListAdaptor<World> _subWorldListAdaptor;

        //private PurchaseInfoListView _purchaseListView;
        //private PackInfoListView _packListView;
        //private UpgradesListView _upgradesListView;
        //private CategoryPropertyView _categoryPropertyView;

        private Vector2 _scrollPosition;
        private float _currentYOffset;
        private WorldTreeExplorer _treeExplorer;

        private string _currentWorldID;
        private const string IDInputControlName = "world_id_field";
    }
}