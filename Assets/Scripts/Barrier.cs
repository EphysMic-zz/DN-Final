using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public void Disable()
    {
        Destroy(gameObject);
    }
}
