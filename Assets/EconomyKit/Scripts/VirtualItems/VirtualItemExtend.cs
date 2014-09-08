using UnityEngine;

public abstract class VirtualItemExtend : ScriptableObject
{
#if UNITY_EDITOR
	public abstract float DrawInspector(Rect rect);
#endif
}