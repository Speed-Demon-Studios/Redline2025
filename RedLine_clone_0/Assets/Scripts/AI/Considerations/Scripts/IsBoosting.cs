using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decisions;

[CreateAssetMenu(fileName = "Is Boosting", menuName = "Considerations/Is Boosting")]
public class IsBoosting : Consideration
{
    public override float Score(NPCController npc, ShipsControls controls)
    {
        if (controls.ReturnIsBoosting)
            score = 0;
        else
            score = 1;

        return score;
    }
}
