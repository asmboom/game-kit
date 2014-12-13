using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Beetle23
{
    [System.Serializable]
    public class Challenge : Mission
    {
        [SerializeField]
        public List<Mission> SubMissions;

        public Challenge()
        {
            SubMissions = new List<Mission>();

            if (!IsCompleted)
            {
                for (int i = 0; i < SubMissions.Count; i++)
                {
                    SubMissions[i].OnCompleted += OnSubmissionCompleted;
                }
            }
        }

        public override bool IsCompleted
        {
            get
            {
                for (int i = 0; i < SubMissions.Count; i++)
                {
                    if (!SubMissions[i].IsCompleted)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        protected override void UnregisterEvents()
        {
            for (int i = 0; i < SubMissions.Count; i++)
            {
                SubMissions[i].OnCompleted -= OnSubmissionCompleted;
            }
        }

        protected override void RegisterEvents()
        {
            for (int i = 0; i < SubMissions.Count; i++)
            {
                SubMissions[i].OnCompleted += OnSubmissionCompleted;
            }
        }

        private void OnSubmissionCompleted()
        {
            if (IsCompleted)
            {
                Complete();
            }
        }
    }
}
