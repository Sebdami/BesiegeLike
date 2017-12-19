using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBall : MonoBehaviour {

    private void Start()
    {
        Destroy(gameObject, 10.0f);
    }

    //Temporary
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.relativeVelocity.magnitude);
        if (collision.relativeVelocity.magnitude > 5.0f && !collision.collider.GetComponent<CoreBlock>())
        {
            collision.collider.transform.SetParent(null);
            if(!collision.collider.GetComponent<Rigidbody>())
                collision.collider.gameObject.AddComponent<Rigidbody>().useGravity = false;
            Destroy(gameObject);
        }
    }
}
