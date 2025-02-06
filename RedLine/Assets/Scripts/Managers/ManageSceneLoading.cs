using EAudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class ManageSceneLoading : MonoBehaviour
{
    bool reloadingmenu = false;
    bool coroutineStarted = false;

    public void InitializeForMainMenu()
    {
        reloadingmenu = true;

        foreach (GameObject playerOBJ in GameManager.gManager.players)
        {
            ResetShip(playerOBJ);
        }

        GameManager.gManager.aC.SetParamValue("", 0.0f);

        for (int i = GameManager.gManager.aiRacerObjects.Count - 1; i >= 0; i--)
        {
            GameObject temp = GameManager.gManager.aiRacerObjects[i];

            if (GameManager.gManager.allRacers.Contains(temp))
            {
                GameManager.gManager.allRacers.Remove(temp);
            }

            GameManager.gManager.aiRacerObjects.Remove(temp);

            Destroy(temp);
        }

        ResetGameManager();

        reloadingmenu = true;
        if (coroutineStarted == false)
        {
            coroutineStarted = true;
            StartCoroutine(LoadMenuScene());
        }
    }

    public void ResetShip(GameObject playerOBJ)
    {
        GameObject tempShip = playerOBJ.GetComponent<PlayerInputScript>().GetShipControls().gameObject;

        playerOBJ.GetComponent<PlayerInputScript>().SetShipControls(null);
        playerOBJ.GetComponent<PlayerInputScript>().uiController = null;

        if (GameManager.gManager.playerShips.Contains(tempShip))
            GameManager.gManager.playerShips.Remove(tempShip);
        if (GameManager.gManager.allRacers.Contains(tempShip))
            GameManager.gManager.playerShips.Remove(tempShip);

        Destroy(tempShip);

        playerOBJ.GetComponent<PlayerInputScript>().playerReadyInMenu = false;
    }

    public void ResetGameManager()
    {
        GameManager.gManager.isTimeTrial = false;
        GameManager.gManager.firstLoadIntoGame = true;
        GameManager.gManager.raceAboutToStart = false;
        GameManager.gManager.readyForCountdown = false;
        GameManager.gManager.pHandler.racerFinder = new List<RacerDetails>();
        GameManager.gManager.pHandler.racers = new List<RacerDetails>();
        GameManager.gManager.aiRacerObjects = new List<GameObject>();
        GameManager.gManager.racersPlaced = false;
        GameManager.gManager.raceFinished = false;
        GameManager.gManager.raceStarted = false;
        GameManager.gManager.racersAdded = false;
        GameManager.gManager.pHandler.racersAdded = false;
        GameManager.gManager.countdownIndex = 5;
        GameManager.gManager.namesAssigned = false;
        GameManager.gManager.nRandomiser.usedNames = new List<string>();
        GameManager.gManager.redlineActivated = false;
        GameManager.gManager.aC.musicRaceFinishedTriggerGO.SetActive(false);
        GameManager.gManager.aC.musicRaceFinishedTrigger.Value = 0;
        GameManager.gManager.aC.musicRaceFinishedTriggerGO.SetActive(true);
        GameManager.gManager.aC.musicRaceFinishedTriggerGO.SetActive(false);
        GameManager.gManager.finalLap = false;
    }

    IEnumerator LoadMenuScene()
    {
        yield return new WaitForEndOfFrame();

        foreach (GameObject collider in GameObject.FindGameObjectsWithTag("Redline"))
        {
            DestroyImmediate(collider.gameObject);
        }


        PlayerPrefs.SetInt("SceneID", 1);
        SceneManager.LoadSceneAsync(3);

        coroutineStarted = false;
        StopCoroutine(LoadMenuScene());
    }

    public void SetPlayerUIInputMM()
    {
        if (reloadingmenu == true)
        {
            reloadingmenu = false;
        }
    }
}
