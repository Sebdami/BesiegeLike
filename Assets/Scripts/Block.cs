using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* NOT YET UPDATED TO THE NEW ARCHITECTURE (Use PropertyDrawer?)
  #if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Block block = target as Block;
        EditorGUILayout.LabelField("Anchors", EditorStyles.boldLabel);
        for (int i = 0; i < 6; i++)
        {
            block.Anchors[i] = EditorGUILayout.ObjectField(((Anchor)i).ToString(), block.Anchors[i], typeof(Transform), true) as Transform;
        }
    }
}
#endif
*/
public enum Anchor
{
    Right = 0,
    Left,
    Front,
    Back,
    Up,
    Down
}

[System.Serializable]
public class AnchorDirection
{
    [SerializeField]
    Transform[] anchors = new Transform[1];

    public Transform[] Anchors
    {
        get
        {
            return anchors;
        }

        set
        {
            anchors = value;
        }
    }
}

public class Block : MonoBehaviour {
    public static float unitSize = 1.0f;

    [SerializeField]
    int id;
    [SerializeField]
    string blockName;
    [SerializeField]
    Vector3 extent;
    [Header("Hierarchy: Right, Left, Front, Back, Up, Down")]
    [SerializeField]
    AnchorDirection[] anchorsDirection = new AnchorDirection[6];

    public AnchorDirection[] AnchorsDirection
    {
        get
        {
            return anchorsDirection;
        }

        set
        {
            anchorsDirection = value;
        }
    }

    public Vector3 Extent
    {
        get
        {
            return extent;
        }

        set
        {
            extent = value;
        }
    }

    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    public string BlockName
    {
        get
        {
            return blockName;
        }

        set
        {
            blockName = value;
        }
    }

    public Transform GetAnchorFromPositionAndNormal(Vector3 position, Vector3 normal)
    {
        //Find which direction to use to look for our anchor 
        int index = 0;
        normal = transform.worldToLocalMatrix * normal; // Work in local space

        Vector3 closestCartesian = Utils.GetClosestCartesianFromVector(normal);

        if(closestCartesian == Vector3.right)
        {
            index = (int)Anchor.Right;
        }
        else if(closestCartesian == -Vector3.right)
        {
            index = (int)Anchor.Left;
        }
        else if(closestCartesian == Vector3.forward)
        {
            index = (int)Anchor.Front;
        }
        else if(closestCartesian == -Vector3.forward)
        {
            index = (int)Anchor.Back;
        }
        else if(closestCartesian == Vector3.up)
        {
            index = (int)Anchor.Up;
        }
        else if(-closestCartesian == Vector3.up)
        {
            index = (int)Anchor.Down;
        }
        else
        {
            return null;
        }

        //Now find in which unit we are and if an anchor exists here

        AnchorDirection ad = anchorsDirection[index];
        if (ad.Anchors.Length <= 0)
            return null;

        int index2 = -1;

        for (int i = 0; i < ad.Anchors.Length; i++)
        {
            if (ad.Anchors[i] == null)
                continue;
            Vector3 localPos = ad.Anchors[i].worldToLocalMatrix.MultiplyPoint3x4(position);
            if((localPos.x < (unitSize/2.0f)+0.1f && localPos.x > - unitSize / 2.0f-0.1f) && (localPos.y < unitSize/2.0f+0.1f && localPos.y > -unitSize/2.0f-0.1f))
            {
                index2 = i;
                break;
            }
        }

        if(index2 == -1)
            return null;

        return anchorsDirection[index].Anchors[index2]; //Returns null if no anchor has been set at this index;
    }
}
