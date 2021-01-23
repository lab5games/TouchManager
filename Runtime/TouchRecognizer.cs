using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lab5Games.LTouch
{
    public class TouchRecognizer : GestureRecognizer
    {
        private float _startTime;

        public Vector2 position { get; private set; }
        public float elpasedTime { get; private set; }

        public event Action<TouchRecognizer> onBegin;
        public event Action<TouchRecognizer> onEnd;
        public event Action<TouchRecognizer> onMove;
        public event Action<TouchRecognizer> onStay;

        public TouchRecognizer()
        {

        }

        public override bool OnTouchBegan(List<UTouch> touches)
        {
            if(state == EState.Ready)
            {
                if(touches[0].phase == TouchPhase.Began)
                {
                    _tracking.Add(touches[0]);
                }

                if(_tracking.Count > 0)
                {
                    _startTime = Time.realtimeSinceStartup;

                    position = TouchPosition();
                    elpasedTime = 0;

                    onBegin?.Invoke(this);

                    state = EState.Began;
                }
            }

            return false;
        }

        public override void OnTouchEnded(List<UTouch> touches)
        {
            if(state == EState.Began)
            {
                position = TouchPosition();
                elpasedTime = Time.realtimeSinceStartup - _startTime;

                onEnd?.Invoke(this);

                state = EState.Recognized;
            }
        }

        public override void OnTouchMoved(List<UTouch> touches)
        {
            if (state == EState.Began)
            {
                position = TouchPosition();
                elpasedTime = Time.realtimeSinceStartup - _startTime;

                onMove?.Invoke(this);
            }
        }

        public override void OnTouchStay(List<UTouch> touches)
        {
            if (state == EState.Began)
            {
                position = TouchPosition();
                elpasedTime = Time.realtimeSinceStartup - _startTime;

                onStay?.Invoke(this);
            }
        }
    }
}
