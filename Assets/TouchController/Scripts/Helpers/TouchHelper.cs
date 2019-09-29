using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Metropolis.InputControllers
{
    public sealed class TouchHelper
    {
        public static RaycastResult RaycastGUI(Vector2 position)
        {
            var result = new List<RaycastResult>();

            var currentEventSystem = EventSystem.current;
            var pointerEventData = new PointerEventData(currentEventSystem);

            pointerEventData.position = position;

            currentEventSystem.RaycastAll(pointerEventData, result);

            return result[0];
        }

        public static bool CheckRaycastObject(Vector2 position, GameObject gameObject)
        {
            var isFound = false;
            var rayResult = TouchHelper.RaycastGUI(position);
            return rayResult.gameObject == gameObject ? true : false;
           
        }

    }
}
