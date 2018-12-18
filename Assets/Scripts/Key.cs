using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Key : MonoBehaviour
{

    Player _player;

    [SerializeField] float _interactRange = .1f;
    bool _registered;
    [SerializeField] Transform _otherKey;
    Door _door;

    public bool rotating;
    [SerializeField] Vector3 _rotationOffset;
    [SerializeField] float _rotateSpeed;
    float _rotation;

    public void InteractWithKey()
    {
        rotating = true;
    }

    void Start()
    {
        _player = FindObjectOfType<Player>();
        _door = FindObjectOfType<Door>();
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
            }
        }

        else
        {
            if (_registered)
            {
                _registered = false;
                _player.Interact -= InteractWithKey;
            }
        }

        if (rotating)
        {
            transform.Rotate(transform.forward, _rotateSpeed * Time.deltaTime);
            _otherKey.Rotate(transform.forward, _rotateSpeed * Time.deltaTime);
            _rotation += _rotateSpeed * Time.deltaTime;

            if (_rotation >= 90)
            {
                rotating = false;
                _door.Open();
            }
        }
    }
}
