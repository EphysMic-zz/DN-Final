using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        var player = FindObjectOfType<Player>();


        player.OnPlayerHealthChanged += x => GetComponent<Image>().fillAmount = x / player.MaxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
