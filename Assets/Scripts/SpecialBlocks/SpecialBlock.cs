using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialBlock : MonoBehaviour
{
    [SerializeField]
    public KeyCode key; // Set the default value on the prefab
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
}
