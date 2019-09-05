using System;
using System.Collections;
using System.Collections.Generic;
using Metropolis.InputControllers;
using UnityEngine;
using UnityEngine.UI;

public class Startup : MonoBehaviour
{
    [SerializeField] Text DebugText = null;
    public TouchController ImageTouchController2;
    public TouchController ImageTouchController3;
    // Start is called before the first frame update
    void Start()
    {
        ImageTouchController2.OnOneFingerTouch += OnImageTouch2;
        ImageTouchController3.OnOneFingerTouch += OnImageTouch3;
        ImageTouchController2.OnOneFingerHold += OnImageHold2;
        ImageTouchController3.OnOneFingerHold += OnImageHold3;
    }

    private void OnImageHold3(Vector2 Position)
    {
        Log(" Holded Image 3");
    }

    private void OnImageHold2(Vector2 Position)
    {
        Log(" Holded Image 2");
    }

    private void OnImageTouch2(Vector2 Position)
    {
        Log(ImageTouchController2.gameObject.name);
    }
    private void OnImageTouch3(Vector2 Position)
    {
        Log(ImageTouchController3.gameObject.name);
    }


    public void Log(string Message)
    {
        DebugText.text += Message + "\n";
    }
}
