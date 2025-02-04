using Cinemachine;
using MenuManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class PlayerInputScript : MonoBehaviour
{
    [Header("References")]
    public PlayerInput player;
    public MultiplayerEventSystem eventSystem;
    public PlayerUiControl uiController;
    private ShipsControls m_shipControls;
    private Gamepad m_playerGamepad;
    private GameManager gMan;
    private ShipSelection m_selection;

    [Header("Player Camera Stuff")]
    public List<int> playerLayers = new();
    public List<LayerMask> ignoreLayers = new();
    public Camera cam;
    [SerializeField] private CinemachineVirtualCamera m_virtualCam;

    [Header("Camera Values")]
    private float m_currentFOV;
    private float m_desiredFOV;
    public float lerpTime;
    public float minFOV;
    public float maxFOV;

    private bool m_alreadyLoadedIn = false;
    private int m_playerNumber;
    public bool playerReadyInMenu;


    /////////////////////////////////////////////////////////////////
    ///                                                          ///
    ///      All of the getters and setters in this script       ///
    ///                                                          ///
   /////////////////////////////////////////////////////////////////
    public void SetPlayerNumber(int number) { m_playerNumber = number; }
    public int GetPlayerNumber() { return m_playerNumber; }

    public void SetSelection(ShipSelection selection) { m_selection = selection; }
    public ShipSelection GetShipSelection() { return m_selection; }
    public Gamepad GetPlayerGamepad() { return m_playerGamepad; }
    public ShipSelection ReturnShipSelection() { return m_selection; }
    public void ActivateVirtualCam() { m_virtualCam.gameObject.SetActive(true); }
    public void DeActivateVirtualCam() { m_virtualCam.gameObject.SetActive(false); }

    // Start is called before the first frame update
    private void Awake()
    {
        if(!m_alreadyLoadedIn)
            Inistialize();
    }

    public void Inistialize()
    {
        m_shipControls = GetComponentInParent<ShipsControls>(); // getting the objects shipControls script which would be on the parent
        gMan = GameManager.gManager; // seting a reference to the GameManager
        //m_virtualCam.gameObject.SetActive(true);

        if (!m_alreadyLoadedIn)
        {
            GameManager.gManager.uiCInput.AddToPlayers();

            if (gMan != null) // if there is a GameManager
                if(!gMan.players.Contains(this.gameObject))
                    gMan.players.Add(gameObject); // add this object to the players list in GameManager

            if (gMan != null) // if there is a GameManager
                m_playerNumber = gMan.numberOfPlayers; // Set this objects player number

            if (m_playerNumber == 1)
            {
                GameManager.gManager.uiCInput.GetMenuManager().SetGameLoaded(false);
                GameManager.gManager.uiCInput.GetMenuManager().PressStart();
            }
        }


        gMan.uiCInput.sssManager.ReOrderShipSelection();

        if (player != null) // chech for player so that we dont get error later                
        {
            AssignController(); // calls a function that help setup controllers for feedback   
        }
        if (!m_shipControls.isTestShip)
        {
            SwitchCameraLayers(m_playerNumber - 1);
        }

        m_alreadyLoadedIn = true;
    }

    public void SwitchCameraLayers(int playerNumber)
    {
        if (m_virtualCam != null && playerLayers.Count > 0)
            m_virtualCam.gameObject.layer = playerLayers[playerNumber];
        if (cam != null && playerLayers.Count > 0)
            cam.cullingMask = ignoreLayers[playerNumber];
    }

    /// <summary>
    /// Gets called when a controller looses connection
    /// </summary>
    public void PlayerDisconnect()
    {
        if (GameManager.gManager.CurrentScene != "Race")
        {
            // this for loop will go through all other player and set their player number down 1 so if player 2 disconnects then player 3
            // will now become player 2
            for (int i = m_playerNumber - 1; i < GameManager.gManager.players.Count; i++)
            {
                if (GameManager.gManager.players[i].GetComponent<PlayerInputScript>().GetPlayerNumber() > m_playerNumber)
                {
                    GameManager.gManager.players[i].GetComponent<PlayerInputScript>().SetPlayerNumber(i);
//                    GameManager.gManager.players[i].GetComponent<PlayerInputScript>().player.
                }
            }
            foreach(Transform child in GameManager.gManager.uiCInput.GetMenuManager().gameObject.transform)
            {
                SetMenu temp;
                if(child.TryGetComponent<SetMenu>(out temp))
                {
                    if(temp.typeOfMenu == MenuType.ShipSelectionReady)
                    {
                        temp.menuStartButtons.Remove(m_selection.GetComponentInChildren<Button>().gameObject);
                    }
                }
            }
            foreach (InputDevice device in InputUser.GetUnpairedInputDevices())
            {
                InputUser.GetUnpairedInputDevices().Remove(device);
            }

            Destroy(m_selection.gameObject);

            if (GameManager.gManager.players.Contains(this.gameObject)) // if the players list contains this object
            {
                GameManager.gManager.players.Remove(this.gameObject); // then remove it from the list
            }
            if (GameManager.gManager.allRacers.Contains(this.gameObject)) // if the playerObjects list contains this object
            {
                GameManager.gManager.allRacers.Remove(this.gameObject); // then remove the object from the list
            }
            GameManager.gManager.numberOfPlayers -= 1;

            // -1 from number of player in the uicontroller script
            GameManager.gManager.uiCInput.SetNumberOfPlayers(GameManager.gManager.uiCInput.GetNumberOfPlayers() - 1);

            gMan.uiCInput.sssManager.ReOrderShipSelection();
            gMan.uiCInput.GetMenuManager().SetButtons(gMan.uiCInput.GetMenuManager().GetCurrentMenu());
            GameManager.gManager.uiCInput.GetMenuManager().BackGroundPanelForSelection();

            for (int i = m_playerNumber - 1; i < GameManager.gManager.players.Count; i++)
            {
                GameManager.gManager.players[i].GetComponent<PlayerInputScript>().SwitchCameraLayers(i);
            }

            Destroy(this.gameObject);
        }
        else
        {

        }
    }
    
    /// <summary>
    /// assigning controllers so that they can have vibration feedback
    /// </summary>
    private void AssignController()
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i] != null && player.GetDevice<Gamepad>() != null)
            {
                if (Gamepad.all[i].deviceId == player.GetDevice<Gamepad>().deviceId)
                {
                    Debug.Log("FOUND CONTROLLER (Device ID: " + player.GetDevice<Gamepad>().deviceId + ")");
                    m_playerGamepad = Gamepad.all[i];
                    break;
                }
            }
        }
        return;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.gManager.raceStarted == true && GameManager.gManager.raceFinished == false) // if the race has started and not finished
        {
            CalculateFOV(); // calculate the FOV for the camera
        }
    }

    private void CalculateFOV()
    {
        // calculating how fast the ships going compaired to the top speed as a percentage                                  
        float speedPercentage = m_shipControls.RB.velocity.magnitude / m_shipControls.VariantObject.DefaultMaxSpeed;
        // if the ship is moving slightly
        if(speedPercentage > 0.001)
        {
            m_desiredFOV = ((maxFOV - minFOV) * speedPercentage) + minFOV; // calculate the desiredFOV                      
        }
        else // if its not moving
        {
            m_desiredFOV = minFOV; // then the FOV is now the min it can go
        }
        //m_desiredPOV = Mathf.Lerp(minPOV, maxPOV, speedPercentage);

        m_currentFOV = Mathf.Lerp(m_currentFOV, m_desiredFOV, lerpTime); // lerp to the desiredFOV so that its smooth
        m_currentFOV = Mathf.Clamp(m_currentFOV, 0, maxFOV + 10);
        if(m_virtualCam != null) 
            m_virtualCam.m_Lens.FieldOfView = m_currentFOV; // set the FOV to the currentFOV                                
    }

    /// <summary>
    /// OnRight is for the selection screen to use the joystick or d-pad to go to the next ship in the list
    /// </summary>
    /// <param name="context"></param>
    public void OnRight(InputAction.CallbackContext context)
    {
        if(m_selection != null)
        {
            if(context.performed && !playerReadyInMenu)
                m_selection.OnNext();
        }
    }

    /// <summary>
    /// OnLeft is for the selection screen to use the joystick or d-pad to go to the prev ship in the list
    /// </summary>
    /// <param name="context"></param>
    public void OnLeft(InputAction.CallbackContext context)
    {
        if (m_selection != null)
        {
            if (context.performed && !playerReadyInMenu)
                m_selection.OnPrev();
        }
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (m_selection != null)
        {
            if (context.performed && !playerReadyInMenu)
                m_selection.OnNextMat();
        }
    }

    /// <summary>
    /// Pause is to pause the game when the pause button is pressed
    /// </summary>
    /// <param name="context"></param>
    public void Pause(InputAction.CallbackContext context)
    {
        if(GameManager.gManager != null)
        {
            if (GameManager.gManager.raceStarted)
            {
                GameManager.gManager.StopTime(m_playerNumber - 1);
            }
        }
    }

    public void Back(InputAction.CallbackContext context)
    {
        if (GameManager.gManager != null)
        {
            if(context.performed)
                GameManager.gManager.uiCInput.GetMenuManager().BackOutMenu(m_playerNumber - 1);
        }
    }

    /// <summary>
    /// Brake will send through a value to the shipControlls of how mush the brake is being pressed
    /// </summary>
    /// <param name="context"></param>
    public void Brake(InputAction.CallbackContext context)
    {
        if (m_shipControls != null)
        {
            m_shipControls.SetBrakeMultiplier(context.ReadValue<float>());
        }
    }

    /// <summary>
    /// Accelerate will send through a value to the shipControlls for how mush the accelerate button is being pressed
    /// </summary>
    /// <param name="context"></param>
    public void Accelerate(InputAction.CallbackContext context)
    {
        if(m_shipControls != null)
        {
            m_shipControls.SetSpeedMultiplier(context.ReadValue<float>());
        }
    }

    /// <summary>
    /// Turn will send through a value to the shipControlls for how much and which direction the player wants to turn
    /// </summary>
    /// <param name="context"></param>
    public void Turn(InputAction.CallbackContext context)
    {
        if (m_shipControls != null)
        {
            m_shipControls.SetTurnMultipliers(context.ReadValue<float>() * 0.75f);
        }
    }

    /// <summary>
    /// Strafe will send through a value to the shipControlls for how much and which direction the player wants to strafe
    /// </summary>
    /// <param name="context"></param>
    public void Strafe(InputAction.CallbackContext context)
    {
        if (m_shipControls != null)
        {
            m_shipControls.SetStrafeMultiplier(context.ReadValue<float>() * 0.45f);
        }
    }

    /// <summary>
    /// Boost will activate the boost in shipControlls when the player presses the boost button
    /// </summary>
    /// <param name="context"></param>
    public void Boost(InputAction.CallbackContext context)
    {
        if (m_shipControls != null)
        {
            m_shipControls.WantToBoost();
        }
    }

    /// <summary>
    /// this will change the action map to what ever is sent through
    /// </summary>
    /// <param name="map"> which map to change to </param>
    public void ChangeActionMap(string map)
    {
        player.SwitchCurrentActionMap(map);
    }
}
