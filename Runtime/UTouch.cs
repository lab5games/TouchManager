using UnityEngine;

namespace Lab5Games
{
    public class UTouch 
    {
        public readonly int Id;

        public TouchPhase phase { get; private set; }
        public int tapCount { get; private set; }
        public Vector2 position { get; private set; }
        public Vector2 startPosition { get; private set; }
        public Vector2 deltaPosition { get; private set; }
        public float deltaTime { get; private set; }

#if UNITY_EDITOR
        private Vector2? _lastPosition;
        private double _lastClickTime;
        private double _multipleClickInterval = 0.2;
        public static double dragPixels = 1;
#endif

        public UTouch(int Id)
        {
            this.Id = Id;
            phase = TouchPhase.Ended;
        }

        public override string ToString()
        {
            return string.Format("UTouch: Id= {0}, phase= {1}, position= {2}.", Id, phase, position);
        }

        public UTouch UpdateFromTouch(Touch touch)
        {
            position = touch.position;
            deltaPosition = touch.deltaPosition;
            deltaTime = touch.deltaTime;
            tapCount = touch.tapCount;

            if (touch.phase == TouchPhase.Began)
            {
                startPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Canceled)
            {
                phase = TouchPhase.Ended;
            }
            else
            {
                phase = touch.phase;
            }

            return this;
        }

#if UNITY_EDITOR
        public UTouch UpdateFromMouse()
        {
            if(Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))
            {
                phase = TouchPhase.Moved;

                if (Input.GetMouseButtonUp(0) && Input.GetMouseButtonDown(0))
                {
                    phase = TouchPhase.Canceled;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    phase = TouchPhase.Ended;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    phase = TouchPhase.Began;
                }


                Vector2 mousePosition = Input.mousePosition;

                if (_lastPosition.HasValue)
                    deltaPosition = mousePosition - _lastPosition.Value;
                else
                    deltaPosition = Vector2.zero;


                switch (phase)
                {
                    case TouchPhase.Began:
                        if (Time.time < (_lastClickTime + _multipleClickInterval))
                            tapCount++;
                        else
                            tapCount = 1;

                        startPosition = mousePosition;
                        _lastPosition = mousePosition;
                        _lastClickTime = Time.time;
                        break;

                    case TouchPhase.Stationary:
                    case TouchPhase.Moved:
                        if (deltaPosition.magnitude < dragPixels)
                            phase = TouchPhase.Stationary;

                        _lastPosition = mousePosition;
                        break;

                    case TouchPhase.Ended:
                        _lastPosition = null;
                        break;

                }

                position = mousePosition;
            }

            return this;
        }
#endif
    }
}
