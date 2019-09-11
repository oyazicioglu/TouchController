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
        public OneFingerTouchHandler OnTouch;

        /// The event and event handler that capture one finger pressed and ended
        public delegate void OneFingerHoldHandler(Vector2 Position);
        public OneFingerHoldHandler OnHold;

        public delegate void OneFingerSwipe(SwipeDirections Direction);
        public OneFingerSwipe OnSwipe;

        /// The event and event handler that capture pinch
        public delegate void PinchTouchEventHandler(float Ratio, PinchDirections Direction);
        public PinchTouchEventHandler OnPinch;

        //Distance for detecting swipe gesture
        [SerializeField]
        private float SwipeThreshold = 100f;

        // The time in second which determine when coroutine triggers the event
        [SerializeField]
        private float HoldDuration = 1f;

        // One Finger hold Coroutine
        private IEnumerator HoldCoroutine;

        /// Result of raycasting
        private List<RaycastResult> RaycastResult;

        // Start posisitons of fingers
        private Vector2[] StartPositions = new Vector2[2];

        // End posisitons of fingers
        private Vector2[] EndPositions = new Vector2[2];

        // Distance between posisitons of two fingers each
        private Vector2[] PinchDeltas = new Vector2[2];

        GestureTypes GestureType;

        // Called once application started
        void Start()
        {
            // Always assume the gesture is touch
            GestureType = GestureTypes.Touch;

            // Initialize raycast result object
            RaycastResult = new List<RaycastResult>();
        }

        // Update is called once per frame
        void Update()
        {
            // If no finger touched the screen, do nothing
            if (Input.touchCount == 0)
                return;

            // One finger Gestures
            if (Input.touchCount == 1)
            {
                var finger = Input.touches[0];

                // When finger touched; store the start position of it
                if (finger.phase == TouchPhase.Began)
                {
                    StartPositions[0] = finger.position;

                    // Check if this object is under the touched position
                    if (TouchHelper.CheckRaycastObject(finger.position, this.gameObject))
                    {
                        // Start HoldCoroutine to detect if finger is pressing and holding
                        HoldCoroutine = FingerHold(finger.position);
                        StartCoroutine(HoldCoroutine);
                    }
                }
                else if (finger.phase == TouchPhase.Ended || finger.phase == TouchPhase.Canceled)
                {
                    // IF finger touch is ended or canceled
                    // Stop Hold coroutine and store the end position of finger
                    StopCoroutine(HoldCoroutine);
                    EndPositions[0] = finger.position;

                    // Check if this object is under the touched position
                    if (TouchHelper.CheckRaycastObject(finger.position, this.gameObject))
                    {
                        // Detect if the finger is moved out of swipe threshold
                        DetectSwipe(EndPositions[0] - StartPositions[0]);

                        // If gesture type is still touch (No swipe detection and no second finger touched)
                        // trigger the Touch event handler
                        if (GestureType == GestureTypes.Touch)
                        {
                            OnTouch?.Invoke(finger.position);
                        }
                    }
                    // Reset the values
                    Reset();
                }
            }

            // Two finger Gestures
            if (Input.touchCount == 2)
            {
                // If Hold coroutine is started, stop it
                if (HoldCoroutine != null)
                    StopCoroutine(HoldCoroutine);

                // Iterate through all fingers to assign start and end positions
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.touches[i].phase == TouchPhase.Began)
                    {
                        // If fingers touched, store start positions of two fingers
                        StartPositions[i] = Input.touches[i].position;
                    }
                    else if (Input.touches[i].phase == TouchPhase.Moved)
                    {
                        // Store end positions of fingers
                        EndPositions[i] = Input.touches[i].position;

                        // Set gesture type as Pinch to avoid touch event
                        GestureType = GestureTypes.Pinch;
                    }
                    else if (Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled)
                    {
                        // Store end positions of fingers
                        EndPositions[i] = Input.touches[i].position;

                        // Detect if fingers got closed to each other or got away from each other
                        DetectPinch(StartPositions, EndPositions);

                        // Reset the values
                        Reset();
                    }
                }
            }
        }

        /// <summary>
        /// Detects if fingers got closed to each other 
        /// or got away from each other
        /// </summary>
        /// <param name="startPositions"></param>
        /// <param name="endPositions"></param>
        private void DetectPinch(Vector2[] startPositions, Vector2[] endPositions)
        {
            float ratio = 1f;

            // Assign magnitudes (distance between first and second fingers movement)
            float startMagnitude = (startPositions[0] - startPositions[1]).magnitude;
            float endMagnitude = (endPositions[0] - endPositions[1]).magnitude;


            ratio = endMagnitude / startMagnitude;

            // Check distance between start positions and end positions of fingers
            if (startMagnitude > endMagnitude) OnPinch?.Invoke(ratio, PinchDirections.In);
            else OnPinch?.Invoke(ratio, PinchDirections.Out);
        }

        /// <summary>
        /// Detects finger movement for +-X or +-Y direction
        /// </summary>
        /// <param name="swipeDelta"></param>
        private void DetectSwipe(Vector2 swipeDelta)
        {
            SwipeDirections SwipeDirection;

            // Check finger movement distance is much more then threshold
            if (swipeDelta.magnitude >= SwipeThreshold)
            {
                // Assign gesture type to swipe to prevent touch event
                GestureType = GestureTypes.Swipe;

                // If movement on X axis, trigger right or left
                // else trigger up or bottom
                if (IsHorizontalSwipe(swipeDelta))
                    SwipeDirection = swipeDelta.x < 0 ? SwipeDirections.Left : SwipeDirections.Right;
                else
                    SwipeDirection = swipeDelta.y < 0 ? SwipeDirections.Bottom : SwipeDirections.Up;

                OnSwipe?.Invoke(SwipeDirection);
            }
        }

        /// <summary>
        /// Checks the direction of movement
        /// </summary>
        /// <param name="swipeDelta"></param>
        /// <returns></returns>
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