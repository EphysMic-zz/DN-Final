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

    [SerializeField] GameObject _interactUI;

    public void InteractWithKey()
    {
        coloring = true;
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

        if (coloring)
        {
            var mat = GetComponent<Renderer>().sharedMaterial;
            _colorAmount += Time.deltaTime * _colorateSpeed;

            mat.color = Color.LerpUnclamped(Color.red, Color.green, _colorAmount);

            if(_colorAmount >= 10)
            {
                _door.Open();
                coloring = false;
            }
        }
    }
}
