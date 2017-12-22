using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreBlock : Block {

    bool isPlayer = false;

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
            if (isPlayer) // TO DO LATER : Check if player lost or won or none
                GameManager.instance.ExitPlayMode();
            else
                GameManager.instance.ExitPlayMode();
        }
    }
}
