﻿using System;
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
        public OneFingerTouchHandler OnTouch;

        /// The event and event handler that capture one finger pressed and ended
        public delegate void OneFingerHoldHandler(Vector2 Position);
        public OneFingerHoldHandler OnHold;

        public delegate void OneFingerSwipe(SwipeDirections Direction);
        public OneFingerSwipe OnSwipe;

        public delegate void PinchTouchEventHandler(PinchDirections Direction);
        public PinchTouchEventHandler OnPinch;

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

       
        private Vector2[] StartPositions = new Vector2[2];
        private Vector2[] EndPositions = new Vector2[2];
        private Vector2[] PinchDeltas = new Vector2[2];

        private Vector2 SwipeDelta;


        GestureTypes GestureType;

        void Start()
        {
            GestureType = GestureTypes.Touch;

            // Initialize raycast result object
            RaycastResult = new List<RaycastResult>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.touchCount == 0)
                return;

            // One finger Gestures
            if (Input.touchCount == 1)
            {
                var finger = Input.touches[0];

                if (finger.phase == TouchPhase.Began)
                {
                    StartPositions[0] = finger.position;

                    if (TouchHelper.CheckRaycastObject(finger.position, this.gameObject))
                    {
                        HoldCoroutine = FingerHold(finger.position);
                        StartCoroutine(HoldCoroutine);
                    }
                }
                else if (finger.phase == TouchPhase.Ended || finger.phase == TouchPhase.Canceled)
                {
                    StopCoroutine(HoldCoroutine);
                    EndPositions[0] = finger.position;
                    SwipeDelta = EndPositions[0] - StartPositions[0];

                    if (TouchHelper.CheckRaycastObject(finger.position, this.gameObject))
                    {
                        DetectSwipe(SwipeDelta);

                        if (GestureType == GestureTypes.Touch)
                        {
                            GestureType = GestureTypes.Touch;
                            OnTouch?.Invoke(finger.position);
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

                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.touches[i].phase == TouchPhase.Began)
                    {
                        StartPositions[i] = Input.touches[i].position;
                    }
                    else if (Input.touches[i].phase == TouchPhase.Moved)
                    {
                        EndPositions[i] = Input.touches[i].position;                        
                    }
                    else if (Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled)
                    {
                        EndPositions[i] = Input.touches[i].position;
                        DetectPinch(StartPositions, EndPositions);
                        Reset();
                    }
                }
            }
        }


        private void DetectPinch(Vector2[] startPositions, Vector2[] endPositions)
        {
            float firstPinchDelta = (startPositions[0] - startPositions[1]).magnitude;
            float secondPinchDelta = (endPositions[0] - endPositions[1]).magnitude;

            if(firstPinchDelta > secondPinchDelta){
                OnPinch?.Invoke(PinchDirections.In);
            }else{
                OnPinch?.Invoke(PinchDirections.Out);
            }
        }

        private void DetectSwipe(Vector2 swipeDelta)
        {
            SwipeDirections SwipeDirection;

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
                    OnSwipe?.Invoke(SwipeDirection);
                }
                else
                {
                    return;
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
            OnHold?.Invoke(position);
        }
    }
}