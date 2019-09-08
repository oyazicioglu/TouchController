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
        public OneFingerTouchHandler OnFingerTouch;

        /// The event and event handler that capture one finger pressed and ended
        public delegate void OneFingerHoldHandler(Vector2 Position);
        public OneFingerHoldHandler OnFingerHold;

        public delegate void OneFingerSwipe(SwipeDirections Direction);
        public OneFingerSwipe OnFingerSwipe;

        public float SwipeThreshold = 100f;
        private bool IsSwiping = false;

        // The time in second which determine when coroutine triggers the event
        public float HoldDuration = 1f;
        private bool IsHolded = false;
        // One Finger hold Coroutine
        private IEnumerator HoldCoroutine;

        /// Result of raycasting
        private List<RaycastResult> RaycastResult;

        // Start positions of touched fingers
        private List<Vector2> StartPositions;

        private List<Vector2> SwipeDeltas;



        void Start()
        {
            // Initialize raycast result object
            RaycastResult = new List<RaycastResult>();

            StartPositions = new List<Vector2>();


            SwipeDeltas = new List<Vector2>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.touchCount == 0)
                return;


            var iterator = 0;
            foreach (var finger in Input.touches)
            {
                if (finger.phase == TouchPhase.Began)
                {
                    StartPositions.Add(finger.position);

                    if (TouchHelper.CheckRaycastObject(finger.position, this.gameObject))
                    {
                        HoldCoroutine = FingerHold(finger.position);
                        StartCoroutine(HoldCoroutine);
                    }
                }
                else if (finger.phase == TouchPhase.Ended || finger.phase == TouchPhase.Canceled)
                {
                    StopCoroutine(HoldCoroutine);
                    var swipeDelta = finger.position - StartPositions[iterator];
                    SwipeDeltas.Add(swipeDelta);

                    if (TouchHelper.CheckRaycastObject(finger.position, this.gameObject))
                    {
                        DetectSwipe(SwipeDeltas);

                        if (!IsSwiping && !IsHolded)
                            OnFingerTouch?.Invoke(finger.position);
                        if (iterator == Input.touchCount)
                        {
                            StartPositions.Clear();
                            SwipeDeltas.Clear();
                        }
                    }

                    IsSwiping = false;
                    IsHolded = false;
                }

                iterator++;
            }

        }

        private void DetectSwipe(List<Vector2> swipeDeltas)
        {
            SwipeDirections SwipeDirection;
            IsSwiping = true;

            foreach (var swipeDelta in swipeDeltas)
            {
                if (swipeDelta.magnitude >= SwipeThreshold)
                {
                    if (IsHorizontalSwipe(swipeDelta))
                    {
                        SwipeDirection = swipeDelta.x < 0 ? SwipeDirections.Left : SwipeDirections.Right;
                    }
                    else
                    {
                        SwipeDirection = swipeDelta.y < 0 ? SwipeDirections.Bottom : SwipeDirections.Up;
                    }
                    OnFingerSwipe?.Invoke(SwipeDirection);
                }
                else
                {
                    IsSwiping = false;
                    return;
                }
            }
        }

        private bool IsHorizontalSwipe(Vector2 swipeDelta)
        {
            return Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y) ? true : false;
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
            OnFingerHold?.Invoke(position);
        }
    }
}