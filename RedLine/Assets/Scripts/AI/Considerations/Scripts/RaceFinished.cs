using Decisions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Race Finished", menuName = "Considerations/Race Finished")]
public class RaceFinished : Consideration
{
    public override float Score(NPCController npc, ShipsControls controls)
    {
        if (GameManager.gManager.raceFinished && !GameManager.gManager.raceStarted)
            score = 0;
        else
            score = 1;

        return score;
    }
}
