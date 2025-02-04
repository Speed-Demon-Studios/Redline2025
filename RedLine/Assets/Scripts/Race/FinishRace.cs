using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Net;
using UnityEngine.Assertions.Must;

public class FinishRace : MonoBehaviour
{
    public GameObject mainButton;
    public bool m_allRacersFinished = false;
    [SerializeField] private GameObject[] placementTexts;
    [SerializeField] private GameObject placementWindow;
    private TextMeshProUGUI[] tempSortingTextList;
    private bool m_alreadyShowingPlacements = false;
    private bool m_allRacersCrosedLine = false;
    private bool m_checkingRacersFinished = false;
    private bool m_racersFinishedChecked = false;
    private bool readyToSetSelected = false;
    private bool readyToDisplay = false;
    private bool timingsListsUpdated = false;
    private bool textListSorted = false;
    private int indexForPlacementText;

    public void Inistialize()
    {
        GameManager.gManager.raceFinisher = this;
        m_alreadyShowingPlacements = false;
    }

    public void GoToFinishRace()
    {
        Debug.Log("Pressed");
        placementWindow.SetActive(false);
        GameManager.gManager.mSL.InitializeForMainMenu();
    }

    /// <summary>
    /// Enables and updates information for the placement window, and activates UI input for player 1.
    /// </summary>
    public void ShowFinalPlacements()
    {
        if (m_allRacersFinished == true && m_allRacersCrosedLine == true)
        {
            m_alreadyShowingPlacements = true;
            GameManager.gManager.raceFinished = true;

            GameManager.gManager.aC.musicRaceFinishedTriggerGO.SetActive(false);
            GameManager.gManager.aC.musicRaceFinishedTrigger.Value = 1;
            GameManager.gManager.aC.musicRaceFinishedTriggerGO.SetActive(true);
            GameManager.gManager.aC.musicRaceFinishedTriggerGO.SetActive(false);


            foreach (GameObject racer in GameManager.gManager.allRacers)
            {
                if (GameManager.gManager.players.Contains(racer) == false)
                {
                    RacerDetails rDeets = racer.GetComponent<RacerDetails>();

                    rDeets.finishedRacing = true;
                }
            }
            // Enable the race placement display window and its child objects.
            if (placementWindow.activeSelf == false)
            {
                placementWindow.SetActive(true);
            }

            SelectMainButton();

            StartCoroutine(nameof(ShowFinishText));

            //int currentWins;
            //Steamworks.SteamUserStats.GetStat("FirstPlaceWins", out currentWins);
            //RacerDetails racerDeetsScript = GameManager.gManager.players[0].GetComponent<RacerDetails>();
            //if (racerDeetsScript.placement == 1)
            //{
            //    currentWins++;
            //    bool gotAchievement;
            //    Steamworks.SteamUserStats.GetAchievement("RedlineCadet", out gotAchievement);
            //    Steamworks.SteamUserStats.SetStat("FirstPlaceWins", currentWins);
            //    if (currentWins == 5)
            //    {
            //        gotAchievement = true;
            //        Steamworks.SteamUserStats.SetAchievement("RedlineCadet");
            //    }
            //    if (currentWins >= 1)
            //    {
            //        Steamworks.SteamUserStats.SetAchievement("FirstTimer");
            //    }
            //    Steamworks.SteamUserStats.StoreStats();
            //}
        }
    }

