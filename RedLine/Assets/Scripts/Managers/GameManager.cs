using System.Collections.Generic;
using UnityEngine;
using MenuManagement;
using EAudioSystem;
using FMODUnity;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public string CurrentScene;

    public RedlineActivator rActivator;
    public NameRandomiser nRandomiser;
    public RaceCountdown raceCountdown;
    public UIControllerInput uiCInput;
    public CheckpointHandler checkpointParent;
    public PositionHandler pHandler;
    public ManageSceneLoading mSL;
    public RaceManager rManager;
    public FinishRace raceFinisher;
    public ControllerHaptics hapticsController;
    public PauseMenu pMenu;
    public Nodes startNode;
    public UIAudioController uAC;
    public StudioEventEmitter musicEmitter;
    public AudioController aC;
    public PlayerInputManager PAM;

    public GameObject[] StartingPoints;

    public List<GameObject> players;
    public IList<GameObject> allRacers = new List<GameObject>();
    public IList<GameObject> racerObjects = new List<GameObject>();

    public bool finalLap = false;
    private int m_playerInteractingWithPause;

    public bool resetRacerVariables = false;
    public bool namesAssigned = false;

    public bool readyForCountdown = false;
    public bool startCamerasFinished = false;

    public bool raceStarted = false;
    public bool raceFinished = false;
    public bool raceAboutToStart = false;

    public bool racersAdded = false;
    public bool racersPlaced = false;
    public bool isTimeTrial = false;

    public bool disablePlayerCams = false;
    public bool enablePlayerCams = false;

    public bool disableRacerMovement = false;
    public bool enableRacerMovement = false;

    public bool redlineActivated = false;
    
    public bool indexListSorted = true;

    public bool timingsListUpdated = false;

    public bool timeStopped = false;

    public bool firstLoadIntoGame = false;

    public int countdownIndex = 2;
    public int neededLaps = 0;
    public int numberOfPlayers = 0;
    public float difficultyChange = 1;
    public float catchUpMultiplier = 1;
    public float catchUpTopCap = 1;
    public string classString = "Debut";

    public float m_masterVolume = 1.0f;
    public float m_sfxVolume = 1.0f;
    public float m_musicVolume = 3.0f;

    public void ChangeCatchUp(float change) { catchUpMultiplier = change; }
    public void ChangeDifficulty(float change) { difficultyChange = change; ChangeClassString(); }
    public void ChangeClassString()
    {
        switch (difficultyChange)
        {
            case 0.8f:
                classString = "Debut Class";
                break;
            case 1f:
                classString = "Pro Class";
                break;
            case 1.2f:
                classString = "Elite Class";
                break;
        }
    }

    public static GameManager gManager { get; private set; }

    void Awake()
    {
        if (gManager != null && gManager != this)
        {
            Destroy(this);
        }
        else
        {
            gManager = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (resetRacerVariables == true)
        {
            resetRacerVariables = false;
            foreach (GameObject playerOBJ in players)
            {
                RacerDetails rDeets = playerOBJ.GetComponent<RacerDetails>();

                rDeets.ResetRacerVariables();
            }
        }

        if (disablePlayerCams == true)
        {
            disablePlayerCams = false;

            if (players.Count > 0)
            {
                foreach (GameObject playerOBJ in players)
                {
                    InitializeBeforeRace rDeets = playerOBJ.GetComponent<InitializeBeforeRace>();

                    if (rDeets.playerCamOBJECT != null)
                    {
                        rDeets.playerCamOBJECT.SetActive(false);
                    }
                }
            }
        }

        if (enablePlayerCams == true)
        {
            enablePlayerCams = false;

            if (players.Count > 0)
            {
                foreach (GameObject playerOBJ in players)
                {
                    InitializeBeforeRace rDeets = playerOBJ.GetComponent<InitializeBeforeRace>();

                    if (rDeets.playerCamOBJECT != null)
                    {
                        rDeets.playerCamOBJECT.SetActive(true);
                    }
                }
            }
        }
    }

    public void StopTime(int playerNumber)
    {
        pMenu.transform.GetChild(0).gameObject.SetActive(true);
        m_playerInteractingWithPause = playerNumber;
        players[playerNumber].GetComponent<ActionMappingControl>().UpdateActionMapForUI();
        pMenu.SwitchPlayerOneButton(playerNumber);
        Time.timeScale = 0;
    }

    public void StartTime(bool switchs)
    {
        if(switchs)
            players[m_playerInteractingWithPause].GetComponent<ActionMappingControl>().UpdateActionMapForRace();
        Time.timeScale = 1;
    }

    public void EnableRMovement()
    {
        foreach (GameObject racerOBJ in allRacers)
        {
            InitializeBeforeRace rDeets = racerOBJ.GetComponent<InitializeBeforeRace>();
            Rigidbody rB = racerOBJ.GetComponent<Rigidbody>();
            rDeets.EnableRacerMovement();
            rB.isKinematic = false;
            racerOBJ.GetComponent<ShipsControls>().ResetAcceleration();
            racerOBJ.GetComponent<ShipsControls>().ResetAngles(0,0,0);
            racerOBJ.GetComponent<ShipsControls>().SetBrakeMultiplier(0);
            racerOBJ.GetComponent<ShipsControls>().SetSpeedMultiplier(0);
            racerOBJ.GetComponent<ShipsControls>().SetTurnMultipliers(0);
            racerOBJ.GetComponent<ShipsControls>().SetStrafeMultiplier(0);
        }
    }

    public void DisableRMovement(GameObject racer = null)
    {
        if (racer != null)
        {
            ShipsControls sControls = racer.GetComponent<ShipsControls>();

            if(racer.GetComponent<PlayerInputScript>() != null)
                racer.GetComponent<PlayerInputScript>().uiController.FinishPopUp();

            sControls.ResetAcceleration();
            AIMoveInputs temp;
            if (!racer.TryGetComponent(out temp))
            {
                AIMoveInputs aiMove = racer.AddComponent<AIMoveInputs>();
                aiMove.SetVariant(sControls.VariantObject);
                aiMove.desiredNode = startNode;
            }


        }
        else if (racer == null)
        {
            foreach (GameObject racerOBJ in racerObjects)
            {
                InitializeBeforeRace rDeets = racerOBJ.GetComponent<InitializeBeforeRace>();
                Rigidbody rB = racerOBJ.GetComponent<Rigidbody>();
                ShipsControls sControls = racerOBJ.GetComponent<ShipsControls>();

                sControls.ResetAcceleration();
                AIMoveInputs test;
                if (!racerOBJ.TryGetComponent<AIMoveInputs>(out test))
                {
                    AIMoveInputs aiMove = racerOBJ.AddComponent<AIMoveInputs>();

                    aiMove.desiredNode = startNode;
                }
            }
        }
    }

    public void AddToNumberOfPlayers() { numberOfPlayers += 1; }
}
