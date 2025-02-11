using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decisions;

[CreateAssetMenu(fileName = "Idle", menuName = "Actions/Idle")]
public class Idle : Action
{
    public override void ExecuteAction(NPCController npc, ShipsControls controls)
    {
        Debug.Log("Is Idling");
        isFinished = true;
    }

}
