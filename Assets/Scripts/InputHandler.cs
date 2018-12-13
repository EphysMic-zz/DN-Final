using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputHandler : MonoBehaviour {

    public event Action<float> OnVerticalAxisChanged = delegate { };
    public event Action<float> OnHorizontalAxisChanged = delegate { };
    public event Action OnSteadyAxis = delegate { };

    public event Action OnAttackPressed;

    public float verticalAxis
    {
        get { return Input.GetAxis("Vertical"); }
    }

    public float horizontalAxis
    {
        get { return Input.GetAxis("Horizontal"); }
    }

    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetAxis("Vertical") != 0)
            OnVerticalAxisChanged(Input.GetAxis("Vertical"));        

        if (Input.GetAxis("Horizontal") != 0)
            OnHorizontalAxisChanged(Input.GetAxis("Horizontal"));

        if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
            OnSteadyAxis();

        if (Input.GetButtonDown("Attack"))
            OnAttackPressed();
    }
}