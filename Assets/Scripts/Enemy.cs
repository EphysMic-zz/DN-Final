using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    protected EventFSM<EnemyActions> _fsm;
    protected Player _player;
    protected NavMeshAgent _navMesh;
    [SerializeField] protected float _onTimeToUpdatePath;

    [SerializeField] protected float _onTimeToAttack;
    [SerializeField] protected float _rangeToAttack;

    [SerializeField] protected int _maxHealth;
    [SerializeField] protected int _currentHealth;

    protected Animator _anim;

	// Use this for initialization
	protected virtual void Start ()
    {
        _anim = GetComponent<Animator>();
        _player = FindObjectOfType<Player>();
        _navMesh = GetComponent<NavMeshAgent>();
        _currentHealth = _maxHealth;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public virtual void Damage(int amount)
    {
        _currentHealth -= amount;
    }
}
