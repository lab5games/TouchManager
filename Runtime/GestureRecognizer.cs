using UnityEngine;
using System;
using System.Collections.Generic;

namespace Lab5Games
{
    public abstract class GestureRecognizer : IComparable<GestureRecognizer>
    {
        public enum EState
        {
            Ready,
            Began,
            FailedOrEnded,
            Recognizing,
            Recognized
        }

        protected List<UTouch> _tracking;

        public virtual int order { get { return 0; } }

        private EState _state;

        public EState state
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;

                if (_state == EState.Recognized || _state == EState.Recognizing)
                    OnTriggerEvents();

                if (_state == EState.Recognized || _state == EState.FailedOrEnded)
                    Reset();
            }
        }

        public UTouch PrimaryTouch
        {
            get
            {
                if(_tracking != null && _tracking.Count > 0)
                {
                    for (int i = 0; i < _tracking.Count; i++)
                        if (_tracking[i].Id == 0)
                            return _tracking[i];
                }

                return null;
            }
        }

        public virtual void OnTriggerEvents() { }
        public virtual bool OnTouchBegan(List<UTouch> touches) { return false; }
        public virtual void OnTouchMoved(List<UTouch> touches) { }
        public virtual void OnTouchEnded(List<UTouch> touches) { }
        public virtual void OnTouchStay(List<UTouch> touches) { }

        public GestureRecognizer()
        {
            Reset();
        }

        public virtual void Reset()
        {
            _state = EState.Ready;

            if (_tracking == null)
                _tracking = new List<UTouch>(TouchManager.MAX_TOUCHES_PROCESS);
            else
                _tracking.Clear();
        }

        public virtual void Recognize(List<UTouch> touches)
        {
            for(int i= touches.Count-1; i>=0; i--)
            {
                UTouch touch = touches[i];

                switch(touch.phase)
                {
                    case TouchPhase.Began:
                        if(OnTouchBegan(touches))
                        {
                            int removedTouches = 0;
                            for(int j=touches.Count-1; j>=0; j--)
                            {
                                if(touches[j].phase == TouchPhase.Began)
                                {
                                    touches.RemoveAt(j);
                                    removedTouches++;
                                }
                            }

                            if(removedTouches > 0)
                            {
                                i -= (removedTouches - 1);
                            }
                        }
                        break;

                    case TouchPhase.Moved:
                        OnTouchMoved(touches);
                        break;

                    case TouchPhase.Stationary:
                        OnTouchStay(touches);
                        break;

                    case TouchPhase.Ended:
                        OnTouchEnded(touches);
                        break;
                }
            }
        }

        public Vector2 TouchPosition()
        {
            float x = 0;
            float y = 0;
            float k = 0;

            for (int i = 0; i < _tracking.Count; i++)
            {
                x += _tracking[i].position.x;
                y += _tracking[i].position.y;
                k++;
            }

            return (k > 0) ? new Vector2(x / k, y / k) : Vector2.zero;
        }

        // Sorting by descending
        public int CompareTo(GestureRecognizer other)
        {
            if (this.order < other.order)
                return 1;

            if (this.order > other.order)
                return -1;

            return 0;
        }
    }
}
