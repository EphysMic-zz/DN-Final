using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CameraTransitionController : MonoBehaviour
{

    public Material transitionEffect;
    //public Slider slider;
    public Camera otherCamera;
    public RenderTexture renderText;
    Camera _myCamera;

    [Range(0f, 1f)] public float value;

    public Dictionary<float, Camera> cameras = new Dictionary<float, Camera>();

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, transitionEffect);
    }

    private void Start()
    {
        //slider.onValueChanged.AddListener(delegate { UpdateTransition(slider.value); });

        //cameras[slider.value] = GetComponent<Camera>();
        //cameras[1 - slider.value] = otherCamera;

        cameras[value] = GetComponent<Camera>();
        cameras[1 - value] = otherCamera;

        otherCamera.enabled = false;
        _myCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        UpdateTransition(value);
    }

    public void UpdateTransition(float value)
    {
        //Remap para el valor del lerp
        if(cameras[1] == _myCamera)
            transitionEffect.SetFloat("_Transition", 1 - value);

        else
            transitionEffect.SetFloat("_Transition", value);

        otherCamera.enabled = true;

        if (cameras.ContainsKey(value))
        {
            if (cameras[value] != _myCamera)
            {
                //Paso el componente
                var newController = cameras[value].gameObject.AddComponent<CameraTransitionController>();
                newController.transitionEffect = this.transitionEffect;
                //newController.slider = this.slider;
                newController.value = this.value;
                newController.otherCamera = _myCamera;
                newController.renderText = this.renderText;
                otherCamera.targetTexture = null;

                //Deshabilito este
                _myCamera.targetTexture = renderText;
                _myCamera.enabled = false;
                Destroy(this);
            }

            //Si está solo mi camara, deshabilito la otra.
            else
                otherCamera.enabled = false;
        }
    }
}
