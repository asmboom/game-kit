using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System;

namespace Beetle23
{
    public class WorldTreeExplorer
    {
        public Action<World> OnSelectionChange = delegate { };
        public World CurrentSelectedWorld { get; private set; }

        public WorldTreeExplorer(GameKitConfig config)
        {
            _config = config;
            UpdateWorldUIData();
        }

        public void Draw(Rect position)
        {
            GUILayout.BeginArea(position, string.Empty, "Box");

            if (GUILayout.Button("Check References"))
            {
                GameKitConfigEditor.CheckIfAnyInvalidRef(_config);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+Expand All", GUILayout.Width(90)))
            {
            }
            if (GUILayout.Button("-Collapse All", GUILayout.Width(90)))
            {
            }
            GUILayout.EndHorizontal();

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            DrawWorldUIData(_rootWorldUIData);

            GUILayout.Space(30);

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        private void UpdateWorldUIData()
        {
            _rootWorldUIData = new WorldUIData(_config.RootWorld, 
                CreateWorld, DrawWorld, GetWorldItemHeight);
        }

        private void DrawWorldUIData(WorldUIData worldUIData)
        {
            worldUIData.Expanded = EditorGUILayout.Foldout(worldUIData.Expanded,
                worldUIData.RelatedWorld.ID, GameKitEditorDrawUtil.FoldoutStyle);
            if (worldUIData.Expanded)
            {
                worldUIData.ListControl.Draw(worldUIData.ListAdaptor);
            }
        }

        private World CreateWorld()
        {
            return new World();
        }

        private float GetWorldItemHeight(World world)
        {
            return 20;
        }

        private World DrawWorld(Rect position, World item, int index)
        {
            if (item == null)
            {
                GUI.Label(position, "NULL");
                return item;
            }

            if (GUI.Button(position, item.ID,
                    (!string.IsNullOrEmpty(item.ID) && item == CurrentSelectedWorld ?
                        GameKitEditorDrawUtil.ItemSelectedStyle : GameKitEditorDrawUtil.ItemStyle)))
            {
                SelectWorld(item);
            }
            return item;
        }

        private void OnItemInsert(object sender, ItemInsertedEventArgs args)
        {
            GenericClassListAdaptor<World> listAdaptor = args.adaptor as GenericClassListAdaptor<World>;
            if (listAdaptor != null)
            {
                SelectWorld(listAdaptor[args.itemIndex]);
                if (listAdaptor[args.itemIndex] is World)
                {
                    ShowInputDialogForId<World>(CurrentSelectedWorld.ID);
                }
            }
        }

        private void ShowInputDialogForId<T>(string defaultId) where T : SerializableItem
        {
            SingleInputDialog.Show("Enter id for the new item", defaultId, "OK", OnGetNewId<T>);
        }

        private void OnGetNewId<T>(string id) where T : SerializableItem
        {
            GameKit.Config.UpdateIdToItemMap();
            SerializableItem itemWithID = GameKit.Config.GetVirtualItemByID(id);
            if (itemWithID != null && itemWithID != CurrentSelectedWorld)
            {
                Debug.LogWarning("Id [" + id + "] is already used by [" + 
                    itemWithID.Name + "], please change one.");
                ShowInputDialogForId<T>(id);
            }
            else
            {
                CurrentSelectedWorld.ID = id;
                GameKitEditorWindow.GetInstance().Repaint();
            }
        }

        private void OnItemRemoving(object sender, ItemRemovingEventArgs args)
        {
            GenericClassListAdaptor<World> listAdaptor = args.adaptor as GenericClassListAdaptor<World>;
            World item = listAdaptor[args.itemIndex];
            if (listAdaptor != null)
            {
                if (EditorUtility.DisplayDialog("Confirm to delete",
                        "Confirm to delete item [" + item.ID + "]?", "OK", "Cancel"))
                {
                    args.Cancel = false;
                    SelectWorld(null);
                    GameKitEditorWindow.GetInstance().Repaint();
                }
                else
                {
                    args.Cancel = true;
                }
            }
        }

        private void SelectWorld(World world)
        {
            if (world != CurrentSelectedWorld)
            {
                CurrentSelectedWorld = world;
                OnSelectionChange(world);
            }
        }

        private class WorldUIData
        {
            public World RelatedWorld;
            public bool Expanded;
            public ReorderableListControl ListControl;
            public GenericClassListAdaptor<World> ListAdaptor;
            //public List<WorldUIData> SubWorldUIData;

            public WorldUIData(World world, 
                GenericListAdaptorDelegate.ItemCreator<World> itemCreator, 
                GenericListAdaptorDelegate.ClassItemDrawer<World> itemDrawer,
                GenericListAdaptorDelegate.ItemHeightGetter<World> itemHeightGetter)
            {
                Expanded = false;
                RelatedWorld = world;
                //SubWorldUIData = new List<WorldUIData>();
                //foreach (var subworld in world.SubWorlds)
                //{
                    //SubWorldUIData.Add(new WorldUIData(subworld));
                //}
                ListAdaptor = new GenericClassListAdaptor<World>(world.SubWorlds, 20, itemCreator, itemDrawer, itemHeightGetter);
                ListControl = new ReorderableListControl();
            }
        }

        private GameKitConfig _config;
        private Vector2 _scrollPosition;
        private WorldUIData _rootWorldUIData;
    }
}