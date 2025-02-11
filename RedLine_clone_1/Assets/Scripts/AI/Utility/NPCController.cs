using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Decisions
{
    [RequireComponent(typeof(AgentBrain))]
    [RequireComponent(typeof(ShipsControls))]
    [RequireComponent(typeof(AIMoveInputs))]
    public class NPCController : MonoBehaviour 
    {
        [Header("Script Refrences")]
        public Action[] listOfActions;
        private AgentBrain brain;
        private AIMoveInputs moveInputs;
        private ShipsControls controls;
        private WhatsInVisionCone visionCones;

        public WhatsInVisionCone GetVisionCones() { return visionCones; }


        private void Start()
        {
            brain = GetComponent<AgentBrain>();
            controls = GetComponent<ShipsControls>();
            visionCones = GetComponentInChildren<WhatsInVisionCone>();
        }

        private void Update()
        {
            if (GameManager.gManager.raceStarted)
            {
                if (brain.currentAction == null || brain.currentAction.isFinished)
                {
                    Debug.Log("Started Picking");
                    StartPick();
                }
                else
                {
                    brain.currentAction.ExecuteAction(this, controls);
                }
            }
        }

        /// <summary>
        /// Starts the cycle to pick the best action
        /// </summary>
        public void StartPick()
        {
            brain.PickTheBestAction(listOfActions);
        }

        public AgentBrain Brain()
        {
            return brain;
        }

    }
}
