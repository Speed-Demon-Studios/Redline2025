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
        GameManager.gManager.rActivator.DeactivateRedline();
        foreach (GameObject playerOBJ in GameManager.gManager.players)
        {
            ResetShip(playerOBJ);
        }

        GameManager.gManager.aC.SetParamValue("", 0.0f);

        for (int i = GameManager.gManager.racerObjects.Count - 1; i >= 0; i--)
        {
            GameObject temp = GameManager.gManager.racerObjects[i];

            if (GameManager.gManager.allRacers.Contains(temp))
            {
                GameManager.gManager.allRacers.Remove(temp);
            }

            GameManager.gManager.racerObjects.Remove(temp);

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
        InitializeBeforeRace IBR = playerOBJ.GetComponent<InitializeBeforeRace>();
        AIMoveInputs aiMove = playerOBJ.GetComponent<AIMoveInputs>();
        ShipsControls controls = playerOBJ.GetComponent<ShipsControls>();
        IsShipCollider shipCollider = controls.collisionParent.GetComponentInChildren<IsShipCollider>();
        RacerDetails racerDeets = playerOBJ.GetComponent<RacerDetails>();
        PlayerInputScript playerInputScript = playerOBJ.GetComponent<PlayerInputScript>();
        ActionMappingControl acm = playerOBJ.GetComponent<ActionMappingControl>();

        GameObject shipCollisionObject = shipCollider.gameObject;
        GameObject shipModelObject = controls.shipModel.transform.GetChild(0).gameObject;
        DestroyImmediate(playerOBJ.GetComponent<AIMoveInputs>());

        if (GameManager.gManager.SAM != null)
        {
            GameManager.gManager.SAM.CheckAllAchievementProgress(GameManager.gManager.players[0].GetComponent<RacerDetails>().placement);
        }

        playerInputScript.GetComponent<ShipToWallCollision>().ResetDetails();

        controls.FireList.Clear();
        controls.VariantObject = null;

        shipCollisionObject.transform.parent = null;
        shipModelObject.transform.parent = null;

        Destroy(shipCollisionObject);
        Destroy(shipModelObject);

        playerInputScript.playerReadyInMenu = false;
        playerInputScript.cam.gameObject.SetActive(false);
        playerInputScript.DeActivateVirtualCam();

        racerDeets.finishedRacing = false;
        racerDeets.currentLap = 0;
        racerDeets.totalRaceTimeSeconds = 0;
        racerDeets.totalRaceTimeMinutes = 0;
        racerDeets.currentLapTimeSECONDS = 0;
        racerDeets.currentLapTimeMINUTES = 0;
        racerDeets.quickestLapTimeSECONDS = 99;
        racerDeets.quickestLapTimeMINUTES = 99;

        SparksParticlesController SPC = playerOBJ.GetComponentInChildren<SparksParticlesController>();
        if (SPC != null)
        {
            foreach (SparksTrigger sT in SPC.sparksList)
            {
                if (sT != null)
                {
                    sT.isColliding = false;

                    foreach (VisualEffect sparksOBJ in sT.sparks)
                    {
                        if (sparksOBJ != null)
                        {
                            SPC.DeactivateSparks(sparksOBJ, sT);
                        }
                    }
                }
            }
        }

        controls.ChangeDoneDifficulty(false);
        controls.DeInitialize();

        controls.ResetRedline();

        playerOBJ.GetComponent<ShipsControls>().enabled = false;
        playerOBJ.GetComponent<ShipBlendAnimations>().enabled = false;

        racerDeets.rCS.ClearList();
        ShipToWallCollision stwc = playerOBJ.GetComponent<ShipToWallCollision>();


        foreach (GameObject player in GameManager.gManager.players)
        {
            player.GetComponent<PlayerAudioController>().ResetPlayerAudio();
        }
    }

    public void ResetGameManager()
    {
        GameManager.gManager.isTimeTrial = false;
        GameManager.gManager.firstLoadIntoGame = true;
        GameManager.gManager.raceAboutToStart = false;
        GameManager.gManager.readyForCountdown = false;
        GameManager.gManager.pHandler.racerFinder = new List<RacerDetails>();
        GameManager.gManager.pHandler.racers = new List<RacerDetails>();
        GameManager.gManager.racerObjects = new List<GameObject>();
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
            ActionMappingControl aMC = GameManager.gManager.players[0].GetComponent<ActionMappingControl>();
        }
    }
}
