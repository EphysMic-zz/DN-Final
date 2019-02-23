using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour {

    AudioSource _as;
    [SerializeField] int _amountOfHeal;

    private void Start()
    {
        _as = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if (player) player.Heal(_amountOfHeal);
    }

    public void DelayedDestroy()
    {
        StartCoroutine(StepDestroy());
    }

    IEnumerator StepDestroy()
    {
        _as.PlayOneShot(_as.clip);
        yield return new WaitForSeconds(_as.clip.length);
        Destroy(gameObject);
    }
}
