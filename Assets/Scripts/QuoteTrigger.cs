using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuoteTrigger : MonoBehaviour {

    [SerializeField] Transform _erikaTransform;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if (player)
        {
            FindObjectOfType<Messages>().UpdateQuote("It seems sealed by some kind of spell...", _erikaTransform);
            Destroy(gameObject);
        }
    }
}
