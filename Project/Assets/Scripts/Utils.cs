using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    public static Vector3 GetClosestCartesianFromVector(Vector3 vec)
    {
        //Find the closest cartesian vector by comparing the biggest value
        float xn = Mathf.Abs(vec.x);
        float yn = Mathf.Abs(vec.y);
        float zn = Mathf.Abs(vec.z);

        Vector3 CartesianVector;

        if ((xn >= yn) && (xn >= zn))
        {
            CartesianVector = vec.x > 0 ? Vector3.right : -Vector3.right;
        }
        else if ((yn > xn) && (yn >= zn))
        {

            CartesianVector = vec.y > 0 ? Vector3.up : -Vector3.up;
        }
        else if ((zn > xn) && (zn > yn))
        {
            CartesianVector = vec.z > 0 ? Vector3.forward : -Vector3.forward;
        }
        else
        {
            CartesianVector = Vector3.zero;
        }

        return CartesianVector;
    }
}
