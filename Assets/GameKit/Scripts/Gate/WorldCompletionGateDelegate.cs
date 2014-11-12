using UnityEngine;

namespace Beetle23
{
    public class WorldCompletionGateDelegate : GateDelegate
    {
        public WorldCompletionGateDelegate(Gate gate)
            : base(gate)
        {
            _world = gate.GetRelatedItem<World>();

            if (_world != null)
            {
                if (!_context.IsOpened)
                {
                    _world.OnCompleted += OnWorldCompleted;
                }
            }
            else
            {
                Debug.LogError("World completion gate [" + gate.Name + "] isn't connected with a gate!!!");
            }
        }

        public override bool CanOpenNow
        {
            get { return _world != null && _world.IsCompleted; }
        }

        public override void HandleOnOpen()
        {
            _world.OnCompleted -= OnWorldCompleted;
        }

        public override void HandleOnClose()
        {
            _world.OnCompleted += OnWorldCompleted;
        }

        private void OnWorldCompleted()
        {
            _context.ForceOpen(true);
        }

        private World _world;
    }
}