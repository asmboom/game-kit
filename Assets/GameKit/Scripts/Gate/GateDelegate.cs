namespace Beetle23
{
    public abstract class GateDelegate : ItemDelegate<Gate>
    {
        public GateDelegate(Gate gate)
            : base(gate)
        { }

        public abstract bool CanOpenNow { get; }
        public abstract void RegisterEvents();
        public abstract void UnregisterEvents();

        public virtual bool TryOpen()
        {
            if (_context.CanOpenNow)
            {
                _context.ForceOpen(true);
                return true;
            }

            return false;
        }
    }
}
