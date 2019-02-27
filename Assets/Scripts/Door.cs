using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public void Open()
    {
        GetComponent<Animator>().SetBool("Opened", true);
        GetComponent<AudioSource>().Play();
    }
}
