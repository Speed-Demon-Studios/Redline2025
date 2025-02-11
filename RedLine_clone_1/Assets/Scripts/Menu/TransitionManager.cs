using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public Animator anim;

    public void Transition(string whichTransition)
    {
        anim.SetTrigger(whichTransition);
    }
}
