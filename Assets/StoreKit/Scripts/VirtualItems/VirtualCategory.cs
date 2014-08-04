using System.Collections.Generic;
using UnityEngine;

public class VirtualCategory : ScriptableObject
{
    [SerializeField]
    public string ID;

    [SerializeField]
    public List<VirtualItem> Items;
}