    IEnumerator ShowFinishText()
    {
        for (int i = 0; i < GameManager.gManager.allRacers.Count; i++)
        {

            yield return new WaitForSeconds(0.2f);
            placementTexts[i].SetActive(true); // Activate a text object in the placement window for each racer.
            RacerEntry rEntry = placementTexts[i].GetComponent<RacerEntry>();
            TextMeshProUGUI placementText = rEntry.placementObject; // Get a reference to the PLACEMENT text object.
            TextMeshProUGUI nameText = rEntry.racerNameObject; // Get a reference to the NAME text object.
            TextMeshProUGUI totalTimeText = rEntry.Time1Object; // Get a reference to the TOTAL RACE TIME text object.
            TextMeshProUGUI quickestTimeText = rEntry.Time2Object; // Get a reference to the FASTEST RACE TIME text object.

            // Iterate through all of the racer objects again, this time to update the text objects with each racer's respective name and placement.
            foreach (GameObject racerOBJ in GameManager.gManager.allRacers)
            {
                RacerDetails racerDeets = racerOBJ.GetComponent<RacerDetails>();

                if ((i + 1) == racerDeets.placement)
                {
                    readyToDisplay = true;

                    if (GameManager.gManager.players.Contains(racerOBJ))
                    {
                        if (racerDeets.finishedRacing == true)
                        {


                            float totalMinutes = Mathf.CeilToInt(racerDeets.totalRaceTimeSeconds / 60);
                            float totalSeconds = Mathf.CeilToInt(racerDeets.totalRaceTimeSeconds - totalMinutes % 60);
                            float quickestTime = 0;

                            float quickestSeconds = 0;
                            int quickestMinutes = 0;

                            for (int a = 0; a < quickestTime; a++)
                            {
                                quickestSeconds += 1f;

                                if (quickestSeconds >= 60)
                                {
                                    quickestMinutes += 1;

                                    quickestSeconds = 0;
                                }
                            }


                            placementText.text = racerDeets.placement.ToString();
                            nameText.text = racerDeets.RacerName;
                            totalTimeText.text = string.Format("{0:00}", racerDeets.totalRaceTimeMinutes) + ":" + string.Format("{0:00.00}", racerDeets.totalRaceTimeSeconds);
                            quickestTimeText.text = string.Format("{0:00}", racerDeets.quickestLapTimeMINUTES) + ":" + string.Format("{0:00.00}", racerDeets.quickestLapTimeSECONDS);
                        }
                    }
                    else
                    {
                        if (racerDeets.crossedFinishLine == false)
                        {
                            placementText.text = racerDeets.placement.ToString();
                            nameText.text = racerDeets.RacerName;
                            totalTimeText.text = "DNF";
                            quickestTimeText.text = string.Format("{0:00}", racerDeets.quickestLapTimeMINUTES) + ":" + string.Format("{0:00.00}", racerDeets.quickestLapTimeSECONDS);
                        }
                        else
                        {
                            float totalMinutes = Mathf.CeilToInt(racerDeets.totalRaceTimeSeconds / 60);
                            float totalSeconds = Mathf.CeilToInt(racerDeets.totalRaceTimeSeconds - totalMinutes % 60);

                            placementText.text = racerDeets.placement.ToString();
                            nameText.text = racerDeets.RacerName;
                            totalTimeText.text = string.Format("{0:00}", racerDeets.totalRaceTimeMinutes) + ":" + string.Format("{0:00.00}", racerDeets.totalRaceTimeSeconds);
                            quickestTimeText.text = string.Format("{0:00}", racerDeets.quickestLapTimeMINUTES) + ":" + string.Format("{0:00.00}", racerDeets.quickestLapTimeSECONDS);
                        }
                    }
                }
            }
        }
    }

    private void SelectMainButton()
    {
        ActionMappingControl aMC = GameManager.gManager.players[0].GetComponent<ActionMappingControl>(); // Get a reference to player ones ActionMappingControl script.
        aMC.UpdateActionMapForUI();
        aMC.SwitchActionMapToUI();
        aMC.mES.SetSelectedGameObject(mainButton); // Set player ones MultiplayerEventSystem's selectedGameObject to the mainButton object.
        aMC.mES.firstSelectedGameObject = mainButton;
    }

    public bool AllRacersFinishedCheck()
    {
        m_checkingRacersFinished = true;
        m_racersFinishedChecked = false;

        m_allRacersFinished = true;
        m_allRacersCrosedLine = true;

        // Iterate through all of the PLAYER objects.
        foreach (GameObject racerOBJ in GameManager.gManager.players)
        {
            RacerDetails rDeets = racerOBJ.GetComponent<RacerDetails>();
            if (rDeets.crossedFinishLine == false)
            {
                m_allRacersCrosedLine = false;
            }

            if (rDeets.finishedRacing == false)
            {
                m_allRacersFinished = false;
                break;
            }
        }

        for (int i = 0; i < GameManager.gManager.players.Count; i++)
        {
            m_allRacersFinished = true;
            RacerDetails rDeets = GameManager.gManager.players[i].GetComponent<RacerDetails>();

            if (rDeets.crossedFinishLine == false)
            {
                m_allRacersCrosedLine = false;
            }

            if (rDeets.finishedRacing == false)
            {
                m_allRacersFinished = false;
                break;
            }
        }

        m_racersFinishedChecked = true;
        m_checkingRacersFinished = false;

        if (m_allRacersFinished == true && m_alreadyShowingPlacements == false && m_allRacersCrosedLine == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckAllRacersFinished()
    {
        m_checkingRacersFinished = true;
        m_racersFinishedChecked = false;

        m_allRacersFinished = true;
        m_allRacersCrosedLine = true;

        // Iterate through all of the PLAYER objects.
        foreach (GameObject racerOBJ in GameManager.gManager.players)
        {
            RacerDetails rDeets = racerOBJ.GetComponent<RacerDetails>();
            if (rDeets.crossedFinishLine == false)
            {
                m_allRacersCrosedLine = false;
            }

            if (rDeets.finishedRacing == false)
            {
                m_allRacersFinished = false;
                break;
            }
        }

        for (int i = 0; i < GameManager.gManager.players.Count; i++)
        {
            m_allRacersFinished = true;
            RacerDetails rDeets = GameManager.gManager.players[i].GetComponent<RacerDetails>();

            if (rDeets.crossedFinishLine == false)
            {
                m_allRacersCrosedLine = false;
            }

            if (rDeets.finishedRacing == false)
            {
                m_allRacersFinished = false;
                break;
            }
        }

        m_racersFinishedChecked = true;
        m_checkingRacersFinished = false;

        if (m_allRacersFinished == true && m_alreadyShowingPlacements == false && m_allRacersCrosedLine == true)
        {
            ShowFinalPlacements();
            SelectMainButton();
        }
        return;
    }
}
