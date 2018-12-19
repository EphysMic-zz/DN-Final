using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNode : MonoBehaviour {

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, .5f);
    }
}
