public static class VirtualItemDelegateFactory
{
    public static VirtualItemDelegate Create(VirtualItemType type)
    {
        switch (type)
        {
            case VirtualItemType.VirtualCurrency:
                return new VirtualCurrencyDelegate();
            case VirtualItemType.VirtualCurrencyPack:
                return new VirtualCurrencyPackDelegate();
            case VirtualItemType.ConsumableItem:
                return new ConsumableDelegate();
            case VirtualItemType.ConsumablePack:
                return new ConsumablePackDelegate();
            case VirtualItemType.NonConsumableItem:
                return new NonConsumableDelegate();
            case VirtualItemType.EquippableItem:
                return new EquippableItemDelegate();
            case VirtualItemType.UpgradeItem:
                return new UpgradeItemDelegate();
            default:
                return new NullableVirtualItemDelegate();
        }
    }
}