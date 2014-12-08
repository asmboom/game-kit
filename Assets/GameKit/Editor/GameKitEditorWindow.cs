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
            Missions = 3,
            Gates = 4,
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

        public string FindVirtualItemPropertyPath(VirtualItem item)
        {
            if (item is VirtualCurrency)
            {
                for (int i = 0; i < _config.VirtualCurrencies.Count; i++)
                {
                    if (_config.VirtualCurrencies[i] == item)
                    {
                        return string.Format("VirtualCurrencies.Array.data[{0}]", i);
                    }
                }
            }
            else if (item is SingleUseItem)
            {
                for (int i = 0; i < _config.SingleUseItems.Count; i++)
                {
                    if (_config.SingleUseItems[i] == item)
                    {
                        return string.Format("SingleUseItems.Array.data[{0}]", i);
                    }
                }
            }
            else if (item is LifeTimeItem)
            {
                for (int i = 0; i < _config.LifeTimeItems.Count; i++)
                {
                    if (_config.LifeTimeItems[i] == item)
                    {
                        return string.Format("LifeTimeItems.Array.data[{0}]", i);
                    }
                }
            }
            else if (item is VirtualItemPack)
            {
                for (int i = 0; i < _config.ItemPacks.Count; i++)
                {
                    if (_config.ItemPacks[i] == item)
                    {
                        return string.Format("ItemPacks.Array.data[{0}]", i);
                    }
                }
            }
            else if (item is UpgradeItem)
            {
                VirtualItem relatedItem = (item as UpgradeItem).RelatedItem;
                string path = FindVirtualItemPropertyPath(relatedItem);
                for (int i = 0; i < relatedItem.Upgrades.Count; i++)
                {
                    if (relatedItem.Upgrades[i] == item)
                    {
                        return path + string.Format(".Upgrades.Array.data[{0}]", i);
                    }
                }
            }

            Debug.LogError("FindVirtualItemPropertyPath::Code shoul never run here.");
            return string.Empty;
        }

        public string FindPurchasePropertyPath(PurchasableItem purchasableItem, Purchase purchase)
        {
            string path = FindVirtualItemPropertyPath(purchasableItem);
            for (int i = 0; i < purchasableItem.PurchaseInfo.Count; i++)
            {
                if (purchasableItem.PurchaseInfo[i] == purchase)
                {
                    return path + string.Format(".PurchaseInfo.Array.data[{0}]", i);
                }
            }

            Debug.LogError("FindPurchasePropertyPath::Code shoul never run here.");
            return string.Empty;
        }

        private void OnEnable()
        {
            _sections = new string[] { "Virtual Items", "Worlds", "Scores", "Missions", "Gates" };

            GetConfigAndCreateIfNonExist();

            if (_config == null)
            {
                _config = GameKit.Config;
            }
            if (_serializedConfig == null)
            {
                _serializedConfig = new SerializedObject(_config);
            }
            if (_treeExplorers == null)
            {
                _treeExplorers = new Dictionary<TabType, ItemTreeExplorer>();
                _treeExplorers.Add(TabType.VirtualItems, new VirtualItemsTreeExplorer(_config));
                _treeExplorers.Add(TabType.Worlds, new WorldTreeExplorer(_config));
                _treeExplorers.Add(TabType.Scores, new ScoreTreeExplorer(_config));
                _treeExplorers.Add(TabType.Missions, new MissionTreeExplorer(_config));
                _treeExplorers.Add(TabType.Gates, new GateTreeExplorer(_config));
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
                _propertyInspectors.Add(TabType.Gates,
                    new GatePropertyInspector(_treeExplorers[TabType.Gates] as GateTreeExplorer));
            }

            for (TabType type = TabType.VirtualItems; type <= TabType.Gates; type++)
            {
                _treeExplorers[type].OnSelectionChange += _propertyInspectors[type].OnExplorerSelectionChange;
            }

            VirtualItemsEditUtil.UpdateDisplayedOptions();
        }

        private void OnDisable()
        {
            if (_treeExplorers != null)
            {
                for (TabType type = TabType.VirtualItems; type <= TabType.Gates; type++)
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

            float y = 5;
            _currentSection = GUI.SelectionGrid(new Rect(10, y, position.width - 20, 20), _currentSection, _sections, 8);
            y += 25;

            GUI.Box(new Rect(10, y, position.width - 20, 10), string.Empty);
            y += 15;

            if (_currentSection >= 0 && _currentSection <= (int)TabType.Gates)
            {
                _treeExplorers[(TabType)_currentSection].Draw(new Rect(10, y, 250, position.height - y - 5));
                _propertyInspectors[(TabType)_currentSection].Draw(new Rect(270, y, position.width - 280, position.height - y - 5));
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_config);
                _serializedConfig.Update();
            }
        }

        private GameKitConfig _config;
        private SerializedObject _serializedConfig;
        private Dictionary<TabType, ItemTreeExplorer> _treeExplorers;
        private Dictionary<TabType, ItemPropertyInspector> _propertyInspectors;

        private string[] _sections;
        private int _currentSection;
    }
}
