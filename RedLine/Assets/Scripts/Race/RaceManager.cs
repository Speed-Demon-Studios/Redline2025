using System.Collections;
using TMPro;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private int TotalLaps;
    bool coroutineStarted = false;
    public bool isTimeTrial = false;
    public TMP_Text classText;
    public TMP_Text classTextRed;
    [SerializeField] private GameObject thirdPersonCameras;

    public GameObject ThirdPersonCameras => thirdPersonCameras;

    public void ClassTextChange() { classText.text = GameManager.gManager.classString; classTextRed.text = GameManager.gManager.classString; }

    public int GetTotalLaps() { return TotalLaps; }

    public void Inistialize()
    {
        GameManager.gManager.rManager = this;
        GameManager.gManager.CurrentScene = "Race";
        GameManager.gManager.enablePlayerCams = true;
        GameManager.gManager.raceFinished = false;
        GameManager.gManager.raceStarted = false;

        foreach (GameObject playerOBJ in GameManager.gManager.players)
        {
            playerOBJ.GetComponent<InitializeBeforeRace>().Initialize();
        }

        foreach (GameObject gObj in GameManager.gManager.allRacers)
        {
            if (gObj != null)
            {
                InitializeBeforeRace playerInit = gObj.GetComponent<InitializeBeforeRace>();

                if (playerInit != null)
                {
                    playerInit.InitializeForRace(gObj);
                }

                if (gObj.GetComponent<RacerDetails>() != null)
                    gObj.GetComponent<RacerDetails>().rCS.CallSpawnCollider();
            }
        }

        if(GameManager.gManager.players.Count == 3)
        {
            thirdPersonCameras.SetActive(true);
        }
    }

    public void StartRace()
    {
        GameManager gMAN = GameManager.gManager;
        gMAN.nRandomiser.AssignRacerNames();
        gMAN.raceStarted = true;
        
        for (int i = 0; i < gMAN.players.Count; i++)
        {
            ActionMappingControl AMC = gMAN.players[i].GetComponent<ActionMappingControl>();

            AMC.UpdateActionMapForRace();
        }

        gMAN.rActivator.ActivateRedline();
        gMAN.EnableRMovement();
    }

    public void FinishRace()
    {
        GameManager gMAN = GameManager.gManager;
        gMAN.DisableRMovement();

        for (int i = 0; i < gMAN.players.Count; i++)
        {
            ActionMappingControl AMC = gMAN.players[i].GetComponent<ActionMappingControl>();
            
            AMC.UpdateActionMapForUI();
        }

        if(GameManager.gManager.players.Count != 3)
            thirdPersonCameras.SetActive(false);

        Invoke(nameof(CallFinalPlacements), 2f);
    }

    public void CallFinalPlacements()
    {
        GameManager.gManager.raceFinished = true;
        GameManager.gManager.raceFinisher.ShowFinalPlacements();
    }

    public void DisableFinishedRacerMovement(RacerDetails racer = null)
    {
        if (racer == null)
        {
            for (int i = 0; i < GameManager.gManager.players.Count; i++)
            {
                RacerDetails racerDeets = GameManager.gManager.players[i].GetComponent<RacerDetails>();

                if (racerDeets.finishedRacing == true && racerDeets.crossedFinishLine == true)
                {
                    GameManager.gManager.DisableRMovement(GameManager.gManager.players[i]);
                }
            }
        }
        else
        {
            if(racer.finishedRacing == true && racer.crossedFinishLine == true)
            {
                GameManager.gManager.DisableRMovement(racer.gameObject);
            }
        }
    }

    public void LapComplete(RacerDetails racer)
    {
        if (GameManager.gManager.isTimeTrial == false)
        {
            if (racer.currentLap >= TotalLaps)
            {
                racer.crossedFinishLine = true;
                racer.finishedRacing = true;
            }

            if (racer.transform.gameObject.GetComponent<PlayerInputScript>() != null && racer.currentLap == (TotalLaps - 1) && GameManager.gManager.finalLap == false)
            {
                GameManager.gManager.finalLap = true;

                GameManager.gManager.aC.SetParamValue("", 1.60f);
            }

            if (racer.currentLap < TotalLaps)
            {
                racer.currentLap += 1;
            }
            
            if (racer.currentLap > 0)
            {
                Debug.Log("Lap " + (racer.currentLap - 1) + " Completed!");
            }

            if (GameManager.gManager.raceFinisher.AllRacersFinishedCheck() == true)
            {
                FinishRace();
            }

            if (GameManager.gManager.players.Contains(racer.gameObject))
                DisableFinishedRacerMovement();
            else
                DisableFinishedRacerMovement(racer);
        }
        else
        {
            if (racer.currentLap == 0)
            {
                racer.currentLap = 1;
            }
        }

        return;
    }
}
