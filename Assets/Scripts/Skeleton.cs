using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy {

    [SerializeField] float _rangeOfVision;
    [SerializeField] float _angleOfVision;

    Sword _sword;

	// Use this for initialization
	protected override void Start ()
    {
        base.Start();

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
        idle.AddTransition(EnemyActions.PlayerOutOfInterest, chasing);

        //Chasing
        float _timeToUpdatePath = 0;
        chasing.OnEnter += () =>
        {
            _navMesh.isStopped = false;
            _navMesh.SetDestination(_player.transform.position);
        };
        chasing.OnUpdate += () =>
        {
            _timeToUpdatePath += Time.deltaTime;

            if(_timeToUpdatePath >= _onTimeToUpdatePath)
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

        //Attacking
        float _timeToAttack = _onTimeToAttack;
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


        _fsm = new EventFSM<EnemyActions>(idle);
        #endregion
    }

    // Update is called once per frame
    void Update ()
    {
        _fsm.Update();

        if (LineOfSight())
            _fsm.Feed(EnemyActions.PlayerInSight);

        if(Vector3.Distance(transform.position, _player.transform.position) > _rangeOfVision)
            _fsm.Feed(EnemyActions.PlayerOutOfInterest);

        if (Vector3.Distance(transform.position, _player.transform.position) < _rangeToAttack)
            _fsm.Feed(EnemyActions.PlayerInRange);
        else
            _fsm.Feed(EnemyActions.PlayerOutOfRange);
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
        if(Physics.Raycast(transform.position + Vector3.up * .8f, direction, out rayInfo))
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

        _fsm.Feed(EnemyActions.PlayerOutOfInterest);

        if (_currentHealth <= 0)
            Destroy(gameObject);
        else
            StartCoroutine(DamageBlink());
    }

    IEnumerator DamageBlink()
    {
        var matColor = GetComponentInChildren<Renderer>().material.color;
        GetComponentInChildren<Renderer>().material.color = Color.red;

        yield return new WaitForSeconds(.1f);

        GetComponentInChildren<Renderer>().material.color = matColor;
    }
}
