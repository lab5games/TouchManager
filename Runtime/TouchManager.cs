using UnityEngine;
using System.Collections.Generic;
using Lab5Games;

namespace Lab5Games.LTouch
{
    /*
     * https://github.com/prime31/TouchKit
     * */
    public class TouchManager : Singleton<TouchManager>
    {
        
        private List<UTouch> _cacheTouches = new List<UTouch>(MAX_TOUCHES_PROCESS);
        private List<UTouch> _liveTouches = new List<UTouch>(MAX_TOUCHES_PROCESS);
        private List<GestureRecognizer> _recognizers = new List<GestureRecognizer>(MAX_TOUCHES_PROCESS);


        public const int MAX_TOUCHES_PROCESS = 5;

        public static void AddRecognizer(GestureRecognizer recognizer)
        {
            Instance._recognizers.Add(recognizer);

            if (Instance._recognizers.Count > 1)
                Instance._recognizers.Sort();
        }

        public static void RemoveRecognizer(GestureRecognizer recognizer)
        {
            Instance._recognizers.Remove(recognizer);
        }

        public static List<GestureRecognizer> GetRecognizers()
        {
            return Instance._recognizers;
        }

        private void UpdateTouches()
        {
#if UNITY_EDITOR
            if(Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))
            {
                _liveTouches.Add(_cacheTouches[0].UpdateFromMouse());
            }
#else
            if(Input.touchCount > 0)
            {
                int c = Mathf.Min(Input.touchCount, MAX_TOUCHES_PROCESS);
                
                for(int i=0; i<c; i++)
                {
                    Touch touch = Input.GetTouch(i);

                    if (touch.fingerId < MAX_TOUCHES_PROCESS)
                        _liveTouches.Add(_cacheTouches[touch.fingerId].UpdateFromTouch(touch));
                }
            }
#endif

            if(_liveTouches.Count > 0)
            {
                for (int i = 0; i < _recognizers.Count; i++)
                    _recognizers[i].Recognize(_liveTouches);

                _liveTouches.Clear();
            }
        }

        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            for (int i = 0; i < MAX_TOUCHES_PROCESS; i++)
                _cacheTouches.Add(new UTouch(i));
        }

        private void Update()
        {
            UpdateTouches();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _cacheTouches = null;
            _liveTouches = null;
            _recognizers = null;
        }
    }
}
