using UnityEngine;
using System;
using System.Collections;

namespace Beetle23
{
    public class ScoreGateDelegate : GateDelegate
    {
        public ScoreGateDelegate(Gate gate)
            : base(gate)
        {
            _requiredScoreNumber = gate.RelatedNumber;
            _score = gate.GetRelatedItem<Score>();

            if (_score != null)
            {
                if (!_context.IsOpened)
                {
                    _score.OnBeatRecord += OnBeatRecord;
                }
            }
            else
            {
                Debug.LogError("Score gate [" + gate.Name + "] isn't connected with a score!!!");
            }
        }

        public override bool CanOpenNow
        {
            get
            {
                return _score != null &&
                       _score.HasReachedScore(_score.Record, _requiredScoreNumber);
            }
        }

        public override void HandleOnOpen()
        {
            _score.OnBeatRecord -= OnBeatRecord;
        }

        public override void HandleOnClose()
        {
            _score.OnBeatRecord += OnBeatRecord;
        }

        private void OnBeatRecord()
        {
            if (_context.CanOpenNow)
            {
                _context.ForceOpen(true);
            }
        }

        private Score _score;
        private float _requiredScoreNumber;
    }
}
