using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Beetle23
{
    [System.Serializable]
    public class World : SerializableItem
    {
        public Action OnCompleted = delegate { };

        [SerializeField]
        public string Description;
        
    	[SerializeField]
    	public GateGroup Gate;

        [SerializeField]
        public List<World> SubWorlds;

        [SerializeField]
        public List<Score> Scores;

        [SerializeField]
        public List<Mission> Missions;

        [SerializeField]
        public ScriptableObject Extend;

        public World()
        {
            Gate = new GateGroup();
            SubWorlds = new List<World>();
            Scores = new List<Score>();
            Missions = new List<Mission>();
        }

        public World Parent { get; internal set; }

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

        public void Complete(bool recursive)
        {
            SetCompleted(true, recursive);
        }

        public void RevokeComplete(bool resursive)
        {
            SetCompleted(false, resursive);
        }

        public void ResetRuntimeScores(bool save)
        {
            for (int i = 0; i < Scores.Count; i++)
            {
                Scores[i].ResetRuntimeScore(save);
            }
        }

        public T GetExtend<T>() where T : ScriptableObject
        {
            return Extend as T;
        }

        private void SetCompleted(bool completed, bool recursive) 
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
