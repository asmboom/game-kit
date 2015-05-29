using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Codeplay
{
    public abstract class ItemPopupDrawerDelegate
    {
        public abstract void Init(object param);
        public abstract void InsertIDs(List<string> itemIDs);
    }

    public class VirtualItemPopupDrawerDelegate : ItemPopupDrawerDelegate
    {
        public override void Init(object param)
        {
            _virtualItemType = (VirtualItemType)param;
        }

        public override void InsertIDs(List<string> itemIDs)
        {
            if ((_virtualItemType & VirtualItemType.VirtualCurrency) != 0)
            {
                foreach (var item in GameKit.Config.VirtualCurrencies)
                {
                    itemIDs.Add(item.ID);
                }
            }
            if ((_virtualItemType & VirtualItemType.SingleUseItem) != 0)
            {
                foreach (var item in GameKit.Config.SingleUseItems)
                {
                    itemIDs.Add(item.ID);
                }
            }
            if ((_virtualItemType & VirtualItemType.LifeTimeItem) != 0)
            {
                foreach (var item in GameKit.Config.LifeTimeItems)
                {
                    itemIDs.Add(item.ID);
                }
            }
            if ((_virtualItemType & VirtualItemType.VirtualItemPack) != 0)
            {
                foreach (var item in GameKit.Config.ItemPacks)
                {
                    itemIDs.Add(item.ID);
                }
            }
        }

        private VirtualItemType _virtualItemType;
    }

    public class WorldPopupDrawerDelegate : ItemPopupDrawerDelegate
    {
        public override void Init(object param)
        {
        }

        public override void InsertIDs(List<string> itemIDs)
        {
            InsertWorld(itemIDs, GameKit.Config.RootWorld);
        }

        private void InsertWorld(List<string> itemIDs, World world)
        {
            itemIDs.Add(world.ID);
            foreach (var subworld in world.SubWorlds)
            {
                InsertWorld(itemIDs, subworld);
            }
        }
    }

    public class ScorePopupDrawerDelegate : ItemPopupDrawerDelegate
    {
        public override void Init(object param)
        {
        }

        public override void InsertIDs(List<string> itemIDs)
        {
            InsertScore(itemIDs, GameKit.Config.RootWorld);   
        }

        private void InsertScore(List<string> itemIDs, World world)
        {
            foreach (var score in world.Scores)
            {
                itemIDs.Add(score.ID);
            }
            foreach (var subworld in world.SubWorlds)
            {
                InsertScore(itemIDs, subworld);
            }
        }
    }

    public class ItemPopupDrawer
    {
        public ItemPopupDrawer(ItemType itemType, bool allowNone, object param)
        {
            _allowNone = allowNone;
            _itemIDs = new List<string>();
            _delegate = CreateDelegate(itemType);
            _delegate.Init(param);
        }

        public string Draw(Rect position, string value, GUIContent label)
        {
            string[] itemIDs = GetItemIDs();
            if (itemIDs.Length == 0)
            {
                EditorGUI.LabelField(position, label, new GUIContent(None));
                return string.Empty;
            }
            if (!string.IsNullOrEmpty(value))
            {
                _selectedValue = GetIndex(itemIDs, value);
            }
            _selectedValue = EditorGUI.Popup(position, label.text, _selectedValue, itemIDs);
            if (_allowNone && _selectedValue == 0)
            {
                return string.Empty;
            }
            else
            {
                return itemIDs[_selectedValue];
            }
        }

        private int GetIndex(string[] itemIDs, string itemID)
        {
            if (_allowNone && string.IsNullOrEmpty(itemID))
            {
                return 0;
            }

            int result = 0;
            for (int i = 0; i < itemIDs.Length; i++)
            {
                if (itemID == itemIDs[i])
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        private string[] GetItemIDs()
        {
            _itemIDs.Clear();
            if (_allowNone)
            {
                _itemIDs.Add(None);
            }
            _delegate.InsertIDs(_itemIDs);
            return _itemIDs.ToArray();
        }

        private ItemPopupDrawerDelegate CreateDelegate(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.VirtualItem:
                    return new VirtualItemPopupDrawerDelegate();
                case ItemType.Score:
                    return new ScorePopupDrawerDelegate();
                case ItemType.World:
                    return new WorldPopupDrawerDelegate();
                default:
                    return null;
            }
        }

        private List<string> _itemIDs;
        private bool _allowNone = true;
        private int _selectedValue = 0;
        private ItemPopupDrawerDelegate _delegate;

        private const string None = "none";
    }
}