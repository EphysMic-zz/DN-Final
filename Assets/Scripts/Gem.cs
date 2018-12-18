using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {

    Player _player;
    [SerializeField] float _interactRange;

    [SerializeField] Barrier[] _barriers;
    [SerializeField] Enemy[] _enemies;

	// Use this for initialization
	void Start ()
    {
        _player = FindObjectOfType<Player>();

        foreach (var enemy in _enemies)
        {
            enemy.gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Vector3.Distance(transform.position, _player.transform.position) < _interactRange)
        {
            _player.LearnSpell();

            foreach (var barrier in _barriers)
            {
                barrier.gameObject.SetActive(true);
            }

            foreach (var enemy in _enemies)
            {
                enemy.gameObject.SetActive(true);
            }

            Destroy(this);
        }
	}
}
