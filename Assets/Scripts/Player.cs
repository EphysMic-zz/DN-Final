using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Player : MonoBehaviour
{
    InputHandler _controller;
    EventFSM<PlayerActions> _fsm;

    Animator _anim;
    Rigidbody _rb;

    [SerializeField] GameObject _mouseMiddleBButton;

    [SerializeField] float _speed;
    [SerializeField] float _turnSpeed;

    Sword _sword;

    [SerializeField] int _currentHealth;
    [SerializeField] int _maxHealth;
    [SerializeField] float _dieTime;
    bool _damaged;
    bool _defense;

    public int Health
    {
        get { return _currentHealth; }
    }

    public int MaxHealth
    {
        get { return _maxHealth; }
    }

    bool _blocked;
    public Checkpoint latestCheckpoint;

    [SerializeField] float _spellRange;
    [SerializeField] int _spellDamage;

    public Action Interact = delegate { };
    public event Action OnPlayerDeath = delegate { };
    public event Action<float> OnPlayerHealthChanged = delegate { };

    [SerializeField] ParticleSystem[] _spellFX;
    [SerializeField] ParticleSystem[] _healFX;
    [SerializeField] ParticleSystem[] _parryFX;

    void Start()
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
        var castSpell = new State<PlayerActions>("Cast");
        var damaged = new State<PlayerActions>("Damaged");
        var death = new State<PlayerActions>("Death");

        //Idle
        idle.OnEnter += () => _anim.Play("Idle");
        idle.AddTransition(PlayerActions.Moved, moving);
        idle.AddTransition(PlayerActions.Attacked, attack);
        idle.AddTransition(PlayerActions.Blocking, defend);
        idle.AddTransition(PlayerActions.Spell, castSpell);
        idle.AddTransition(PlayerActions.Hurt, damaged);
        idle.AddTransition(PlayerActions.Death, death);

        //Moving
        moving.OnUpdate += () =>
        {
            if(!_blocked) transform.position += transform.forward * _controller.verticalAxis * _speed * Time.deltaTime;

            if (!_blocked) transform.Rotate(Vector3.up, _controller.horizontalAxis * _turnSpeed * Time.deltaTime);

            _anim.SetFloat("Speed", _controller.verticalAxis);
        };
        moving.OnExit += () => _anim.SetFloat("Speed", 0);
        moving.AddTransition(PlayerActions.Steady, idle);
        moving.AddTransition(PlayerActions.Attacked, attack);
        moving.AddTransition(PlayerActions.Blocking, defend);
        moving.AddTransition(PlayerActions.Spell, castSpell);
        moving.AddTransition(PlayerActions.Hurt, damaged);
        moving.AddTransition(PlayerActions.Death, death);

        //Attack
        attack.OnEnter += () =>
        {
            Attack();
            _anim.Play("Attack");
        };
        attack.OnExit += () => _sword.GetComponent<Collider>().enabled = false;
        attack.AddTransition(PlayerActions.AttackReady, idle);
        attack.AddTransition(PlayerActions.Blocking, defend);       
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
            StartCoroutine(DelayedDie());
        };
        death.OnExit += () => _blocked = false;
        death.AddTransition(PlayerActions.Revive, idle);

        //Defend
        defend.OnEnter += () =>
        {
            _blocked = true;
            _anim.Play("Block");
        };
        defend.AddTransition(PlayerActions.BlockingReady, idle);
        defend.AddTransition(PlayerActions.Hurt, damaged);
        defend.AddTransition(PlayerActions.Death, death);

        //Spell
        castSpell.OnEnter += () =>
        {
            _blocked = true;
            _anim.Play("CastSpell");
        };
        castSpell.AddTransition(PlayerActions.SpellReady, idle);
        castSpell.AddTransition(PlayerActions.Hurt, damaged);
        castSpell.AddTransition(PlayerActions.Death, death);

        _fsm = new EventFSM<PlayerActions>(idle);
        #endregion

        #region Input Handler

        _controller.OnHorizontalAxisChanged += x =>
        {
            if (!_blocked) _fsm.Feed(PlayerActions.Moved);
        };

        _controller.OnVerticalAxisChanged += x =>
        {
            if (!_blocked) _fsm.Feed(PlayerActions.Moved);
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

        _controller.OnInteractPressed += () =>
        {
            if (!_blocked) Interact();
        };
        #endregion
    }

    void Update()
    {
        if (_fsm != null)
            _fsm.Update();
    }

    void Attack()
    {
        //_blocked = true;
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
    }

    public void BlockingReady()
    {
        _defense = false;
        _blocked = false;
        _fsm.Feed(PlayerActions.BlockingReady);
    }

    public void Spell()
    {
        var cols = Physics.OverlapSphere(transform.position, _spellRange);

        var enemies = cols.
                      Where(x => x.gameObject.GetComponent<Enemy>()).
                      Select(x => x.gameObject.GetComponent<Enemy>());

        var barriers = cols.
                       Where(x => x.gameObject.GetComponent<Barrier>()).
                       Select(x => x.gameObject.GetComponent<Barrier>());

        foreach (var enemy in enemies)
        {
            enemy.Damage(_spellDamage);
        }

        foreach (var barrier in barriers)
        {
            barrier.Disable();
        }

        foreach(var part in _spellFX)
        {
            part.Play();
        }
    }

    public void SpellReady()
    {
        _blocked = false;
        _fsm.Feed(PlayerActions.SpellReady);

        foreach (var part in _spellFX)
        {
            part.Stop();
        }
    }

    public void Damage(int amount)
    {
        if (_defense)
        {
            foreach (var fx in _parryFX)
            {
                fx.Play();
            }
            return;
        }

        if (!_damaged)
        {
            if (_currentHealth > 0)
            {
                _currentHealth -= amount;
                OnPlayerHealthChanged(_currentHealth);
                _fsm.Feed(PlayerActions.Hurt);
            }
            else
                _fsm.Feed(PlayerActions.Death);
        }
    }

    void Die()
    {
        _fsm.Feed(PlayerActions.Revive);
        _currentHealth = latestCheckpoint.currentHealth;
        transform.position = latestCheckpoint.transform.position;
    }

    IEnumerator DelayedDie()
    {
        yield return new WaitForSeconds(_dieTime);
        Die();
        OnPlayerDeath();
    }

    public void Heal(int amount)
    {
        if (_currentHealth >= _maxHealth)
            return;

        _currentHealth += amount;
        OnPlayerHealthChanged(_currentHealth);

        //Destruccion de la botella 
        Destroy(Physics.OverlapSphere(transform.position, 1)
                .Where(x => x.GetComponent<Potion>() != null)
                .Select(x => x.GetComponent<Potion>()).First().gameObject);

        foreach (var fx in _healFX)
        {
            fx.Play();
        }

        if (_currentHealth > _maxHealth)
        {
            var diference = _currentHealth - _maxHealth;
            _currentHealth -= diference;
        }
    }

    public void LearnSpell()
    {
        _controller.OnSpellPressed += () =>
        {
            if (!_blocked) _fsm.Feed(PlayerActions.Spell);
        };

        /*_anim.Play("Examine");
        _blocked = true;*/
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _spellRange);
    }

    public void EstoyDesesperando()
    {
        _mouseMiddleBButton.SetActive(true);
        StartCoroutine(LaPutaMadre());
    }

    IEnumerator LaPutaMadre()
    {
        yield return new WaitForSeconds(5);
        _mouseMiddleBButton.SetActive(false);
    }
}
