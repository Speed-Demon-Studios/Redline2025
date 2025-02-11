using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decisions;

namespace Decisions
{
    public abstract class Action : ScriptableObject
    {
        public string Name;
        public float score;
        public bool isFinished;

        public List<Consideration> considerations = new List<Consideration>();

        /// <summary>
        /// This is what the npc will do when in this action
        /// </summary>
        /// <param name="npc"> Gives the action a refrence to the npc controller script </param>
        public abstract void ExecuteAction(NPCController npc, ShipsControls controls);
    }
}
