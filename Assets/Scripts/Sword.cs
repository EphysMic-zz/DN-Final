using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    [SerializeField] int damage;

    AudioManager _audioMg;

	// Use this for initialization
	void Start ()
    {
        GetComponent<Collider>().enabled = false;
        _audioMg = GetComponent<AudioManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        var enemy = other.GetComponent<Enemy>();

        if (player)
        {
            player.Damage(damage);
            _audioMg.PlayAudio("Hit");
        }
        if (enemy)
        {
            enemy.Damage(damage);
            _audioMg.PlayAudio("Hit");
        }

        GetComponent<Collider>().enabled = false;
    }
}
