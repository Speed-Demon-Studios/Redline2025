using DifficultyButtonSwitch;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MenuManagement
{
    public class UIControllerInput : MonoBehaviour
    {
        //[SerializeField] private GameObject InputControllerParentPrefab;
        //private PlayerInputManager PIM;

        [SerializeField] private int currentMenuObjectIndex = 0;
        private int m_numberOfPalyers;
        public void SetNumberOfPlayers(int number) { m_numberOfPalyers = number; }
        public int GetNumberOfPlayers() { return m_numberOfPalyers; }

        public Transform selectionMenuGrid;

        public List<RenderTexture> textures = new();

        private MenuManager m_mManager;
        public SetMenu shipSelectionMenu;
        public ShipSelectionSpawnerManager sssManager;
        public ButtonSelectManager bSManager;
        [SerializeField] TransitionManager m_tManager;
        // Testing
        private bool HasInitialized = true;

        public MenuManager GetMenuManager() { return m_mManager; }
        public void SetTimeTrial(bool isTrue) { GameManager.gManager.isTimeTrial = isTrue; }
        public void ChangeGameManagerDifficulty(float change) { GameManager.gManager.ChangeDifficulty(change); }
        public void ChangeGameManagerCatchUpCap(float change) { GameManager.gManager.catchUpTopCap = change; }
        public void ChangeGameManagerCatchUp(float change) { GameManager.gManager.ChangeCatchUp(change); }
        /// <summary>
        /// Adds another selectionMenu in for when a player joins
        /// </summary>
        public void AddToPlayers()
        {
            m_numberOfPalyers += 1;

            GameManager.gManager.AddToNumberOfPlayers();
        }

        public void Inistialize()
        {
            m_mManager = GetComponent<MenuManager>();
            GameManager.gManager.uiCInput = this; // Sets the gameManagers reference to this script
            GameManager.gManager.CurrentScene = "MainMenu";// Sets a string to MainMenu to know when we are in main menu 
            GameManager.gManager.disablePlayerCams = true; // turns a bool on that will disaple all cameras when in the main menu
            GameManager.gManager.resetRacerVariables = true; // turns a bool on that will let the game know the players variables are ready
            GameManager.gManager.mSL.SetPlayerUIInputMM(); // Reseting player 1 main menu button 
        }

        public void GoToRace()
        {
            if (HasInitialized == true)
            {
                foreach (GameObject player in GameManager.gManager.players) // For each player in the player list
                {
                    player.GetComponent<PlayerInputScript>().player.SwitchCurrentActionMap("Player"); // Switch the action map to player
                    RedlineColliderSpawner redline = null; // Makes a new reference to the redline collider spawner script
                    player.GetComponent<PlayerInputScript>().ActivateVirtualCam();
                    foreach (Transform child in player.transform) // for each child object in the player object
                    {
                        if (child.GetComponent<RedlineColliderSpawner>()) // If the child object has a redline collider spawner script
                            redline = child.GetComponent<RedlineColliderSpawner>(); // then assign it to the redline reference
                    }
                    player.GetComponent<ShipsControls>().Initialize(); // Initialize Player ready for race
                    player.GetComponent<ShipsControls>().MaxSpeedCatchupChange(1);
                    foreach (Transform child in player.transform) // do another check on the redline collider spawner reference
                    {
                        FindEveryChild(child, redline);
                    }
                    ActionMappingControl aMC = player.GetComponent<ActionMappingControl>(); // Reseting the first selected buttons 
                    aMC.mES.firstSelectedGameObject = null;
                    aMC.mES.SetSelectedGameObject(null);
                }

                GameManager.gManager.racerObjects = new List<GameObject>(); // Empty the racerObject List
                for (int i = GameManager.gManager.allRacers.Count - 1; i >= 0; i--)
                {
                    GameObject temp = GameManager.gManager.allRacers[i];
               
                    if (temp == null)
                    {   
                        GameManager.gManager.allRacers.Remove(temp);
                    }
                }
                PlayerPrefs.SetInt("SceneID", 2);
                SceneManager.LoadSceneAsync(3); // Load the new race scene
            }
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

        /// <summary>
        /// Quit the game
        /// </summary>
        public void QuitButton()
        {
            Application.Quit();
        }

        /// <summary>
        /// Check if each player is ready
        /// </summary>
        /// <param name="playerNumber"> which player just ready up </param>
        public void ReadyPlayer(int playerNumber)
        {
            // go to that players inputScript and change the bool to say they are ready
            GameManager.gManager.players[playerNumber].GetComponent<PlayerInputScript>().playerReadyInMenu = true; 
            // the number of players ready
            int playersReady = 0; 
            // for each player in the players list
            foreach (GameObject player in GameManager.gManager.players)
            { 
                if (player.GetComponent<PlayerInputScript>().playerReadyInMenu) // if the player is ready
                    playersReady += 1; // add 1 to the number of players ready
            } 
            if (playersReady >= GameManager.gManager.players.Count) // if the number of player ready is equal to the number of players         
            {
                GameManager.gManager.raceAboutToStart = true;
                Invoke(nameof(DelayTransition), 1f);
                Invoke(nameof(DelayGoToRace), 1.5f);// then go to race
            } 
        }     
        void DelayTransition() { m_tManager.Transition("WipeOut"); }
        void DelayGoToRace()
        {
            GoToRace();
        }

        /// <summary>
        /// Check if each player is ready
        /// </summary>
        /// <param name="playerNumber"> which player just ready up </param>
        public void UnReadyPlayer(int playerNumber)
        {
            bool unReadyedThisTurn = false;

            if (GameManager.gManager.players[playerNumber].GetComponent<PlayerInputScript>().playerReadyInMenu == true)
            {
                GameManager.gManager.players[playerNumber].GetComponent<PlayerInputScript>().playerReadyInMenu = false;//the number of players ready
                GameManager.gManager.players[playerNumber].GetComponent<PlayerInputScript>().ReturnShipSelection().UnReady();
                unReadyedThisTurn = true;
            }
            else { unReadyedThisTurn = false; }

            int playersReady = 0;                                                                    
            // for each player in the players list                                                       
            foreach (GameObject player in GameManager.gManager.players)                             
            {                                                                                         
                if (player.GetComponent<PlayerInputScript>().playerReadyInMenu) // if the player is ready
                    playersReady += 1; // add 1 to the number of players ready                                                                                                               
            }                                                                                        
                                                                                                         
            if(playersReady == 0 && !unReadyedThisTurn)                                                  
            {
                GameManager.gManager.uiCInput.bSManager.TransitionToClassSelect(false);
                GameManager.gManager.uAC.PlayUISound(3);
                m_mManager.Back();
            }                                                                                            
            // go to that players inputScript and change the bool to say they are ready                  
                                                                                                         
        }

        /// <summary>
        /// Reseting the action maps and the new first selected buttons
        /// </summary>
        public void CallResetButtons()
        {                                                                                                
            foreach (GameObject player in GameManager.gManager.players) // foreach player in players list                       
            {                                                                                                                
                ActionMappingControl aMC = player.GetComponent<ActionMappingControl>(); // reference to the actionMappingControl
                                    
                aMC.UpdateActionMapForUI(); // Update Action map
            }                                                                                                                
        }

        public void ResetFirstButton(int playerNumber, GameObject button)
        {
            GameManager.gManager.players[playerNumber].GetComponent<ActionMappingControl>().mES.SetSelectedGameObject(button);
            GameManager.gManager.players[playerNumber].GetComponent<ActionMappingControl>().mES.firstSelectedGameObject = button;
        }

        /// <summary>
        /// Sets up the selection screen for each player
        /// </summary>
        public void SetUpSelectionScreen()
        {
            foreach (GameObject playerOBJ in GameManager.gManager.players)
            {
                playerOBJ.GetComponent<PlayerInputScript>().ReturnShipSelection().SetUp();
            }
        }
    }
}
