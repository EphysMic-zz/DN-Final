using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    [SerializeField] int damage;

	// Use this for initialization
	void Start ()
    {
        GetComponent<Collider>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        var enemy = other.GetComponent<Enemy>();

        if (player) player.Damage(damage);
        if (enemy) enemy.Damage(damage);

        GetComponent<Collider>().enabled = false;
    }
}
