using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBall : MonoBehaviour {

    private void Start()
    {
        Destroy(gameObject, 5.0f);
    }

    //Temporary
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 5.0f && collision.collider.GetComponent<Block>())
        {
            if(collision.collider.GetComponent<Block>())
                collision.collider.GetComponent<Block>().DamageBlock();
            Destroy(gameObject);
        }
    }
}
