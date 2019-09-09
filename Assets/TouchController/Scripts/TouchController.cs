using System;
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

        [SerializeField]
        private float SwipeThreshold = 100f;

        [SerializeField]
        private float RotateThreshold = 10f;

        // The time in second which determine when coroutine triggers the event
        [SerializeField]
        private float HoldDuration = 1f;

        // One Finger hold Coroutine
        private IEnumerator HoldCoroutine;

        /// Result of raycasting
        private List<RaycastResult> RaycastResult;

        // Start positions of touched fingers
        private Vector2 OneFingerStartPosition;

        private List<Vector2> TwoFingerStartPositions;

        private List<Vector2> SwipeDeltas;

        private List<Vector2> PinchDeltas;

        private IEnumerator TouchStartCoroutine;
        private float TouchStartDelay = 0.3f;
        private bool TouchStarted = false;

        GestureTypes GestureType;

        void Start()
        {
            GestureType = GestureTypes.Touch;

            // Initialize raycast result object
            RaycastResult = new List<RaycastResult>();

            SwipeDeltas = new List<Vector2>();

            TwoFingerStartPositions = new List<Vector2>();

            PinchDeltas = new List<Vector2>();


        }

        // Update is called once per frame
        void Update()
        {
            if (Input.touchCount == 0)
                return;

            if (TouchStarted == false)
            {
                StartCoroutine(TouchStart());
            }

            TouchDetection();
        }

        private IEnumerator TouchStart()
        {
            yield return new WaitForSecondsRealtime(TouchStartDelay);
            TouchStarted = true;
        }

        private void TouchDetection()
        {
            if (TouchStarted == false)
                return;

            Debug.Log(Input.touchCount);
            // One finger Gestures
            if (Input.touchCount == 1)
            {
                var finger = Input.touches[0];

                if (finger.phase == TouchPhase.Began)
                {
                    OneFingerStartPosition = finger.position;

                    if (TouchHelper.CheckRaycastObject(finger.position, this.gameObject))
                    {
                        HoldCoroutine = FingerHold(finger.position);
                        StartCoroutine(HoldCoroutine);
                    }
                }
                else if (finger.phase == TouchPhase.Ended || finger.phase == TouchPhase.Canceled)
                {
                    StopCoroutine(HoldCoroutine);

                    var swipeDelta = finger.position - OneFingerStartPosition;
                    SwipeDeltas.Add(finger.position - OneFingerStartPosition);

                    if (TouchHelper.CheckRaycastObject(finger.position, this.gameObject))
                    {
                        DetectSwipe(SwipeDeltas);

                        if (GestureType == GestureTypes.Touch)
                        {
                            GestureType = GestureTypes.Touch;
                            OnFingerTouch?.Invoke(finger.position);
                        }
                    }

                    Reset();
                }
            }

            // Two Finger  Gestures (Pinch and Rotate)
            if (Input.touchCount == 2)
            {
                if (HoldCoroutine != null)
                    StopCoroutine(HoldCoroutine);

                foreach (var finger in Input.touches)
                {
                    if (finger.phase == TouchPhase.Began)
                    {
                        TwoFingerStartPositions.Add(finger.position);
                    }
                    else if (finger.phase == TouchPhase.Moved)
                    {

                    }
                    else if (finger.phase == TouchPhase.Ended || finger.phase == TouchPhase.Canceled)
                    {


                        //Reset();
                    }
                }
            }
        }

        private void DetectPinch(List<Vector2> PinchDeltas)
        {

            //GestureType = GestureTypes.Pinch;
        }

        private void DetectSwipe(List<Vector2> swipeDeltas)
        {
            SwipeDirections SwipeDirection;

            foreach (var swipeDelta in swipeDeltas)
            {
                if (swipeDelta.magnitude >= SwipeThreshold)
                {
                    GestureType = GestureTypes.Swipe;
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
            GestureType = GestureTypes.Touch;
            RaycastResult.Clear();
            SwipeDeltas.Clear();
            TwoFingerStartPositions.Clear();
            TouchStarted = false;
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
            GestureType = GestureTypes.Hold;
            OnFingerHold?.Invoke(position);
        }
    }
}