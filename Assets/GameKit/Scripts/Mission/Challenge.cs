using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Codeplay
{
    [System.Serializable]
    public class Challenge : SerializableItem
    {
        [SerializeField]
        public Texture2D BadgeIcon;
        [SerializeField]
        public List<Reward> Rewards;
        [SerializeField]
        public List<Mission> Missions;

        public Challenge()
        {
            Missions = new List<Mission>();
        }

        public bool IsCompleted
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
    }
}
