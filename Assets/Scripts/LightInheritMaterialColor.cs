using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightInheritMaterialColor : MonoBehaviour {

    Light _light;
    [SerializeField] Renderer _target;
    Material _material;

    void Start ()
    {
        _light = GetComponent<Light>();
        _material = _target.material;
	}
	
	// Update is called once per frame
	void Update ()
    {
        _light.color = _material.color;
	}
}
