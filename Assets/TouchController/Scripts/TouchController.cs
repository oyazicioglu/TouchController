using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Metropolis.InputControllers
{
    public class TouchController : MonoBehaviour
    {
        /// The event and event handler that capture one finger pressed and ended
        public delegate void OneFingerTouchHandler(Vector2 Position);
        public OneFingerTouchHandler OnOneFingerTouch;

        /// The event and event handler that capture one finger pressed and ended
        public delegate void OneFingerHoldHandler(Vector2 Position);
        public OneFingerHoldHandler OnOneFingerHold;

        public delegate void OneFingerSwipe(Vector2 BeginPosition, Vector2 EndPosition, SwipeDirections Direction);
        public OneFingerSwipe OnOneFingerSwipe;

        // Determine if finger is holded or not
        private bool IsHolded = false;

        private bool IsDragging = false;

        public float SwipeThreshold = 100f;

        // The time in second which determine when coroutine triggers the event
        public float HoldDuration = 1f;

        // One Finger hold Coroutine
        private IEnumerator HoldCoroutine;

        /// Result of raycasting
        private List<RaycastResult> RaycastResult;

        // Start positions of touched fingers
        private Vector2 StartPosition;

        private Vector2 SwipeDelta;



        void Start()
        {
            // Initialize raycast result object
            RaycastResult = new List<RaycastResult>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.touchCount == 0)
                return;

            foreach (var finger in Input.touches)
            {
                var rayResult = TouchHelper.RaycastGUI(finger.position);
                foreach (var item in rayResult)
                {
                    if (TouchHelper.CheckRaycastObject(item, this.gameObject))
                    {
                        if (finger.phase == TouchPhase.Began)
                        {
                            StartPosition = finger.position;
                            IsDragging = false;
                        }
                        else if (finger.phase == TouchPhase.Moved)
                        {
                            IsDragging = true;
                        }
                        else if (finger.phase == TouchPhase.Ended || finger.phase == TouchPhase.Canceled)
                        {
                            if (!IsDragging)
                                OnOneFingerTouch?.Invoke(finger.position);
                            IsDragging = false;
                            Reset();
                        }

                        if (IsDragging)
                        {
                            SwipeDelta = finger.position - StartPosition;

                            if (SwipeDelta.magnitude >= SwipeThreshold)
                            {
                                float x = SwipeDelta.x;
                                float y = SwipeDelta.y;

                                if (Mathf.Abs(x) > Mathf.Abs(y))
                                {
                                    if (x < 0)
                                    {
                                        OnOneFingerSwipe?.Invoke(StartPosition, finger.position, SwipeDirections.Left);
                                        return;
                                    }
                                    else
                                        OnOneFingerSwipe?.Invoke(StartPosition, finger.position, SwipeDirections.Right);
                                }
                                else
                                {
                                    if (y < 0)
                                        OnOneFingerSwipe?.Invoke(StartPosition, finger.position, SwipeDirections.Bottom);
                                    else
                                        OnOneFingerSwipe?.Invoke(StartPosition, finger.position, SwipeDirections.Up);
                                }
                            }
                        }


                    }
                }

            }


        }

        /// <summary>
        /// Resets all of touch processes
        /// </summary>
        private void Reset()
        {
            RaycastResult.Clear();
        }

        /// <summary>
        /// Coroutine that trigger Hold Event when one finger holded
        /// after given HoldDuration time in second
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private IEnumerator FingerHold(Vector2 position)
        {
            yield return new WaitForSecondsRealtime(HoldDuration);
            IsHolded = true;
            OnOneFingerHold?.Invoke(position);
        }
    }
}