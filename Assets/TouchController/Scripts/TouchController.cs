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

        // The time in second which determine when coroutine triggers the event
        public float HoldDuration = 1f;

        // One Finger hold Coroutine
        private IEnumerator HoldCoroutine;

        /// Result of raycasting
        private List<RaycastResult> RaycastResult;

        // Start positions of touched fingers
        private List<Vector2> StartPositions;

        void Start()
        {
            // Initialize raycast result object
            RaycastResult = new List<RaycastResult>();
            StartPositions = new List<Vector2>();
        }

        // Update is called once per frame
        void Update()
        {
            // If no finger touches on screen, do nothing
            if (Input.touchCount == 0)
                return;

            // The fingers that touched on screen
            var fingers = Input.touches;

            var i = 0;
            foreach (var finger in fingers)
            {
                StartPositions.Add(finger.position);
                var rayResult = TouchHelper.RaycastGUI(finger.position);
                foreach (var item in rayResult)
                {
                    if (TouchHelper.CheckRaycastObject(item, this.gameObject))
                    {
                        if (finger.phase == TouchPhase.Began)
                        {
                            HoldCoroutine = FingerHold(finger.position);
                            StartCoroutine(HoldCoroutine);
                        }

                        if (finger.phase == TouchPhase.Ended)
                        {
                            if (!IsHolded)
                                OnOneFingerTouch?.Invoke(finger.position);

                            StopCoroutine(HoldCoroutine);
                            IsHolded = false;
                        }

                        if (finger.phase == TouchPhase.Moved)
                        {
                            OnOneFingerSwipe?.Invoke(StartPositions[i], finger.position, SwipeDirections.Bottom);
                            IsHolded = false;
                            StopCoroutine(HoldCoroutine);

                        }
                    }

                }

                i++;
            }

            Reset();
        }

        /// <summary>
        /// Resets all of touch processes
        /// </summary>
        private void Reset()
        {
            RaycastResult.Clear();
            StartPositions.Clear();
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