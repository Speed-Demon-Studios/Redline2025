using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SpawnPlayerInput : MonoBehaviour
{
    public GameObject playerInputScriptObject;
    bool m_alreadyLoadedIn = false;

    private void Awake()
    {
        if (!m_alreadyLoadedIn)
        {
            GameObject tempPlayer = Instantiate(playerInputScriptObject);
            tempPlayer.GetComponent<PlayerInputScript>().SetPlayerInput(this.GetComponent<PlayerInput>());
            tempPlayer.GetComponent<PlayerInputScript>().SetEventSystem(this.GetComponent<MultiplayerEventSystem>());
            tempPlayer.GetComponent<PlayerInputScript>().Inistialize();
            m_alreadyLoadedIn = true;
        }
    }
}
