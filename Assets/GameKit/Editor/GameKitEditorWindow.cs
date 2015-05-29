using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Codeplay
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
            Missions = 3,
        }

        public static GameKitEditorWindow GetInstance()
        {
            if (_instance == null)
            {
                _instance = EditorWindow.GetWindow<GameKitEditorWindow>("GameKit");
            }
            return _instance;
        }

        public static SerializedObject SerializedConfig
        {
            get
            {
                if (GetInstance()._serializedConfig == null)
                {
                    GetInstance()._serializedConfig = new SerializedObject(GameKit.Config);
                }
                return GetInstance()._serializedConfig;
            }
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

        public ItemPropertyInspector GetPropertyInsepctor(TabType tabtype)
        {
            if (_propertyInspectors.ContainsKey(tabtype))
            {
                return _propertyInspectors[tabtype];
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

        public string FindWorldPropertyPath(World worldToFind)
        {
            if (worldToFind == null) return string.Empty;

            World world = worldToFind;
            string path = string.Empty;
            while (world.Parent != null)
            {
                for (int i = 0; i < world.Parent.SubWorlds.Count; i++)
                {
                    if (world.Parent.SubWorlds[i] == world)
                    {
                        path = string.Format(".SubWorlds.Array.data[{0}]", i) + path;
                        break;
                    }
                }
                world = world.Parent;
            }
            return "RootWorld" + path;
        }

        public string FindScorePropertyPath(Score score)
        {
            World world = GameKit.Config.FindWorldThatScoreBelongsTo(score);
            if (world == null) return string.Empty;

            string path = GameKitEditorWindow.GetInstance().FindWorldPropertyPath(world);
            for (int i = 0; i < world.Scores.Count; i++)
            {
                if (world.Scores[i] == score)
                {
                    return path + string.Format(".Scores.Array.data[{0}]", i);
                }
            }

            Debug.LogError("FindScorePropertyPath::Code shoul never run here.");
            return string.Empty;
        }

        private void OnEnable()
        {
            _sections = new string[] { "Virtual Items", "Worlds", "Scores", "Missions" };

            GetConfigAndCreateIfNonExist();

            if (_treeExplorers == null)
            {
                _treeExplorers = new Dictionary<TabType, ItemTreeExplorer>();
                _treeExplorers.Add(TabType.VirtualItems, new VirtualItemsTreeExplorer());
                _treeExplorers.Add(TabType.Worlds, new WorldTreeExplorer());
                _treeExplorers.Add(TabType.Scores, new ScoreTreeExplorer());
                _treeExplorers.Add(TabType.Missions, new MissionTreeExplorer());
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
                _propertyInspectors.Add(TabType.Missions, 
                    new MissionPropertyInspector(_treeExplorers[TabType.Missions] as MissionTreeExplorer));
            }

            for (TabType type = TabType.VirtualItems; type <= TabType.Missions; type++)
            {
                _treeExplorers[type].OnSelectionChange += _propertyInspectors[type].OnExplorerSelectionChange;
            }
        }

        private void OnDisable()
        {
            if (_treeExplorers != null)
            {
                for (TabType type = TabType.VirtualItems; type <= TabType.Missions; type++)
                {
                    _treeExplorers[type].OnSelectionChange -= _propertyInspectors[type].OnExplorerSelectionChange;
                }
            }
        }

        private void OnFocus()
        {
            if (GameKit.Config != null)
            {
                GameKit.Config.RemoveNullRefs();
                GameKit.Config.UpdateMapsAndTree();
            }
        }

        private static GameKitConfig GetConfigAndCreateIfNonExist()
        {
            string configFilePath = GameKit.DefaultConfigDataPath + "/GameKitConfig.asset";
            GameKitConfig virtualItemsConfig = AssetDatabase.LoadAssetAtPath(configFilePath, typeof(GameKitConfig)) as GameKitConfig;
            if (virtualItemsConfig == null)
            {
                virtualItemsConfig = CreateAsset<GameKitConfig>(configFilePath);
            }
            return virtualItemsConfig;
        }

        private static T CreateAsset<T>(string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return asset;
        }

        private void OnGUI()
        {
            if (GameKit.Config == null) return;

            EditorGUI.BeginChangeCheck();

            float y = 5;
            _currentSection = GUI.SelectionGrid(new Rect(10, y, position.width - 20, 20), _currentSection, _sections, 8);
            y += 25;

            float separatorHeight = 10;
            GUI.BeginGroup(new Rect(10, y, position.width - 20, separatorHeight), string.Empty, GUI.skin.GetStyle("Box"));
            y += separatorHeight + 5;
            GUI.EndGroup();

            float treeRatio = 0.35f;
            if (_currentSection >= 0 && _currentSection <= (int)TabType.Missions)
            {
                _treeExplorers[(TabType)_currentSection].Draw(new Rect(10, y, position.width * treeRatio, position.height - y - 5));
                _propertyInspectors[(TabType)_currentSection].Draw(new Rect(position.width * treeRatio + 20, y, position.width * (1 - treeRatio) - 30, position.height - y - 5));
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(GameKit.Config);
                SerializedConfig.Update();
            }
        }

        private SerializedObject _serializedConfig;
        private Dictionary<TabType, ItemTreeExplorer> _treeExplorers;
        private Dictionary<TabType, ItemPropertyInspector> _propertyInspectors;

        private string[] _sections;
        private int _currentSection;
    }
}
