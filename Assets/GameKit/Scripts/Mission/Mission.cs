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
        [GatePopup(true, true)]
        public string RelatedGateID;

        public Mission()
        {
            Rewards = new List<Reward>();
            if (RelatedGate != null && !IsCompleted)
            {
                RelatedGate.OnOpened += Complete;
            }
        }

        public Gate RelatedGate
        {
            get
            {
                return string.IsNullOrEmpty(RelatedGateID) ? null : GameKit.Config.GetGateByID(RelatedGateID);
            }
        }

        public virtual bool IsCompleted
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
                Complete();
            }
            else
            {
                RevokeComplete();
            }
        }

        protected void Complete()
        {
            GiveRewards();
            UnregisterEvents();
            OnCompleted();
        }

        protected void RevokeComplete()
        {
            TakeRewards();
            RegisterEvents();
            RelatedGate.OnOpened += Complete;
        }

        protected virtual void UnregisterEvents()
        {
            RelatedGate.OnOpened -= Complete;
        }

        protected virtual void RegisterEvents()
        {
            RelatedGate.OnOpened += Complete;
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
