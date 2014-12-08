namespace Beetle23
{
    [System.Flags]
    public enum VirtualItemType
    {
        None = 1 << 0,

        VirtualCurrency = 1 << 1,
        LifeTimeItem = 2 << 2,
        SingleUseItem = 2 << 3,
        VirtualItemPack = 2 << 4,
    }
}