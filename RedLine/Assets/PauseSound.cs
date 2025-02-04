using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EAudioSystem;

public class PauseSound : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.gManager.uAC.GamePauseSound();
    }

    public void PlayConfirmSound()
    {
        GameManager.gManager.uAC.MenuConfirmSound();
    }
}
