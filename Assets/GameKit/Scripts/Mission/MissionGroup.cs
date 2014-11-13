using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Beetle23
{
    [System.Serializable]
    public class MissionGroup : Mission
    {
        public List<Mission> Missions;

        public MissionGroup()
        {
            Missions = new List<Mission>();

            if (!IsCompleted)
            {
                for (int i = 0; i < Missions.Count; i++)
                {
                    Missions[i].OnCompleted += OnSubmissionCompleted;
                }
            }
        }

        public override bool IsCompleted
        {
            get
            {
                for (int i = 0; i < Missions.Count; i++)
                {
                    if (!Missions[i].IsCompleted)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        protected override void UnregisterEvents()
        {
            for (int i = 0; i < Missions.Count; i++)
            {
                Missions[i].OnCompleted -= OnSubmissionCompleted;
            }
        }

        protected override void RegisterEvents()
        {
            for (int i = 0; i < Missions.Count; i++)
            {
                Missions[i].OnCompleted += OnSubmissionCompleted;
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
