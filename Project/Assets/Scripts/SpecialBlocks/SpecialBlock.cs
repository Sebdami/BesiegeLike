using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialBlock : MonoBehaviour
{
    [SerializeField]
    protected KeyCode[] keys = new KeyCode[1]; // The default value must be set on the prefab

    protected bool isAttached = true;

    public KeyCode[] Keys
    {
        get
        {
            return keys;
        }

        set
        {
            keys = value;

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
