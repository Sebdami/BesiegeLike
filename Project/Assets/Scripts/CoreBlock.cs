using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreBlock : Block {

    bool isPlayer = false;

    bool isActive = true;

    public bool IsPlayer
    {
        get
        {
            return isPlayer;
        }

        set
        {
            isPlayer = value;
        }
    }

    public override void DamageBlock()
    {
        hp--;
        if(hp <= 0)
        {
            if (isPlayer)
            {
                GameManager.instance.LoseLevel();
            }
            else
            {
                if (!GetComponent<Rigidbody>())
                    gameObject.AddComponent<Rigidbody>().useGravity = false;

                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    if (!transform.parent.GetChild(i).GetComponent<Rigidbody>())
                        transform.parent.GetChild(i).gameObject.AddComponent<Rigidbody>().useGravity = false;
                    transform.parent.GetChild(i).GetComponent<Rigidbody>().AddExplosionForce(1000.0f, transform.position, 500.0f);
                    if (transform.parent.GetChild(i).GetComponent<SpecialBlock>())
                        transform.parent.GetChild(i).GetComponent<SpecialBlock>().IsAttached = false;
                }
                Destroy(transform.parent.gameObject, 5.0f);
                if (isActive)
                    VehicleSpawner.VehiclesRemaining--;
                isActive = false;
                this.enabled = false;
            }
        }
    }

}
