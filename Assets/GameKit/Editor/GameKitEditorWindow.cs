using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Beetle23
{
    public class GameKitEditorWindow : EditorWindow
    {
        [MenuItem("Window/GameKit Editor Window")]
        private static void OpenVirutalItemEditorWindow()
        {
            GameKitEditorWindow.GetInstance();
        }

        public enum TabType
        {
            VirtualItems = 0,
            Worlds = 1,
            Scores = 2,
            Missions = 3
        }

        public static GameKitEditorWindow GetInstance()
        {
            if (_instance == null)
            {
                _instance = EditorWindow.GetWindow<GameKitEditorWindow>("GameKit");
            }
            return _instance;
        }

        private static GameKitEditorWindow _instance;

        public ItemTreeExplorer GetTreeExplorer(TabType tabtype)
        {
            if (_treeExplorers.ContainsKey(tabtype))
            {
                return _treeExplorers[tabtype];
            }
            else
            {
                return null;
            }
        }

        public void SelectTab(TabType tabtype)
        {
            _currentSection = (int)tabtype;
        }

        private void OnEnable()
        {
            _sections = new string[] { "Virtual Items", "Worlds", "Scores", "Missions" };

            GetConfigAndCreateIfNonExist();

            if (_config == null)
            {
                _config = GameKit.Config;
            }
            if (_treeExplorers == null)
            {
                _treeExplorers = new Dictionary<TabType, ItemTreeExplorer>();
                _treeExplorers.Add(TabType.VirtualItems, new VirtualItemsTreeExplorer(_config));
                _treeExplorers.Add(TabType.Worlds, new WorldTreeExplorer(_config));
                _treeExplorers.Add(TabType.Scores, new ScoreTreeExplorer(_config));
            }
            if (_propertyInspectors == null)
            {
                _propertyInspectors = new Dictionary<TabType, ItemPropertyInspector>();
                _propertyInspectors.Add(TabType.VirtualItems, 
                    new VirtualItemsPropertyInspector(_treeExplorers[TabType.VirtualItems] as VirtualItemsTreeExplorer));
                _propertyInspectors.Add(TabType.Worlds, 
                    new WorldPropertyInspector(_treeExplorers[TabType.Worlds] as WorldTreeExplorer));
                _propertyInspectors.Add(TabType.Scores, 
                    new ScorePropertyInspector(_treeExplorers[TabType.Scores] as ScoreTreeExplorer));
            }

            for (TabType type = TabType.VirtualItems; type <= TabType.Scores; type++)
            {
                _treeExplorers[type].OnSelectionChange += _propertyInspectors[type].OnExplorerSelectionChange;
            }

            VirtualItemsEditUtil.UpdateDisplayedOptions();
        }

        private void OnDisable()
        {
            if (_treeExplorers != null)
            {
                for (TabType type = TabType.VirtualItems; type <= TabType.Scores; type++)
                {
                    _treeExplorers[type].OnSelectionChange -= _propertyInspectors[type].OnExplorerSelectionChange;
                }
            }
        }

        private void OnFocus()
        {
            if (_config != null)
            {
                _config.RemoveNullRefs();
                _config.UpdateMapsAndTree();
            }
            VirtualItemsEditUtil.UpdateDisplayedOptions();
        }

        private static GameKitConfig GetConfigAndCreateIfNonExist()
        {
            string configFilePath = GameKit.DefaultConfigDataPath + "/GameKitConfig.asset";
            GameKitConfig virtualItemsConfig = AssetDatabase.LoadAssetAtPath(configFilePath, typeof(GameKitConfig)) as GameKitConfig;
            if (virtualItemsConfig == null)
            {
                virtualItemsConfig = VirtualItemsEditUtil.CreateAsset<GameKitConfig>(configFilePath);
            }
            return virtualItemsConfig;
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            float y = 0;
            _currentSection = GUI.SelectionGrid(new Rect(10, 0, position.width - 20, 20), _currentSection, _sections, 8);
            y += 25;

            GUI.Box(new Rect(10, y, position.width - 20, 10), string.Empty);
            y += 15;

            if (_currentSection >= 0 && _currentSection <= (int)TabType.Scores)
            {
                _treeExplorers[(TabType)_currentSection].Draw(new Rect(10, y, 250, position.height - 10));
                _propertyInspectors[(TabType)_currentSection].Draw(new Rect(270, y, position.width - 280, position.height - 10));
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_config);
            }
        }

        private GameKitConfig _config;
        private Dictionary<TabType, ItemTreeExplorer> _treeExplorers;
        private Dictionary<TabType, ItemPropertyInspector> _propertyInspectors;

        private string[] _sections;
        private int _currentSection;
    }
}
