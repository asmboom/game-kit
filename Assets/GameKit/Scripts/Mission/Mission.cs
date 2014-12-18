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
        public string Name;
        [SerializeField]
        public string Description;
        [SerializeField]
        public Texture2D BadgeIcon;
        [SerializeField]
        public List<Reward> Rewards;
        [SerializeField]
        public Gate Gate;

        public Mission()
        {
            Rewards = new List<Reward>();
            Gate = new Gate();
            if (Application.isPlaying && !IsCompleted)
            {
                Gate.OnOpened += Complete;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return MissionStorage.IsCompleted(ID);
            }
        }

        public virtual bool CanCompleteNow
        {
            get
            {
                return Gate != null && Gate.IsOpened;
            }
        }

        public void ForceCompleted(bool completed)
        {
            MissionStorage.SetCompleted(ID, completed);
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
        }

        protected virtual void UnregisterEvents()
        {
            Gate.OnOpened -= Complete;
        }

        protected virtual void RegisterEvents()
        {
            Gate.OnOpened += Complete;
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
