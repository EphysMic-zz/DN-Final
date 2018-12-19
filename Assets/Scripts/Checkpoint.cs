using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Checkpoint : MonoBehaviour {

    public int currentHealth;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if (player)
        {
            player.latestCheckpoint = this;
            currentHealth = player.Health;

            GetComponent<Collider>().enabled = false;

            var checkpoints = FindObjectsOfType<Checkpoint>()
                              .Where(x => x != this);

            foreach (var checkpoint in checkpoints)
            {
                checkpoint.GetComponent<Collider>().enabled = true;
            }
        }
    }
}
