using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decisions;

[CreateAssetMenu(fileName = "Strafe Left", menuName = "Actions/Strafe Left")]
public class StrafeToLeft : Action
{
    public override void ExecuteAction(NPCController npc, ShipsControls controls)
    {
        controls.SetStrafeMultiplier(-50f);
        isFinished = true;
    }
}
