using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour {

    bool _pickeable;
    Player _player;

    [SerializeField] float _rangeToInteract;

	// Use this for initialization
	void Start ()
    {
        FindObjectOfType<Necromancer>().OnBossDeath += () => _pickeable = true;
        _player = FindObjectOfType<Player>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_pickeable)
        {
            if(Vector3.Distance(_player.transform.position, transform.position) < _rangeToInteract)
            {
                Destroy(gameObject);
            }
        }
	}
}
