using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decisions;

[CreateAssetMenu(fileName = "Has Boost", menuName = "Considerations/Has Boost")]
public class HasBoost : Consideration
{
    public bool Invert;
    public override float Score(NPCController npc, ShipsControls controls)
    {
        if (!Invert)
        {
            if (controls.ReturnBoostLevel > 1)
            {
                score = 1;
            }
            else
            {
                score = 0;
            }
        }
        else
        {
            if (controls.ReturnBoostLevel > 0)
            {
                score = 0;
            }
            else
            {
                score = 1;
            }
        }

        return score;
    }
}
