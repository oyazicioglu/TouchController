using System;
using System.Collections;
using System.Collections.Generic;
using Metropolis.InputControllers;
using UnityEngine;
using UnityEngine.UI;

public class Startup : MonoBehaviour
{
    [SerializeField] TouchController go = null;
    [SerializeField] TouchController go2 = null;

    // Start is called before the first frame update
    void Start()
    {
        if (go == null)
            return;

        go.OnSwipe += OnSwipe;
        go.OnTouch += OnTouch;
        go.OnHold += OnHold;
        go.OnPinch += OnPinch;

        go2.OnSwipe += OnSwipe2;
        go2.OnTouch += OnTouch2;
        go2.OnHold += OnHold2;
        go2.OnPinch += OnPinch2;
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

private void OnPinch2(float Ratio, PinchDirections Direction)
    {
        Debug.Log("2 Pinch to " + Direction + " with " + Ratio.ToString() + " ratio.");
    }

    private void OnHold2(Vector2 Position)
    {
        Debug.Log("2 Holded");
    }

    private void OnTouch2(Vector2 Position)
    {
        Debug.Log("2 Touched");
    }

    private void OnSwipe2(SwipeDirections Direction)
    {
        Debug.Log("2 " + Direction.ToString());
    }
}
