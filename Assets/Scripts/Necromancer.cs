using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Necromancer : Enemy
{
    EventFSM<BossActions> _fsm;

    [SerializeField] Barrier[] _barriers;
    [SerializeField] FireTrap[] _fireWalls;

    [SerializeField] Skeleton[] _skeletons;
    Vector3[] _skeletonPositions;

    [SerializeField] EnergyBall _energyBallPrefab;
    [SerializeField] Transform _shootPoint;
    [SerializeField] int _minAmountOfShots, _maxAmountOfShots;
    [SerializeField] int _currentShots;
    [SerializeField] int _targetShots;

    [SerializeField] Transform[] _waypoints;
    Vector3 _currentWaypoint;

    [SerializeField] float _rangeToBeginBattle;
    float _auxRangeToBegin;
    Collider _collider;

    public event Action OnBossDeath = delegate { };
    public event Action OnBattleStarted = delegate { };

    AudioManager _audioMg;

    [SerializeField] Transform _erikaTransform;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        _auxRangeToBegin = _rangeToBeginBattle;
        _audioMg = GetComponent<AudioManager>();
        _collider = GetComponent<Collider>();

        _skeletonPositions = new Vector3[_skeletons.Length];
        for (int i = 0; i < _skeletons.Length; i++)
        {
            _skeletonPositions[i] = _skeletons[i].transform.position;
            _skeletons[i].gameObject.SetActive(false);
        }

        _targetShots = UnityEngine.Random.Range(_minAmountOfShots, _maxAmountOfShots);

        #region FSM
        var waiting = new State<BossActions>("Waiting");
        var idle = new State<BossActions>("Idle");
        var moving = new State<BossActions>("Moving");
        var shooting = new State<BossActions>("Shooting");
        var death = new State<BossActions>("Death");

        //Waiting
        waiting.AddTransition(BossActions.StartBattle, moving);

        //Idle
        idle.OnEnter += () =>
        {
            _collider.enabled = true;
            _anim.Play("Idle");
            SpawnBarriers();
            SpawnSkeletons();
        };
        idle.OnExit += () =>
        {
            _collider.enabled = false;
            DisableBarriers(); 
        };
        idle.AddTransition(BossActions.Damaged, moving);
        idle.AddTransition(BossActions.Death, death);

        //Moving
        moving.OnEnter += () =>
        {
            _collider.enabled = false;
            _navMesh.isStopped = false;
            _navMesh.ResetPath();
            _anim.SetFloat("Speed", 1);
            var currentWaypoint = _waypoints.OrderBy(x => Vector3.Distance(transform.position, x.position)).First();
            _currentWaypoint = GetNextWaypoint(currentWaypoint).position;
            _navMesh.SetDestination(_currentWaypoint);
        };
        moving.OnExit += () =>
        {
            _anim.SetFloat("Speed", 0);
            _navMesh.isStopped = true;
        };
        moving.AddTransition(BossActions.Moved, shooting);

        //Shooting
        shooting.OnEnter += () =>
        {
            _anim.Play("Shoot");
        };
        shooting.OnUpdate += () =>
        {
            transform.LookAt(_player.transform);
        };
        shooting.AddTransition(BossActions.Shooted, moving);
        shooting.AddTransition(BossActions.DoneShooting, idle);

        //Death
        death.OnEnter += () =>
        {
            _anim.Play("Die");
            foreach (var firewall in _fireWalls)
                firewall.gameObject.SetActive(false);
            OnBossDeath();
            FindObjectOfType<Messages>().UpdateQuote("We got the book. Now let's get out of here, quickly.", _erikaTransform);
        };

        _fsm = new EventFSM<BossActions>(waiting);
        #endregion

        var initialPosition = transform.position;
        _player.OnPlayerDeath += () =>
        {
            _fsm = new EventFSM<BossActions>(waiting);
            _navMesh.isStopped = true;
            transform.position = initialPosition;
            _currentShots = 0;
            _targetShots = UnityEngine.Random.Range(_minAmountOfShots, _maxAmountOfShots);
            _currentHealth = _maxHealth;
            DisableBarriers();
            DisableSkeletons();
            _rangeToBeginBattle = _auxRangeToBegin;
            

            foreach (var firewall in _fireWalls)
                firewall.gameObject.SetActive(false);

            foreach (var fireball in FindObjectsOfType<EnergyBall>())
                Destroy(fireball.gameObject);
            
        };
    }

    // Update is called once per frame
    void Update()
    {
        _fsm.Update();

        if (Vector3.Distance(transform.position, _player.transform.position) < _rangeToBeginBattle)
        {
            _fsm.Feed(BossActions.StartBattle);

            OnBattleStarted();

            foreach (var firewall in _fireWalls)
                firewall.gameObject.SetActive(true);
            
            _rangeToBeginBattle = 0;
        }

        if(Vector3.Distance(transform.position, _currentWaypoint) < 1)
        {
            _fsm.Feed(BossActions.Moved);
        }
    }

    Transform GetNextWaypoint(Transform current)
    {
        return _waypoints.Where(x => x != current)
                         .Skip(UnityEngine.Random.Range(0, _waypoints.Length - 2))
                         .First();
    }

    void Shoot()
    {
        if (_currentShots >= _targetShots)
        {
            _fsm.Feed(BossActions.DoneShooting);
            _currentShots = 0;
            _targetShots = UnityEngine.Random.Range(_minAmountOfShots, _maxAmountOfShots);
            return;
        }
        transform.LookAt(_player.transform);

        var energyBall = Instantiate(_energyBallPrefab);
        energyBall.transform.position = _shootPoint.position;
        energyBall.transform.forward = transform.forward;

        _audioMg.PlayAudio("Fireball");

        _currentShots++;        
        _fsm.Feed(BossActions.Shooted);
    }

    void SpawnBarriers()
    {
        foreach (var barrier in _barriers)
        {
            barrier.gameObject.SetActive(true);
        }
    }

    void DisableBarriers()
    {
        foreach (var barrier in _barriers)
        {
            barrier.Disable();
        }
    }

    void SpawnSkeletons()
    {
        for (int i = 0; i < _skeletons.Length; i++)
        {
            if (!_skeletons[i].gameObject.activeInHierarchy)
            {
                _skeletons[i].gameObject.SetActive(true);
                _skeletons[i].Appear();
                _skeletons[i].transform.position = _skeletonPositions[i];
                _skeletons[i].Health = _skeletons[i].MaxHealth;

                if (_skeletons[i].currentState == "Dead") _skeletons[i].Reborn();

                _skeletons[i].transform.LookAt(_player.transform);
                _skeletons[i].SendToPlayer();
            }
        }
    }

    void DisableSkeletons()
    {
        foreach (var skeleton in _skeletons)
        {
            skeleton.gameObject.SetActive(false);
        }
    }

    public override void Damage(int amount)
    {
        if (amount > 1) amount = 1;
        base.Damage(amount);

        if (_currentHealth > 0)
        {
            _fsm.Feed(BossActions.Damaged);
            _audioMg.PlayAudio("Hit");
            _anim.Play("GetHit");
        }
        else
        {
            _audioMg.PlayAudio("Hit");
            _fsm.Feed(BossActions.Death);           
        }
    }
}
