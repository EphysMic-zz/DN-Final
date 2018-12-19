using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    protected Player _player;
    protected NavMeshAgent _navMesh;

    [SerializeField] protected int _maxHealth;
    [SerializeField] protected int _currentHealth;

    public int MaxHealth
    {
        get { return _maxHealth; }
    }

    public int Health
    {
        set { _currentHealth = value; }
    }

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
