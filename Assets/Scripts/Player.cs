using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    InputHandler _controller;
    EventFSM<PlayerActions> _fsm;

    Animator _anim;
    Rigidbody _rb;

    [SerializeField] float _speed;
    [SerializeField] float _turnSpeed;

    Sword _sword;

    [SerializeField] int _currentHealth;
    [SerializeField] int _maxHealth;
    bool _damaged;
    bool _defense;

    bool _blocked;

	void Start ()
    {
        _controller = FindObjectOfType<InputHandler>();
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _sword = GetComponentInChildren<Sword>();
        _currentHealth = _maxHealth;

        #region FSM
        var idle = new State<PlayerActions>("Idle");
        var moving = new State<PlayerActions>("Moving");
        var attack = new State<PlayerActions>("Attack");
        var defend = new State<PlayerActions>("Defend");
        var damaged = new State<PlayerActions>("Damaged");
        var death = new State<PlayerActions>("Death");

        //Idle
        idle.OnEnter += () => _anim.Play("Idle");
        idle.AddTransition(PlayerActions.Moved, moving);
        idle.AddTransition(PlayerActions.Attacked, attack);
        idle.AddTransition(PlayerActions.Blocking, defend);
        idle.AddTransition(PlayerActions.Hurt, damaged);
        idle.AddTransition(PlayerActions.Death, death);

        //Moving
        moving.OnUpdate += () =>
        {
            transform.position += transform.forward * _controller.verticalAxis * _speed * Time.deltaTime;

            transform.Rotate(Vector3.up, _controller.horizontalAxis * _turnSpeed * Time.deltaTime);
            
            _anim.SetFloat("Speed", _controller.verticalAxis);
            //_anim.SetFloat("HorizontalSpeed", _controller.horizontalAxis);
        };
        moving.AddTransition(PlayerActions.Steady, idle);
        moving.AddTransition(PlayerActions.Attacked, attack);
        moving.AddTransition(PlayerActions.Blocking, defend);
        moving.AddTransition(PlayerActions.Hurt, damaged);
        moving.AddTransition(PlayerActions.Death, death);

        //Attack
        attack.OnEnter += () =>
        {
            Attack();
            _anim.Play("Attack");
        };
        attack.AddTransition(PlayerActions.AttackReady, idle);
        attack.AddTransition(PlayerActions.Hurt, damaged);
        attack.AddTransition(PlayerActions.Death, death);

        //Damaged
        damaged.OnEnter += () =>
        {
            _anim.Play("GetHit");
            _blocked = true;
            _damaged = true;
        };
        damaged.OnExit += () =>
        {
            _blocked = false;
            _damaged = false;
        };
        damaged.AddTransition(PlayerActions.HurtReady, idle);
        damaged.AddTransition(PlayerActions.Death, death);

        //Death
        death.OnEnter += () =>
        {
            _blocked = true;
            _anim.Play("Death");
        };

        //Defend
        defend.OnEnter += () =>
        {
            _blocked = true;
            _anim.Play("Block");
        };
        defend.AddTransition(PlayerActions.BlockingReady, idle);
        defend.AddTransition(PlayerActions.Hurt, damaged);
        defend.AddTransition(PlayerActions.Death, death);


        _fsm = new EventFSM<PlayerActions>(idle);
        #endregion

        #region Input Handler

        _controller.OnHorizontalAxisChanged += x => 
        {
            if (!_blocked) _fsm.Feed(PlayerActions.Moved);
        };

        _controller.OnVerticalAxisChanged += x =>
        {
            if(!_blocked) _fsm.Feed(PlayerActions.Moved);
        };

        _controller.OnAttackPressed += () =>
        {
            if (!_blocked) _fsm.Feed(PlayerActions.Attacked);
        };

        _controller.OnSteadyAxis += () =>
        {
            if (!_blocked) _fsm.Feed(PlayerActions.Steady);
        };

        _controller.OnDefendPressed += () =>
        {
            if (!_blocked) _fsm.Feed(PlayerActions.Blocking);
        };
        #endregion
    }
	
	void Update ()
    {
        if(_fsm != null)
            _fsm.Update();
	}

    void Attack()
    {
        _blocked = true;
        _sword.GetComponent<Collider>().enabled = true;
    }

    public void AttackReady()
    {
        _blocked = false;
        _sword.GetComponent<Collider>().enabled = false;
        _fsm.Feed(PlayerActions.AttackReady);
    }

    public void HitReady()
    {
        _blocked = false;
        _damaged = false;
        _fsm.Feed(PlayerActions.HurtReady);
    }

    public void Blocking()
    {
        _defense = true;
        print("REPUTO");
    }

    public void BlockingReady()
    {
        _defense = false;
        _blocked = false;
        print("PUTO");
        _fsm.Feed(PlayerActions.BlockingReady);
    }

    public void Damage(int amount)
    {
        if (_defense)
        {
            print("Bloquea3");
            return;
        }

        if (!_damaged)
        {
            if (_currentHealth > 0)
            {
                _currentHealth -= amount;
                _fsm.Feed(PlayerActions.Hurt);
            }
            else
                _fsm.Feed(PlayerActions.Death);
        }
    }
}
