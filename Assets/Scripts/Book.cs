using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour {

    bool _pickeable;
    Player _player;

    [SerializeField] float _rangeToInteract;
    public GameObject fireWall;

	// Use this for initialization
	void Start ()
    {
        var necro = FindObjectOfType<Necromancer>();
        necro.OnBossDeath += () => _pickeable = true;
        necro.OnBossDeath += () => fireWall.SetActive(true);

        _player = FindObjectOfType<Player>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_pickeable)
        {
            if(Vector3.Distance(_player.transform.position, transform.position) < _rangeToInteract)
            {
                Destroy(fireWall);
                Destroy(gameObject);
            }
        }
	}
}
