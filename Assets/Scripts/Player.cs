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

    [SerializeField] Transform _feets;

    bool _blocked;

	void Start ()
    {
        _controller = FindObjectOfType<InputHandler>();
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();

        #region FSM
        var idle = new State<PlayerActions>("Idle");
        var moving = new State<PlayerActions>("Moving");
        var attack = new State<PlayerActions>("Attack");

        //Idle
        idle.OnEnter += () => _anim.Play("Idle");
        idle.AddTransition(PlayerActions.Moved, moving);
        idle.AddTransition(PlayerActions.Attacked, attack);

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

        //Attack
        attack.OnEnter += () =>
        {
            Attack();
            _anim.Play("Attack");
        };
        attack.AddTransition(PlayerActions.AttackReady, idle);

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
    }

    public void Unblock()
    {
        _blocked = false;
        _fsm.Feed(PlayerActions.AttackReady);
    }
}
