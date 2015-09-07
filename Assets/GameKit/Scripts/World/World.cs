using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Codeplay
{
    [System.Serializable]
    public class World : SerializableItem
    {
        public Action OnUnlocked = delegate { };
        public Action OnCompleted = delegate { };

        [SerializeField]
        public string Name;
        
        [SerializeField]
        public string Description;
        
        [SerializeField]
        public Gate Gate;

		[SerializeField]
		public List<string> SubWorldsID;

		[SerializeField]
        public List<Score> Scores;

		[SerializeField]
        public List<Mission> Missions;

        [SerializeField]
        public ScriptableObject Extend;

		[NonSerialized]
		public List<World> SubWorlds;

        public World()
		{
			Scores = new List<Score>();
			Missions = new List<Mission>();
			SubWorldsID = new List<string>();
			SubWorlds = new List<World>();
            Gate = new Gate();
            if (Application.isPlaying && !IsUnlocked)
            {
                Gate.OnOpened += OnUnlocked;
            }
        }

        public World Parent { get; internal set; }

		public void RefreshSubWorlds()
		{
			SubWorlds.Clear();
			for (int i = 0; i < SubWorldsID.Count; i++)
			{
				SubWorlds.Add(GameKit.Config.GetWorldByID(SubWorldsID[i]));
			}
		}

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
                return WorldStorage.IsUnlocked(ID);
            }
        }

        public bool CanUnlockNow
        {
            get
            {
                return Gate == null || Gate.IsOpened;
            }
        }

        public void ForceUnlocked(bool unlocked)
        {
            WorldStorage.SetUnlocked(ID, unlocked);
            if (unlocked)
            {
                Gate.OnOpened -= OnUnlocked;
                OnUnlocked();
            }
            else
            {
                Gate.OnOpened += OnUnlocked;
            }
        }

        public void Complete(bool recursive = false)
        {
            SetCompleted(true, recursive);
        }

        public void RevokeComplete(bool resursive = false)
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
				foreach (var subWorldID in SubWorldsID) 
                {
					GameKit.Config.GetWorldByID(subWorldID).SetCompleted(completed, true);
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
