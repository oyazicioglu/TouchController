using System;
using System.Collections;
using System.Collections.Generic;
using Metropolis.InputControllers;
using UnityEngine;
using UnityEngine.UI;

public class Startup : MonoBehaviour
{
    [SerializeField] TouchController go = null;

    // Start is called before the first frame update
    void Start()
    {
        if (go == null)
            return;

        go.OnSwipe += OnSwipe;
        go.OnTouch += OnTouch;
        go.OnHold += OnHold;
        go.OnPinch += OnPinch;
    }

    private void OnPinch(float Ratio, PinchDirections Direction)
    {
        Debug.Log("Pinch to " + Direction + " with " + Ratio.ToString() + " ratio.");
    }

    private void OnHold(Vector2 Position)
    {
        Debug.Log("Holded");
    }

    private void OnTouch(Vector2 Position)
    {
        Debug.Log("Touched");
    }

    private void OnSwipe(SwipeDirections Direction)
    {
        Debug.Log(Direction.ToString());
    }


}
