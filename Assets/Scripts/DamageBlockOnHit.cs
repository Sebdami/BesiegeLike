using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBlockOnHit : MonoBehaviour {

    //Temporary
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 5.0f && collision.collider.GetComponent<Block>())
        {
            collision.collider.GetComponent<Block>().DamageBlock();
        }
    }
}
