using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Beetle23
{
    public class World : ScriptableItem
    {
        public Action OnCompleted = delegate { };

    	[SerializeField]
    	public Gate Gate;

        [SerializeField]
        public List<World> SubWorlds;

        [SerializeField]
        public List<Score> Scores;

        [SerializeField]
        public List<Mission> Missions;

    	public bool IsCompleted
    	{
    		get
    		{
                return WorldStorage.IsCompleted(ID);
	    	}
    	}

        public bool IsUnlocked
        {
            get
            {
                return Gate == null || Gate.IsOpened;
            }
        }

        public void SetCompleted(bool completed) 
        {
            SetCompleted(completed, false);
        }

        public void SetCompleted(bool completed, bool recursive) 
        {
            if (recursive) 
            {
                foreach (World world in SubWorlds) 
                {
                    world.SetCompleted(completed, true);
                }
            }
            WorldStorage.SetCompleted(ID, completed);
            if (completed)
            {
                OnCompleted();
            }
        }
    }
}
