using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decisions;

[CreateAssetMenu(fileName = "Strafe Right", menuName = "Actions/Strafe Right")]
public class StrafeToTheRight : Action
{
    public override void ExecuteAction(NPCController npc, ShipsControls controls)
    {
        controls.SetStrafeMultiplier(50f);
        isFinished = true;
    }
}
