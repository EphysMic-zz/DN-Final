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

    public bool coloring;
    [SerializeField] float _colorateSpeed;
    float _colorAmount;

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

        GetComponent<Renderer>().sharedMaterial.color = Color.red;
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

        while (_cameraTransition != null && _cameraTransition.value < 1)
        {
            _cameraTransition.value += Time.deltaTime;
            yield return null;
        }

        while(_colorAmount < 10)
        {
            var mat = GetComponent<Renderer>().sharedMaterial;
            _colorAmount += Time.deltaTime * _colorateSpeed;

            mat.color = Color.Lerp(Color.red, Color.green, _colorAmount);

            if (_colorAmount >= 1)
            {
                _door.Open();
                coloring = false;
            }
        }

        yield return new WaitForSeconds(_timeShowingDoor);

        _cameraTransition = FindObjectOfType<CameraTransitionController>();

        while (_cameraTransition != null && _cameraTransition.value > 0)
        {
            _cameraTransition.value -= Time.deltaTime;
            yield return null;
        }

        _player.SetBlocked(false);
        _player.Interact -= InteractWithKey;
        Destroy(_cameraTransition);
        Destroy(this);
    }
}
