using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour {

    [SerializeField] int _amountOfHeal;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if (player) player.Heal(_amountOfHeal);
    }
}
