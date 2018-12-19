using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour {

    [SerializeField] int _damage;
    [SerializeField] float _speed;

    Player _player;

    void Start()
    {
        _player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        transform.forward = Vector3.Slerp(transform.forward, ((_player.transform.position + Vector3.up) - transform.position).normalized, Time.deltaTime);

        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if (player) player.Damage(_damage);

        Destroy(gameObject);
    }
}
