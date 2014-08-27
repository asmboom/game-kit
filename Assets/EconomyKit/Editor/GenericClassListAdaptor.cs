using UnityEngine;
using System.Collections.Generic;
using Rotorz.ReorderableList;

public class GenericListAdaptorDelegate
{
    public delegate T ItemCreator<T>();
    public delegate T ClassItemDrawer<T>(Rect position, T item, int index);
    public delegate float ItemHeightGetter<T>(T item);

    public static T DefaultClassItemDrawer<T>(Rect position, T item, int index)
    {
        GUI.Label(position, "Item drawer not implemented.");
        return item;
    }
}

public class GenericClassListAdaptor<T> : IReorderableListAdaptor where T : class
{
    public float FixedItemHeight { get; private set; }

    public IList<T> List
    {
        get { return _list; }
    }

    public T this[int index]
    {
        get { return _list[index]; }
    }

    public GenericClassListAdaptor(IList<T> list, float itemHeight,
        GenericListAdaptorDelegate.ItemCreator<T> itemCreator, 
        GenericListAdaptorDelegate.ClassItemDrawer<T> itemDrawer,
        GenericListAdaptorDelegate.ItemHeightGetter<T> itemHeightGetter = null)
    {
        this._list = list;
        this.FixedItemHeight = itemHeight;
        this._itemCreator = itemCreator;
        this._itemDrawer = itemDrawer ?? GenericListAdaptorDelegate.DefaultClassItemDrawer;
        this._itemHeightGetter = itemHeightGetter;
    }

    #region IReorderableListAdaptor - Implementation

    public int Count
    {
        get { return _list.Count; }
    }

    public virtual bool CanDrag(int index)
    {
        return true;
    }

    public virtual bool CanRemove(int index)
    {
        return true;
    }

    public void Add()
    {
        _list.Add(Create());
    }

    public void Insert(int index)
    {
        _list.Insert(index, Create());
    }

    public void Duplicate(int index)
    {
        _list.Insert(index + 1, _list[index]);
    }

    public void Remove(int index)
    {
        _list.RemoveAt(index);
    }

    public void Move(int sourceIndex, int destIndex)
    {
        if (destIndex > sourceIndex)
            --destIndex;

        T item = _list[sourceIndex];
        _list.RemoveAt(sourceIndex);
        _list.Insert(destIndex, item);
    }

    public void Clear()
    {
        _list.Clear();
    }

    public void DrawItem(Rect position, int index)
    {
        _list[index] = _itemDrawer(position, _list[index], index);
    }

    public virtual float GetItemHeight(int index)
    {
        return _itemHeightGetter == null ? FixedItemHeight : _itemHeightGetter(_list[index]);
    }

    private T Create()
    {
        return _itemCreator != null ? _itemCreator() : default(T);
    }

    #endregion

    private IList<T> _list;
    private GenericListAdaptorDelegate.ClassItemDrawer<T> _itemDrawer;
    private GenericListAdaptorDelegate.ItemCreator<T> _itemCreator;
    private GenericListAdaptorDelegate.ItemHeightGetter<T> _itemHeightGetter;

}