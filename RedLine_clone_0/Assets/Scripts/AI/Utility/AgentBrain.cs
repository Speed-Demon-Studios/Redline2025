using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Decisions
{
    [RequireComponent(typeof(ShipsControls))]
    [RequireComponent(typeof(NPCController))]
    public class AgentBrain : MonoBehaviour
    {
        public Action currentAction;
        public Action nextAction;
        public bool finishedDecision;

        private NPCController npc;
        private ShipsControls controls;

        // Start is called before the first frame update
        void Start()
        {
            npc = GetComponent<NPCController>();
            controls = GetComponent<ShipsControls>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Starts the pick for the best action
        /// </summary>
        /// <param name="actions"> A list of actions that the NPC has </param>
        public void PickTheBestAction(Action[] actions)
        {
            if(nextAction == null)
            {
                float score = 0f;
                int nextBestIndex = 0;
                for (int i = 0; i < actions.Length; i++)
                {
                    if (ActionScore(actions[i]) > score)
                    {
                        nextBestIndex = i;
                        score = actions[i].score;
                    }
                }

                currentAction = actions[nextBestIndex];
                finishedDecision = true;
            }
            else
            {
                currentAction = nextAction;
                currentAction.isFinished = false;
                finishedDecision = true;
                nextAction = null;
            }

        }

        /// <summary>
        /// Scores each action in the action list
        /// </summary>
        /// <param name="action"> A list of actions that the NPC has </param>
        /// <returns></returns>
        public float ActionScore(Action action)
        {
            action.isFinished = false;
            float score = 1f;
            for (int i = 0; i < action.considerations.Count; i++)
            {
                float considerationScore = action.considerations[i].Score(npc, controls);
                score *= considerationScore;

                if (score == 0)
                {
                    action.score = 0;
                    return action.score;
                }
            }

            float originalScore = score;
            float modFactor = 1 - (1 / action.considerations.Count);
            float makeupValue = (1 - originalScore) * modFactor;
            action.score = originalScore + (makeupValue * originalScore);

            return action.score;
        }
    }
}
