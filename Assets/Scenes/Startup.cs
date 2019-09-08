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
        go.OnFingerSwipe += OnSwipe;
        go.OnFingerTouch += OnTouch;
        go.OnFingerHold += OnHold;
    }

    private void OnHold(Vector2 Position)
    {
        Debug.Log("holded");
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
