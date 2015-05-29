using UnityEngine;
using System;
using System.Collections;

namespace Codeplay
{
    [System.Serializable]
    public class Score : SerializableItem
    {
        public Action OnBeatRecord = delegate { };
        public Action<float, float> OnRuntimeScoreChange = delegate { };

        [SerializeField]
        public string Name;

        [SerializeField]
        public float DefaultValue = 0;

        [SerializeField]
        public bool IsHigherBetter = true;

        [SerializeField]
        public bool EnableClamp = false;

    	[SerializeField]
    	public float Min = 0;

    	[SerializeField]
    	public float Max = 999999;

        [SerializeField]
        [VirtualItemPopupAttritube(VirtualItemType.VirtualCurrency | VirtualItemType.SingleUseItem, false)]
        public string RelatedVirtualItemID;

        public float Record
        {
            get
            {
                return ScoreStorage.GetRecordScore(ID);
            }
        }

        public float RuntimeScore { get { return _runtimeScore; } }

        public VirtualItem RelatedVirtualItem
        {
            get
            {
                return string.IsNullOrEmpty(RelatedVirtualItemID) ? 
                    null : GameKit.Config.GetVirtualItemByID(RelatedVirtualItemID);
            }
        }

        public bool IsVirtualItemScore
        {
            get { return RelatedVirtualItem != null; }
        }

        public void Increase(float amount)
        {
            SetRuntimeScore(_runtimeScore + amount);
        }

        public void Decrease(float amount)
        {
            SetRuntimeScore(_runtimeScore - amount);
        }

        public void ResetRuntimeScore(bool save)
        {
            if (save)
            {
                if (IsBetterScore(_runtimeScore, ScoreStorage.GetRecordScore(ID)))
                {
                    ScoreStorage.SetRecordScore(ID, _runtimeScore);
                    _isRecordBeatenEventSent = false;
                }

                PerformSaveActions();
            }

            SetRuntimeScore(DefaultValue);
        }

        public void SetRuntimeScore(float score)
        {
            SetRuntimeScore(score, false);
        }

        public void SetRuntimeScore(float score, bool onlyIfBetter)
        {
            score = ClampScore(score);
            if (score != _runtimeScore)
            {
                bool isBetterScore = IsBetterScore(score, _runtimeScore);
                if (onlyIfBetter && !isBetterScore)
                {
                    return;
                }
                if (!_isRecordBeatenEventSent && isBetterScore)
                {
                    OnBeatRecord();
                    _isRecordBeatenEventSent = true;
                }
                float oldScore = _runtimeScore;
                _runtimeScore = score;

                OnRuntimeScoreChange(oldScore, _runtimeScore);
            }
        }

        public bool IsBetterScore(float srcScore, float destScore)
        {
            return IsHigherBetter ? (srcScore > destScore) : (srcScore < destScore);
        }

        public bool HasReachedScore(float srcScore, float destScore)
        {
            return IsHigherBetter ? (srcScore >= destScore) : (srcScore <= destScore);
        }

        private float ClampScore(float score)
        {
            return EnableClamp ? Mathf.Clamp(score, Min, Max) : score;
        }

        private void PerformSaveActions() 
        {
            if (RelatedVirtualItem != null)
            {
                RelatedVirtualItem.Give((int)RuntimeScore);
            }
        }

        private float _runtimeScore;
        private bool _isRecordBeatenEventSent = false;
    }
}