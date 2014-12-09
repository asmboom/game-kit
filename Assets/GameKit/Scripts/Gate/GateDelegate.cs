namespace Beetle23
{
    public abstract class GateDelegate : ItemDelegate<Gate>
    {
        public GateDelegate(Gate gate)
            : base(gate)
        { }

        public abstract bool CanOpenNow { get; }
        public abstract IItem GetRelatedItem(string itemId);
        public abstract void RegisterEvents();
        public abstract void UnregisterEvents();
    }
}
