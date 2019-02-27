using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Key : MonoBehaviour
{

    Player _player;

    [SerializeField] float _interactRange = .1f;
    bool _registered;
    Door _door;

    [SerializeField] Material _colorLerpMaterial;
    [SerializeField] float _colorateSpeed;

    CameraTransitionController _cameraTransition;
    [SerializeField] float _timeShowingDoor;

    [SerializeField] GameObject _interactUI;

    public void InteractWithKey()
    {
        StartCoroutine(ShowDoorAnimation());
    }

    void Start()
    {
        _player = FindObjectOfType<Player>();
        _door = FindObjectOfType<Door>();

        _cameraTransition = FindObjectOfType<CameraTransitionController>();

        _colorLerpMaterial.SetFloat("_Transition", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) < _interactRange)
        {
            if (!_registered)
            {
                _registered = true;
                _player.Interact += InteractWithKey;
                _interactUI.SetActive(true);
            }
        }

        else
        {
            if (_registered)
            {
                _registered = false;
                _player.Interact -= InteractWithKey;
                _interactUI.SetActive(false);
            }
        }
    }

    IEnumerator ShowDoorAnimation()
    {
        _player.SetBlocked(true);
        _interactUI.SetActive(false);

        foreach (var enemy in FindObjectsOfType<Skeleton>())
            enemy.SetBlock(true);
        

        while (_cameraTransition != null && _cameraTransition.value < 1)
        {
            _cameraTransition.value += Time.deltaTime;
            yield return null;
        }

        float amount = 0;
        while(_colorLerpMaterial.GetFloat("_Transition") < 1)
        {
            _colorLerpMaterial.SetFloat("_Transition", amount);
            amount += Time.deltaTime * _colorateSpeed;
            yield return null;
        }

        _door.Open();

        yield return new WaitForSeconds(_timeShowingDoor);

        _cameraTransition = FindObjectOfType<CameraTransitionController>();

        while (_cameraTransition != null && _cameraTransition.value > 0)
        {
            _cameraTransition.value -= Time.deltaTime;
            yield return null;
        }

        foreach (var enemy in FindObjectsOfType<Skeleton>())
            enemy.SetBlock(false);

        _player.SetBlocked(false);
        _player.Interact -= InteractWithKey;
        Destroy(_cameraTransition);
        Destroy(this);
    }
}
