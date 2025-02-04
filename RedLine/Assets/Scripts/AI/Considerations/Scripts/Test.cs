using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decisions;

[CreateAssetMenu(fileName = "Test", menuName = "Considerations/Test")]
public class Test : Consideration
{
    public override float Score(NPCController npc, ShipsControls controls)
    {
        score = 0.5f;
        return score;
    }
}
