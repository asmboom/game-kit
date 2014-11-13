using UnityEngine;
using System;
using System.Collections;

namespace Beetle23
{
    public class Score : ScriptableItem
    {
        public Action OnBeatRecord = delegate { };
        public Action<float, float> OnRuntimeScoreChange = delegate { };

        [SerializeField]
        public float DefaultValue = 0;

        [SerializeField]
        public bool IsHigherBetter = true;

        public float Record
        {
            get
            {
                return ScoreStorage.GetRecordScore(ID);
            }
        }

        public float RuntimeScore { get { return _runtimeScore; } }

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

        protected virtual float ClampScore(float score)
        {
            return score;
        }

        protected virtual void PerformSaveActions() { }

        private float _runtimeScore;
        private bool _isRecordBeatenEventSent = false;
    }
}