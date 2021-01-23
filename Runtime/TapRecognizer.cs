using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lab5Games.LTouch
{
    public class TapRecognizer : GestureRecognizer
    {
        private int _numTapsRequired = 1;
        private int _numTouchesRequired = 1;
        private float _maxDruationForTaps = 0;

        private float _touchBeganTime = 0;
        private int _prformedTapsCount = 0;

        public event Action<TapRecognizer> onRecognized;

        public TapRecognizer() : this(1, 1, 1f)
        {

        }

        public TapRecognizer(int touchesRequired, int tapsRequried, float maxDurationForTaps)
        {
            _numTouchesRequired = Mathf.Max(1, touchesRequired);
            _numTapsRequired = Mathf.Max(1, tapsRequried);
            _maxDruationForTaps = maxDurationForTaps;
        }

        public override void OnTriggerEvents()
        {
            onRecognized?.Invoke(this);
        }

        public override bool OnTouchBegan(List<UTouch> touches)
        {
            if (state == EState.Ready)
            {
                for (int i = 0; i < touches.Count; i++)
                {
                    if (touches[i].phase == TouchPhase.Began)
                    {
                        _tracking.Add(touches[i]);

                        if (_tracking.Count == _numTapsRequired)
                            break;
                    }
                }

                if (_tracking.Count == _numTouchesRequired)
                {
                    _touchBeganTime = Time.realtimeSinceStartup;
                    _prformedTapsCount = 0;
                    state = EState.Began;

                    return true;
                }
            }

            return false;
        }

        public override void OnTouchEnded(List<UTouch> touches)
        {
            if(state == EState.Began && (Time.realtimeSinceStartup <= (_touchBeganTime + _maxDruationForTaps)))
            {
                ++_prformedTapsCount;

                if(_prformedTapsCount == _numTapsRequired)
                {
                    state = EState.Recognized;
                }
            }
            else
            {
                state = EState.FailedOrEnded;
            }
        }
    }
}
