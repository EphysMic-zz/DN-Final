using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{

    EventFSM<EnemyActions> _fsm;

    [SerializeField] protected float _onTimeToUpdatePath;

    [SerializeField] protected float _onTimeToAttack;
    [SerializeField] protected float _rangeToAttack;

    [SerializeField] float _rangeOfVision = 15;
    [SerializeField] float _angleOfVision = 90;

    ParticleSystem _dustParticles;

    Sword _sword;

    public string currentState;

    AudioManager _audioMg;

    bool _blocked;

    // Use this for initialization

    protected override void Start()
    {
        base.Start();

        _dustParticles = GetComponentInChildren<ParticleSystem>();

        _audioMg = GetComponent<AudioManager>();

        _sword = GetComponentInChildren<Sword>();

        #region FSM
        var idle = new State<EnemyActions>("Idle");
        var chasing = new State<EnemyActions>("Chasing");
        var attacking = new State<EnemyActions>("Attacking");
        var damaged = new State<EnemyActions>("Damaged");
        var dead = new State<EnemyActions>("Dead");

        //Idle
        idle.OnEnter += () => _anim.Play("Idle");
        idle.AddTransition(EnemyActions.PlayerInSight, chasing);
        idle.AddTransition(EnemyActions.Damaged, chasing);
        idle.AddTransition(EnemyActions.Death, dead);

        //Chasing
        float _timeToUpdatePath = 0;
        chasing.OnEnter += () =>
        {
            _navMesh.isStopped = false;
            _navMesh.SetDestination(_player.transform.position);
            _audioMg.PlayAudio("Aggro");
        };
        chasing.OnUpdate += () =>
        {
            _timeToUpdatePath += Time.deltaTime;

            if (_timeToUpdatePath >= _onTimeToUpdatePath)
            {
                _navMesh.SetDestination(_player.transform.position);
                _timeToUpdatePath = 0;
            }

            _anim.SetFloat("Speed", _navMesh.velocity.magnitude);
        };
        chasing.OnExit += () =>
        {
            _timeToUpdatePath = 0;
            _navMesh.isStopped = true;
            _anim.SetFloat("Speed", 0);
        };
        chasing.AddTransition(EnemyActions.PlayerOutOfInterest, idle);
        chasing.AddTransition(EnemyActions.PlayerInRange, attacking);
        chasing.AddTransition(EnemyActions.Death, dead);

        //Attacking
        float _timeToAttack = _onTimeToAttack;

        attacking.OnEnter += () =>
        {
            transform.LookAt(_player.transform);
        };

        attacking.OnUpdate += () =>
        {
            _timeToAttack += Time.deltaTime;

            if (_timeToAttack > _onTimeToAttack)
            {
                _anim.Play("Attack");
                Attack();
                _timeToAttack = 0;
            }
        };
        attacking.OnExit += () =>
        {
            _sword.GetComponent<Collider>().enabled = false;
        };
        attacking.AddTransition(EnemyActions.PlayerOutOfRange, chasing);
        attacking.AddTransition(EnemyActions.PlayerOutOfInterest, idle);
        attacking.AddTransition(EnemyActions.Death, dead);

        //Dead
        dead.AddTransition(EnemyActions.Reborn, idle);


        _fsm = new EventFSM<EnemyActions>(idle);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (!_blocked)
        {
            _fsm.Update();

            if (LineOfSight())
                _fsm.Feed(EnemyActions.PlayerInSight);

            if (Vector3.Distance(transform.position, _player.transform.position) > _rangeOfVision)
                _fsm.Feed(EnemyActions.PlayerOutOfInterest);

            if (Vector3.Distance(transform.position, _player.transform.position) < _rangeToAttack)
                _fsm.Feed(EnemyActions.PlayerInRange);
            else
                _fsm.Feed(EnemyActions.PlayerOutOfRange);

            currentState = _fsm.current.name;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        float halfAngle = _angleOfVision / 2.0f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(transform.position, leftRayDirection * _rangeOfVision);
        Gizmos.DrawRay(transform.position, rightRayDirection * _rangeOfVision);
    }

    bool LineOfSight()
    {
        var direction = (_player.transform.position - transform.position).normalized;

        var inRange = Vector3.Distance(transform.position, _player.transform.position) <= _rangeOfVision;
        var inFOV = Vector3.Angle(transform.forward, direction) < _angleOfVision / 2;

        bool nothingBetween = false;
        RaycastHit rayInfo = new RaycastHit();
        if (Physics.Raycast(transform.position + Vector3.up * .8f, direction, out rayInfo, LayerMask.GetMask("Player")))
        {
            nothingBetween = rayInfo.transform.GetComponent<Player>();
        }

        bool inSight = inRange && inFOV && nothingBetween;

        return inSight;
    }

    void Attack()
    {
        _sword.GetComponent<Collider>().enabled = true;
    }

    public void FinishedAttacking()
    {
        _sword.GetComponent<Collider>().enabled = false;
    }

    public override void Damage(int amount)
    {
        base.Damage(amount);

        _fsm.Feed(EnemyActions.Damaged);

        if (_currentHealth <= 0)
        {
            _dustParticles.Play();
            StartCoroutine(Dissolve());
            _fsm.Feed(EnemyActions.Death);
        }
        else
            _dustParticles.Play();
    }

    public void Reborn()
    {
        _fsm.Feed(EnemyActions.Reborn);
    }

    public void SendToPlayer()
    {
        StartCoroutine(SendToPlayerStep());
    }

    IEnumerator SendToPlayerStep()
    {
        yield return new WaitForSeconds(.1f);
        _fsm.Feed(EnemyActions.PlayerInSight);
    }

    IEnumerator Dissolve()
    {
        var dissolveMats = GetComponentInChildren<Renderer>().materials;
        float amount = 0;

        while (dissolveMats[0].GetFloat("_DissolveAmount") < 1)
        {
            foreach (var mat in dissolveMats)
            {
                mat.SetFloat("_DissolveAmount", amount);
            }

            yield return null;
            amount += Time.deltaTime;
        }

        gameObject.SetActive(false);
    }

    public void Appear()
    {
        StartCoroutine(InDissolve());
    }

    IEnumerator InDissolve()
    {
        var dissolveMats = GetComponentInChildren<Renderer>().materials;
        float amount = 1;

        while (dissolveMats[1].GetFloat("_DissolveAmount") > 0)
        {
            foreach (var mat in dissolveMats)
            {
                mat.SetFloat("_DissolveAmount", amount);
            }

            yield return null;
            amount -= Time.deltaTime;
        }
    }

    public void SetBlock(bool b)
    {
        _blocked = b;
    }
}
