using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialBlock : MonoBehaviour
{
    [SerializeField]
    protected KeyCode key; // Set the default value on the prefab

    protected bool isAttached = true;

    public KeyCode Key
    {
        get
        {
            return key;
        }

        set
        {
            key = value;

        }
    }

    public bool IsAttached
    {
        get
        {
            return isAttached;
        }

        set
        {
            isAttached = value;
        }
    }
}
