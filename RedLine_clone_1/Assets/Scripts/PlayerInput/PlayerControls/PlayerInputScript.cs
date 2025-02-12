using MenuManagement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class PlayerInputScript : MonoBehaviour
{
    [Header("References")]
    public PlayerUiControl uiController;

    public PlayerInput player;
    public MultiplayerEventSystem eventSystem;
    private ShipsControls m_shipControls;
    private Gamepad m_playerGamepad;
    private GameManager gMan;
    private ShipSelection m_selection;

    private bool m_alreadyLoadedIn = false;
    private int m_playerNumber;
    public bool playerReadyInMenu;

    static private ShipVariant m_ship;
    static private int m_shipMaterialNumber;
    static private int m_shipIndex;
    public GameObject shipPrefab;

    public void SetShipInfo(ShipVariant ship, int shipMaterialNumber, int shipIndex)
    {
        m_ship = ship;
        m_shipMaterialNumber = shipMaterialNumber;
        m_shipIndex = shipIndex;
    }

    public void InstantiateShip()
    {
        GameObject ship = Instantiate(shipPrefab);

        m_shipControls = ship.GetComponent<ShipsControls>();
        uiController = ship.GetComponentInChildren<PlayerUiControl>();

        ship.GetComponent<ShipsControls>().VariantObject = m_ship;
        ship.GetComponent<ShipsControls>().enabled = true; // Enables shipControls for movement
        ship.GetComponent<ShipsControls>().shipSelected = m_shipIndex;
        ship.GetComponent<ShipsControls>().SetMaterialIndex(m_shipMaterialNumber);

        if (ship.GetComponent<VariantAudioContainer>() != null)
        {
            ship.GetComponent<VariantAudioContainer>().CheckVariant(m_shipIndex);
            ship.GetComponent<ShipsControls>().shipSelected = m_shipIndex;
        }

        if (ship.GetComponent<ShipBlendAnimations>()) // if the ship selected has animations
            ship.GetComponent<ShipBlendAnimations>().enabled = true; // set the refrenece for animations

        RedlineColliderSpawner redline = null;

        foreach (Transform child in ship.transform) // for each child object in the player object
        {
            if (child.GetComponent<RedlineColliderSpawner>()) // If the child object has a redline collider spawner script
                redline = child.GetComponent<RedlineColliderSpawner>(); // then assign it to the redline reference
        }
        ship.GetComponent<ShipsControls>().Initialize(); // Initialize Player ready for race
        ship.GetComponent<ShipsControls>().MaxSpeedCatchupChange(1);
        foreach (Transform child in ship.transform) // do another check on the redline collider spawner reference
        {
            FindEveryChild(child, redline);
        }

        GameManager.gManager.playerShips.Add(ship);
        GameManager.gManager.allRacers.Add(ship);
    }

    /// <summary>
    /// go through every child object in each object untill you find the trailSpawner
    /// </summary>
    /// <param name="parent"> the parent </param>
    /// <param name="redline"> reference to the redline collider spawner script </param>
    public void FindEveryChild(Transform parent, RedlineColliderSpawner redline)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag("TrailSpawn"))
                redline.spawnPoint = child;

            if (child.transform.childCount > 0)
                FindEveryChild(child, redline);
        }
    }

    /////////////////////////////////////////////////////////////////
    ///                                                          ///
    ///      All of the getters and setters in this script       ///
    ///                                                          ///
   /////////////////////////////////////////////////////////////////
    public void SetPlayerNumber(int number) { m_playerNumber = number; }
    public int GetPlayerNumber() { return m_playerNumber; }

    public void SetSelection(ShipSelection selection) { m_selection = selection; }
    public ShipSelection GetShipSelection() { return m_selection; }
    public ShipsControls GetShipControls() { return m_shipControls; }
    public Gamepad GetPlayerGamepad() { return m_playerGamepad; }
    public PlayerInput GetPlayerInput() { return player; }
    public void SetPlayerInput(PlayerInput playerInput) { player = playerInput; }
    public void SetShipControls(ShipsControls controls) { m_shipControls = controls; }
    public void SetEventSystem(MultiplayerEventSystem eventSystem) { this.eventSystem = eventSystem; }

    // Start is called before the first frame update
    private void Awake()
    {
        if(!m_alreadyLoadedIn)
            Inistialize();
    }

    public void Inistialize()
    {
        ActionMappingControl amc = this.GetComponent<ActionMappingControl>();

        amc.SetPlayerInput(player);
        amc.SetmES(eventSystem);

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

        m_alreadyLoadedIn = true;
    }

    /// <summary>
    /// Gets called when a controller looses connection
    /// </summary>
    public void PlayerDisconnect()
    {
        if (GameManager.gManager.CurrentScene != "Race")
        {
            foreach (InputDevice device in InputUser.GetUnpairedInputDevices())
            {
                InputUser.GetUnpairedInputDevices().Remove(device);
            }

            Destroy(m_selection.gameObject);

            GameManager.gManager.numberOfPlayers -= 1;

            // -1 from number of player in the uicontroller script
            GameManager.gManager.uiCInput.SetNumberOfPlayers(GameManager.gManager.uiCInput.GetNumberOfPlayers() - 1);

            gMan.uiCInput.sssManager.ReOrderShipSelection();
            gMan.uiCInput.GetMenuManager().SetButtons(gMan.uiCInput.GetMenuManager().GetCurrentMenu());
            GameManager.gManager.uiCInput.GetMenuManager().BackGroundPanelForSelection();

            GameObject inputSystemSetToDestroy = GameManager.gManager.players[GameManager.gManager.players.Count];

            for(int i = 0; i < GameManager.gManager.players.Count; i++)
            {
                if (GameManager.gManager.players[i].GetComponent<PlayerInputScript>().GetPlayerNumber() < m_playerNumber)
                    return;

                if (i + 1 > GameManager.gManager.players.Count)
                    return;

                DestroyImmediate(GameManager.gManager.players[i].GetComponent<PlayerInputScript>().GetShipControls());

                GameManager.gManager.players[i].GetComponent<PlayerInputScript>().SetShipControls(null);
                GameManager.gManager.players[i].GetComponent<PlayerInputScript>().uiController = null;

                GameManager.gManager.players[i].GetComponent<PlayerInputScript>().SetShipControls(GameManager.gManager.players[i + 1].GetComponent<PlayerInputScript>().GetShipControls());
                GameManager.gManager.players[i].GetComponent<PlayerInputScript>().uiController = GameManager.gManager.players[i + 1].GetComponent<PlayerInputScript>().uiController;
            }

            Destroy(inputSystemSetToDestroy);
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

    }

    /// <summary>
    /// OnRight is for the selection screen to use the joystick or d-pad to go to the next ship in the list
    /// </summary>
    /// <param name="context"></param>
    public void OnRight(InputAction.CallbackContext context)
    {
        if(m_selection != null && GameManager.gManager.uiCInput.GetMenuManager().GetCurrentType() == MenuType.ShipSelectionOnline || GameManager.gManager.uiCInput.GetMenuManager().GetCurrentType() == MenuType.ShipSelectionSoloNSplitScreen)
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
        if (m_selection != null && GameManager.gManager.uiCInput.GetMenuManager().GetCurrentType() == MenuType.ShipSelectionOnline || GameManager.gManager.uiCInput.GetMenuManager().GetCurrentType() == MenuType.ShipSelectionSoloNSplitScreen)
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
