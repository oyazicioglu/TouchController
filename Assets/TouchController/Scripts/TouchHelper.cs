using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Metropolis.InputControllers
{
    public sealed class TouchHelper
    {
        public static List<RaycastResult> RaycastGUI(Vector2 position)
        {
            var result = new List<RaycastResult>();

            var currentEventSystem = EventSystem.current;
            var pointerEventData = new PointerEventData(currentEventSystem);

            pointerEventData.position = position;

            currentEventSystem.RaycastAll(pointerEventData, result);

            return result;
        }

        public static bool CheckRaycastObject(RaycastResult raycastedObject, GameObject gameObject)
        {
            if (raycastedObject.gameObject == null)
                return false;
            return raycastedObject.gameObject == gameObject ? true : false;
        }

    }
}
