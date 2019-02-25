using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Gem : MonoBehaviour {

    Player _player;
    [SerializeField] float _interactRange;

    [SerializeField] Barrier[] _barriers;
    [SerializeField] Enemy[] _enemies;
    Necromancer _boss;

    [SerializeField] Transform _erikaTransform;

    public event Action OnPowerPicked = delegate { };
    public event Action OnPowerUsed = delegate { };

	// Use this for initialization
	void Start ()
    {
        _player = FindObjectOfType<Player>();
        _boss = FindObjectOfType<Necromancer>();

        _boss.gameObject.SetActive(false);

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

            _boss.gameObject.SetActive(true);

            _player.EstoyDesesperando();

            FindObjectOfType<Messages>().UpdateQuote("The symbol you make when you hit the ground \n seems the same as the one in the library...", _erikaTransform);

            OnPowerPicked();

            _interactRange = 0;
        }

        if(_interactRange == 0)
        {
            if(!_barriers[0].gameObject.activeInHierarchy)
            {
                OnPowerUsed();
                Destroy(this);
            }
        }
	}
}
