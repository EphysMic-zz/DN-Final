using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FireTrap : MonoBehaviour {

    [SerializeField] float _onTimeToLightUp;
    float _timeToLightUp;

    [SerializeField] int _damage;

	// Use this for initialization
	void Start ()
    {
        GetComponent<Collider>().enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        _timeToLightUp += Time.deltaTime;

        if(_timeToLightUp >= _onTimeToLightUp)
        {
            _timeToLightUp = 0;
            LightUp();
        }
	}

    void LightUp()
    {
        foreach (var ps in GetComponentsInChildren<ParticleSystem>().Where(x => !x.isPlaying))
        {
            ps.Play();
        }

        GetComponent<Collider>().enabled = true;
        StartCoroutine(DelayedCollider());
    }

    IEnumerator DelayedCollider()
    {       
        yield return new WaitForSeconds(2f);
        GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        var enemy = other.GetComponent<Enemy>();

        if (player)
        {
            GetComponent<Collider>().enabled = false;
            player.Damage(_damage);
        }

        if (enemy)
        {
            GetComponent<Collider>().enabled = false;
            enemy.Damage(_damage);
        }       
    }
}
