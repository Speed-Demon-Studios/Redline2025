using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseButtonFirst;

    // Start is called before the first frame update
    public void Inistialize()
    {
        GameManager.gManager.pMenu = this;
    }

    public void SwitchPlayerOneButton(int playerNumber)
    {
        GameManager.gManager.players[playerNumber].GetComponent<ActionMappingControl>().GetmES().SetSelectedGameObject(pauseButtonFirst);
    }

    public void StartTimeAgain(bool switchs)
    {
        GameManager.gManager.StartTime(switchs);
    }

    public void QuitToMenu()
    {
        foreach(GameObject playerobj in GameManager.gManager.players)
        {
            playerobj.GetComponent<PlayerInputScript>().GetShipControls().gameObject.GetComponent<RacerDetails>().crossedFinishLine = true;
            playerobj.GetComponent<PlayerInputScript>().GetShipControls().gameObject.GetComponent<RacerDetails>().finishedRacing = true;
            playerobj.GetComponent<PlayerInputScript>().uiController.FinishPopUp();
        }
        if (GameManager.gManager.raceFinisher.AllRacersFinishedCheck())
        {
            GameManager.gManager.rManager.FinishRace();
        }
    }
}
