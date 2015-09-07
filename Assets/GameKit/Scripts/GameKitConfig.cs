using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Codeplay
{
    public class GameKitConfig : ScriptableObject
    {
        [SerializeField]
        public List<VirtualCurrency> VirtualCurrencies;

        [SerializeField]
        public List<SingleUseItem> SingleUseItems;

        [SerializeField]
        public List<LifeTimeItem> LifeTimeItems;

        [SerializeField]
        public List<VirtualItemPack> ItemPacks;

        [SerializeField]
        public List<VirtualCategory> Categories;

		[SerializeField]
		public List<UpgradeItem> Upgrades;

		[SerializeField]
		public List<World> Worlds;

		[SerializeField]
		public List<Gate> SubGates;

        public World RootWorld
		{
			get
			{
				if (Worlds == null)
				{
					Worlds = new List<World>();
				}
				if (Worlds.Count == 0)
				{
					Worlds.Add(new World());
					Worlds[0].ID = "root";
				}

				return Worlds[0];
			}
		}

        public IEnumerable<VirtualItem> VirtualItems
        {
            get
            {
                return IdToVirtualItem.Values;
            }
        }

        public int VirtualItemsCount
        {
            get { return IdToVirtualItem.Count; }
        }

		public void ClearVirtualItems()
		{
			ItemPacks.Clear();
			Categories.Clear();
			LifeTimeItems.Clear();
			SingleUseItems.Clear();
			VirtualCurrencies.Clear();
			Upgrades.Clear();
		}

		public bool TryGetCategoryByCategoryID(string id, out VirtualCategory cateroty)
		{
			for (int i = 0; i < Categories.Count; i++)
			{
				if (Categories[i].ID.Equals(id))
				{
					cateroty = Categories[i];
					return true;
				}
			}
			cateroty = null;
			return false;
		}

		public bool TryGetCategoryByID(string id, out VirtualCategory category)
		{
			return IdToCategory.TryGetValue(id, out category);
		}

        public bool TryGetVirtualItemByID(string id, out VirtualItem item)
        {
            return IdToVirtualItem.TryGetValue(id, out item);
        }

        public VirtualItem GetVirtualItemByID(string id)
        {
            if (IdToVirtualItem.ContainsKey(id))
            {
                return IdToVirtualItem[id];
            }
            else
            {
                return null;
            }
        }

        public VirtualCategory GetCategoryByID(string id)
        {
            foreach (var category in Categories)
            {
                if (category.ID.Equals(id))
                {
                    return category;
                }
            }
            return null;
        }

        public World GetWorldByID(string id)
        {
			return IdToWorld.ContainsKey(id) ? IdToWorld[id] : null;
        }

        public Score GetScoreByID(string id)
        {
            return IdToScore.ContainsKey(id) ? IdToScore[id] : null;
        }

		public Gate GetSubGateByID(string id)
		{
			return IdToSubGate.ContainsKey(id) ? IdToSubGate[id] : null;
		}

        public VirtualCategory GetItemCategory(string id)
        {
            return IdToCategory.ContainsKey(id) ? IdToCategory[id] : null;
        }

        public World FindWorldThatScoreBelongsTo(Score score)
        {
            foreach (var world in Worlds)
            {
                foreach (var oneScore in world.Scores)
                {
                    if (oneScore == score)
                    {
                        return world;
                    }
                }
            }
            return null;
        }

        public World FindWorldThatMissionBelongsTo(Mission mission)
        {
            foreach (var world in Worlds)
            {
                foreach (var oneMission in world.Missions)
                {
                    if (oneMission == mission)
                    {
                        return world;
                    }
                }
            }
            return null;
        }

        public void UpdateMapsAndTree()
        {
            UpdateIdToVirtualItemMap();
            UpdateIdToCategoryMap();
            UpdateIdToWorldAndScoreMap();
            UpdateWorldTree();
			UpdateIdToGateMap();
        }

        public void RemoveNullRefs()
        {
            for (int i = 0; i < VirtualCurrencies.Count; i++)
            {
                if (VirtualCurrencies[i] == null)
                {
                    VirtualCurrencies.RemoveAt(i);
                }
            }
            for (int i = 0; i < SingleUseItems.Count; i++)
            {
                if (SingleUseItems[i] == null)
                {
                    SingleUseItems.RemoveAt(i);
                }
            }
            for (int i = 0; i < LifeTimeItems.Count; i++)
            {
                if (LifeTimeItems[i] == null)
                {
                    LifeTimeItems.RemoveAt(i);
                }
            }
            for (int i = 0; i < ItemPacks.Count; i++)
            {
                if (ItemPacks[i] == null)
                {
                    ItemPacks.RemoveAt(i);
                }
            }
            for (int i = 0; i < Categories.Count; i++)
            {
                if (Categories[i] == null)
                {
                    Categories.RemoveAt(i);
                }
            }
        }

        private void OnEnable()
        {
            if (VirtualCurrencies == null)
            {
                VirtualCurrencies = new List<VirtualCurrency>();
            }
            if (SingleUseItems == null)
            {
                SingleUseItems = new List<SingleUseItem>();
            }
            if (LifeTimeItems == null)
            {
                LifeTimeItems = new List<LifeTimeItem>();
            }
            if (ItemPacks == null)
            {
                ItemPacks = new List<VirtualItemPack>();
            }
            if (Categories == null)
            {
                Categories = new List<VirtualCategory>();
            }
			if (Upgrades == null)
			{
				Upgrades = new List<UpgradeItem>();
			}
			if (Worlds == null)
            {
				Worlds = new List<World>();
            }
			if (SubGates == null)
			{
				SubGates = new List<Gate>();
			}

            UpdateMapsAndTree();
        }

        private void UpdateIdToVirtualItemMap()
        {
            _idToVirtualItem = new Dictionary<string, VirtualItem>();
            for (int i = 0; i < VirtualCurrencies.Count; i++)
            {
                TryAddToIdItemMap(VirtualCurrencies[i].ID, VirtualCurrencies[i]);
            }
            for (int i = 0; i < SingleUseItems.Count; i++)
            {
                TryAddToIdItemMap(SingleUseItems[i].ID, SingleUseItems[i]);
            }
            for (int i = 0; i < LifeTimeItems.Count; i++)
            {
                TryAddToIdItemMap(LifeTimeItems[i].ID, LifeTimeItems[i]);
            }
            for (int i = 0; i < ItemPacks.Count; i++)
            {
                TryAddToIdItemMap(ItemPacks[i].ID, ItemPacks[i]);
            }
        }

        private void UpdateIdToWorldAndScoreMap()
        {
            _idToWorld = new Dictionary<string, World>();
            _idToScore = new Dictionary<string, Score>();

			for (int i = 0; i < Worlds.Count; i++)
			{
				_idToWorld.Add(Worlds[i].ID, Worlds[i]);
				foreach (var score in Worlds[i].Scores)
				{
					_idToScore.Add(score.ID, score);
				}
			}
        }

        private void UpdateWorldTree()
        {
			LoopThroughWorld(RootWorld, (world) =>
            {	
				foreach (var subWorldID in world.SubWorldsID)
                {
					GetWorldByID(subWorldID).Parent = world;
                }
            });
        }

        private void UpdateIdToCategoryMap()
        {
            _idToCategory = new Dictionary<string, VirtualCategory>();
            for (int i = 0; i < Categories.Count; i++)
            {
                foreach (var itemID in Categories[i].ItemIDs)
                {
                    if (itemID != null && GetVirtualItemByID(itemID) != null)
                    {
                        _idToCategory.Add(itemID, Categories[i]);
                    }
                }
            }
        }

		private void UpdateIdToGateMap()
		{
			_idToSubGate = new Dictionary<string, Gate>();
			for (int i = 0; i < SubGates.Count; i++)
			{
				_idToSubGate.Add(SubGates[i].ID, SubGates[i]);
			}
		}

        private void LoopThroughWorld(World world, System.Action<World> action)
        {
            action(world);
			foreach (var subWorldID in world.SubWorldsID)
            {
				LoopThroughWorld(GetWorldByID(subWorldID), action);
            }
        }

        private void TryAddToIdItemMap(string id, VirtualItem item)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!_idToVirtualItem.ContainsKey(id))
                {
                    _idToVirtualItem.Add(id, item);
                }
                else
                {
                    Debug.LogWarning("Found duplicated id " + id + " for item."); ;
                }
            }
        }

        private Dictionary<string, VirtualItem> IdToVirtualItem
        {
            get
            {
                if (_idToVirtualItem == null)
                {
                    UpdateIdToVirtualItemMap();
                }
                return _idToVirtualItem;
            }
        }

        private Dictionary<string, VirtualCategory> IdToCategory
        {
            get
            {
                if (_idToCategory == null)
                {
                    UpdateIdToCategoryMap();
                }
                return _idToCategory;
            }
        }

        private Dictionary<string, World> IdToWorld
        {
            get
            {
                if (_idToWorld == null)
                {
                    UpdateIdToWorldAndScoreMap();
                }
                return _idToWorld;
            }
        }

        private Dictionary<string, Score> IdToScore
        {
            get
            {
                if (_idToScore == null)
                {
                    UpdateIdToWorldAndScoreMap();
                }
                return _idToScore;
            }
        }

		private Dictionary<string, Gate> IdToSubGate
		{
			get
			{
				if (_idToSubGate == null)
				{
					UpdateIdToGateMap();
				}
				return _idToSubGate;
			}
		}

        private Dictionary<string, VirtualItem> _idToVirtualItem;
        private Dictionary<string, VirtualCategory> _idToCategory;
        private Dictionary<string, World> _idToWorld;
        private Dictionary<string, Score> _idToScore;
		private Dictionary<string, Gate> _idToSubGate;
    }
}
