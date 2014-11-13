using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Beetle23
{
    [System.Serializable]
    public class Mission : SerializableItem
    {
        public Action OnCompleted = delegate { };

        [SerializeField]
        public List<Reward> Rewards;
        [SerializeField]
        public Gate RelatedGate;

        public Mission()
        {
            Rewards = new List<Reward>();
            RelatedGate = new Gate()
            {
                ID = "gate_" + this.ID
            };
            if (!RelatedGate.IsOpened)
            {
                RelatedGate.OnOpened += OnGateOpened;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return RelatedGate != null && RelatedGate.IsOpened;
            }
        }

        public void ForceCompleted(bool completed)
        {
            RelatedGate.ForceOpen(completed);
            if (completed)
            {
                RelatedGate.OnOpened -= OnGateOpened;
                GiveRewards();
                OnCompleted();
            }
            else
            {
                TakeRewards();
                RelatedGate.OnOpened += OnGateOpened;
            }
        }

        private void OnGateOpened()
        {
            GiveRewards();
        }

        private void TakeRewards()
        {
            foreach (var reward in Rewards)
            {
                reward.Take();
            }
        }

        private void GiveRewards()
        {
            foreach (var reward in Rewards)
            {
                reward.Give();
            }
        }
    }
}
