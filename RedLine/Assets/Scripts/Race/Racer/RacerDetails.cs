using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerDetails : MonoBehaviour
{
    [Header("References & Variables")]
    public GameObject startOfTunnelOBJ;
    public GameObject endOfTunnelOBJ;
    public GameObject corkscrewReset1;
    public GameObject corkscrewReset2;
    public IList<GameObject> resetNormalOBJs = new List<GameObject>();

    [Header("Script References")]
    public RedlineColliderSpawner rCS;

    [Space]
    [Header("LapTime Lists")]
    public float quickestLapTimeSECONDS = 99;
    public float quickestLapTimeMINUTES = 99;

    [Space]
    [Header("Integer Variables")]
    public int placement = 0;
    public int currentLap = 0;
    public int currentCheckpoint = 0;

    [Space]
    [Header("Float Variables")]
    public float distanceToCheckpoint;
    public float totalRaceTimeSeconds = 0;
    public float totalRaceTimeMinutes = 0;
    public float currentLapTimeSECONDS = 0;
    public float currentLapTimeMINUTES = 0;

    [Space]
    [Header("Bool Variables")]
    public bool finishedRacing = false;
    public bool crossedFinishLine = false;

    [Space]
    [Header("String Variables")]
    public string RacerName = "";

    // !!********************************************************!!
    // !! Only PRIVATE variables and references past this point. !!
    // !!********************************************************!!

    private CheckpointHandler m_cHandler; // Reference to the CheckpointHandler.cs script.

    private bool nameSet = false;         // Whether or not the racers name has been set.

    private void Awake()
    {
        if (GameManager.gManager.players.Contains(this.gameObject))
        {
            //GameManager.gManager.uAC.PlayerJoinSound();
        }
    }


    /// <summary>
    /// Calculates the distance to the next checkpoint
    /// </summary>
    public float NextCheckpointDistance()
    {
        m_cHandler = GameManager.gManager.checkpointParent;
        var nextCheckpoint = m_cHandler.GetCheckpoint(currentCheckpoint);
        distanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.transform.position);
        return distanceToCheckpoint;
    }


    public void ResetRacerVariables()
    {
        finishedRacing = false;
        currentCheckpoint = 0;
        currentLap = 0;
        placement = 0;
    }

    private void Update()
    {
        if (endOfTunnelOBJ != null)
        {
            if (Vector3.Distance(this.gameObject.transform.position, endOfTunnelOBJ.transform.position) <= 35f && Vector3.Distance(this.gameObject.transform.position, endOfTunnelOBJ.transform.position) > 1f)
            {
                ShipsControls sControls = this.gameObject.GetComponent<ShipsControls>();
                sControls.SetRotationToTrack(endOfTunnelOBJ.GetComponent<EndOfTunnel>().point);
            }
        }

        if (corkscrewReset1 != null)
        {
            if (Vector3.Distance(this.gameObject.transform.position, corkscrewReset1.transform.position) <= 40f && Vector3.Distance(this.gameObject.transform.position, corkscrewReset1.transform.position) > 1f)
            {
                ShipsControls sControls = this.gameObject.GetComponent<ShipsControls>();
                sControls.SetRotationToTrack(corkscrewReset1.GetComponent<CorkscrewReset>().point);
            }
        }

        if (corkscrewReset2 != null)
        {
            if (Vector3.Distance(this.gameObject.transform.position, corkscrewReset2.transform.position) <= 40f && Vector3.Distance(this.gameObject.transform.position, corkscrewReset2.transform.position) > 1f)
            {
                ShipsControls sControls = this.gameObject.GetComponent<ShipsControls>();
                sControls.SetRotationToTrack(corkscrewReset2.GetComponent<CorkscrewReset>().point);
            }
        }



        if (currentLap > 0 && finishedRacing == false)
        {
            currentLapTimeSECONDS += 1 * Time.deltaTime;
            totalRaceTimeSeconds += 1 * Time.deltaTime;

            if (totalRaceTimeSeconds >= 60.0f)
            {
                totalRaceTimeSeconds = 0.0f;
                totalRaceTimeMinutes += 1.0f;
            }
            
            if (currentLapTimeSECONDS >= 60.0f)
            {
                currentLapTimeSECONDS = 0.0f;
                currentLapTimeMINUTES += 1;
            }
        }

        if (GameManager.gManager.raceStarted == false && nameSet == false)
        {
            nameSet = true;
            for (int i = 0; i < GameManager.gManager.players.Count; i++)
            {
                if (GameManager.gManager.players[i] == this.gameObject)
                {
                    //Debug.Log("Player Index: " + i);
                    RacerName = ("Player" + (i + 1));
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.ToLower() == "checkpoint")
        {
            CheckpointHandler cHandler = GameManager.gManager.checkpointParent;
            if (other.transform == cHandler.GetCheckpoint(currentCheckpoint))
            {
                currentCheckpoint = cHandler.GetNextIndex(currentCheckpoint);

                if (other.TryGetComponent(out CheckpointTrigger trigger))
                {
                    if (trigger.finalCheckpoint == true)
                    {
                        float currnetLapSecs = currentLapTimeSECONDS;
                        float currentLapMins = currentLapTimeMINUTES;
                        if (currentLap > 0)
                        {
                            GameManager.gManager.timingsListUpdated = false;
                            if(currentLapTimeMINUTES < quickestLapTimeMINUTES)
                            {
                                quickestLapTimeMINUTES = currentLapTimeMINUTES;
                                quickestLapTimeSECONDS = currentLapTimeSECONDS;
                            }
                            else if(currentLapTimeSECONDS < quickestLapTimeSECONDS)
                            {
                                quickestLapTimeSECONDS = currentLapTimeSECONDS;
                            }

                            currentLapTimeMINUTES = 0;
                            currentLapTimeSECONDS = 0;
                            GameManager.gManager.timingsListUpdated = true;
                        } 
                        if (currentLap != 0 && currentLap != GameManager.gManager.rManager.GetTotalLaps() && this.gameObject.GetComponent<PlayerInputScript>() != null)
                        {
                            this.gameObject.GetComponent<PlayerInputScript>().uiController.newLapAnim.gameObject.SetActive(true);
                            this.gameObject.GetComponent<PlayerInputScript>().uiController.newLapAnim.SetTrigger("NewLap");
                            if (GameManager.gManager.rManager.GetTotalLaps() - 1 == currentLap)
                            {
                                this.gameObject.GetComponent<PlayerInputScript>().uiController.newLapAnim.GetComponent<NewLapInfo>().lapText.text = "Final Lap";
                                this.gameObject.GetComponent<PlayerInputScript>().uiController.newLapAnim.GetComponent<NewLapInfo>().lapTextRed.text = "Final Lap";
                            }
                            else
                            {
                                this.gameObject.GetComponent<PlayerInputScript>().uiController.newLapAnim.GetComponent<NewLapInfo>().lapText.text = "New Lap";
                                this.gameObject.GetComponent<PlayerInputScript>().uiController.newLapAnim.GetComponent<NewLapInfo>().lapTextRed.text = "New Lap";
                            }
                            this.gameObject.GetComponent<PlayerInputScript>().uiController.newLapAnim.GetComponent<NewLapInfo>().lapTextTime.text = string.Format("{0:00}", currentLapMins) + ":" + string.Format("{0:00.00}", currnetLapSecs);
                        }
                        GameManager.gManager.rManager.LapComplete(this);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                return;
            }
        }
    }

}
