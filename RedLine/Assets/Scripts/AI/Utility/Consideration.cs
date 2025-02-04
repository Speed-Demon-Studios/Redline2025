using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Decisions
{
    public abstract class Consideration : ScriptableObject
    {
        [Tooltip("This curve will determin the score of the consideration and can be changed in the inspector")]
        public AnimationCurve curve;
        public string Name;
        public float score;

        /// <summary>
        /// This is called to score the consideration and how it is score is up to you
        /// </summary>
        /// <param name="npc"> Sends through the npc controller</param>
        /// <returns> a float which is the score </returns>
        public abstract float Score(NPCController npc, ShipsControls controls);
    }

}
