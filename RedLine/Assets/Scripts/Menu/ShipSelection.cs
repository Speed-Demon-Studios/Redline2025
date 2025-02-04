using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MenuManagement;

public class ShipSelection : MonoBehaviour
{
    public List<GameObject> ships = new();
    public List<ShipVariant> variants = new();

    private GameObject m_currentShips;
    public ShipSelectionInfo sInfo;

    public GameObject readyButton;

    private GameObject m_ship;

    public RawImage image;
    public RenderTexture texture;
    public Camera cam;
    private List<List<float>> allLists = new();

    private int m_playerNum;
    private int m_shipIndex;
    private int m_materialIndex;

    public int PlayerNuumber => m_playerNum;
    public int ShipIndex => m_shipIndex;
    public int MaterialIndex => m_materialIndex;

    public void SetPlayerNum(int number) { m_playerNum = number; }
    public void SetShipIndex(int number) { m_shipIndex = number; }
    public void SetMaterialIndex(int number) { m_materialIndex = number; }


    /////////////////////////////////////////////////////////////////
    ///                                                          ///
    ///      All of the getters and setters in this script       ///
    ///                                                          ///
    /////////////////////////////////////////////////////////////////
    public void SetShipSelectionNumbers(int number) { m_playerNum = number; }
    public void SetShip(GameObject ship) { m_ship = ship; }

    private void OnEnable()
    {
        //SetUp();
    }

    private void Update()
    {
        //m_y += 5f * Time.deltaTime;
        //cam.transform.rotation = Quaternion.Euler(0, m_y, 0);
    }

    /// <summary>
    /// Set up makes sure that all the text animation and the stats show up when the screen loads up
    /// </summary>
    public void SetUp()
    {
        texture = GameManager.gManager.uiCInput.textures[m_playerNum];
        cam.GetComponentInChildren<Camera>().targetTexture = texture;
        image.texture = texture;

        GameManager.gManager.uiCInput.bSManager.TransitionToShipSelect(sInfo.shipDisplayAnim);

        List<Animator> tempList = new();
        int index = 0;
        foreach (Animator anim in sInfo.shipAnimators)
        {
            if (index != m_shipIndex)
            {
                tempList.Add(anim);
            }
            index++;
        }

        if (GameManager.gManager.uiCInput.GetMenuManager().GetCurrentType() == MenuType.ShipSelectionReady)
        {
            GameManager.gManager.uiCInput.GetMenuManager().SetButtons(GameManager.gManager.uiCInput.GetMenuManager().GetCurrentMenu());
            GameManager.gManager.uiCInput.GetMenuManager().BackGroundPanelForSelection();
        }

        GameManager.gManager.uiCInput.bSManager.VehicleInfoChange(0, sInfo.shipAnimators[m_shipIndex], tempList);

        allLists.Add(sInfo.m_splitwingStats);
        allLists.Add(sInfo.m_fulcrumStats);
        allLists.Add(sInfo.m_cutlassStats);

        List<float> tempListOfFloats = allLists[m_shipIndex];

        sInfo.TopSpeedBarFill(tempListOfFloats[0]);
        sInfo.AccelerationBarFill(tempListOfFloats[1]);
        sInfo.HandlingBarFill(tempListOfFloats[2]);

        foreach (GameObject ship in ships)
            ship.SetActive(false);

        m_currentShips = ships[m_shipIndex];
        m_currentShips.SetActive(true);

        m_currentShips.GetComponent<ShipTypeInfo>().SwitchMaterials(m_materialIndex);
    }

    public void OnNextMat()
    {
        m_materialIndex++;
        if (m_materialIndex >= 3)
            m_materialIndex = 0;
        ships[m_shipIndex].GetComponent<ShipTypeInfo>().SwitchMaterials(m_materialIndex);
        GameManager.gManager.uiCInput.bSManager.ManufacturerChange(sInfo.manufacturerSprites[m_materialIndex], sInfo.manufacturerDisplayAnim,
            sInfo.manufacturerImage, sInfo.manufacturerImageRed);
        GameManager.gManager.uAC.PlayUISound(1);
    }

    /// <summary>
    /// OnNext will go to the next ship in the list when the input is pressed
    /// </summary>
    public void OnNext()
    {
        m_currentShips.SetActive(false); // sets the ship to false that was currently selected
        m_shipIndex += 1; // adds one to the ship index so that it goes to the next ship
        if (m_shipIndex > ships.Count - 1) // if the index goes over the count then go back to 0
        {
            m_shipIndex = 0;
        }
        m_currentShips = ships[m_shipIndex]; // set the current ship to the index
        m_currentShips.SetActive(true); // then set that current ship to true so it shows up

        List<float> tempListOfFloats = allLists[m_shipIndex];

        sInfo.TopSpeedBarFill(tempListOfFloats[0]);
        sInfo.AccelerationBarFill(tempListOfFloats[1]);
        sInfo.HandlingBarFill(tempListOfFloats[2]);

        List<Animator> tempList = new();
        int index = 0;
        foreach(Animator anim in sInfo.shipAnimators)
        {
            if(index != m_shipIndex)
            {
                tempList.Add(anim);
            }
            index++;
        }

        GameManager.gManager.uiCInput.bSManager.VehicleInfoChange(0, sInfo.shipAnimators[m_shipIndex], tempList);

        GameManager.gManager.uAC.PlayUISound(1);

        ships[m_shipIndex].GetComponent<ShipTypeInfo>().SwitchMaterials(m_materialIndex);
    }

    /// <summary>
    /// OnPrev will go to the prev ship in the list when the input is pressed
    /// </summary>
    public void OnPrev()
    {
        m_currentShips.SetActive(false); // sets the ship to false that was currently selected    
        m_shipIndex -= 1; // adds one to the ship index so that it goes to the next ship          
        if (m_shipIndex < 0) // if the index goes over the count then go back to 0                
        {                                                                                         
            m_shipIndex = ships.Count - 1;                                                        
        }                                                                                         
        m_currentShips = ships[m_shipIndex]; // set the current ship to the index                 
        m_currentShips.SetActive(true); // then set that current ship to true so it shows up
                                        // 
        List<float> tempListOfFloats = allLists[m_shipIndex];

        sInfo.TopSpeedBarFill(tempListOfFloats[0]);
        sInfo.AccelerationBarFill(tempListOfFloats[1]);
        sInfo.HandlingBarFill(tempListOfFloats[2]);

        List<Animator> tempList = new();
        int index = 0;
        foreach (Animator anim in sInfo.shipAnimators)
        {
            if (index != m_shipIndex)
            {
                tempList.Add(anim);
            }
            index++;
        }
        GameManager.gManager.uiCInput.bSManager.VehicleInfoChange(0, sInfo.shipAnimators[m_shipIndex], tempList);

        GameManager.gManager.uAC.PlayUISound(1);

        ships[m_shipIndex].GetComponent<ShipTypeInfo>().SwitchMaterials(m_materialIndex);
    }

    /// <summary>
    /// Once the player presses ready it sets all the variants to the shipControls and gets ready to spawn the models
    /// </summary>
    public void Ready()
    {
        if (!m_ship.GetComponent<PlayerInputScript>().playerReadyInMenu)
        {
            // Sets ship variants
            m_ship.GetComponent<ShipsControls>().VariantObject = variants[m_shipIndex];
            m_ship.GetComponent<ShipsControls>().enabled = true; // Enables shipControls for movement
            m_ship.GetComponent<ShipsControls>().shipSelected = m_shipIndex;
            m_ship.GetComponent<ShipsControls>().SetMaterialIndex(m_materialIndex);

            if (m_ship.GetComponent<VariantAudioContainer>() != null)
            {
                m_ship.GetComponent<VariantAudioContainer>().CheckVariant(m_shipIndex);
                m_ship.GetComponent<ShipsControls>().shipSelected = m_shipIndex;
            }

            if (m_ship.GetComponent<ShipBlendAnimations>()) // if the ship selected has animations
                m_ship.GetComponent<ShipBlendAnimations>().enabled = true; // set the refrenece for animations

            GameManager.gManager.uiCInput.ReadyPlayer(m_playerNum); // Readys this player

            GameManager.gManager.uAC.PlayUISound(2);

            sInfo.readyAnimator.SetTrigger(sInfo.readyTriggerString);
        }
    }
    public void UnReady()
    {
        if (!GameManager.gManager.raceAboutToStart)
        {
            // Sets ship variants
            m_ship.GetComponent<ShipsControls>().VariantObject = null;
            m_ship.GetComponent<ShipsControls>().enabled = false; // Enables shipControls for movement 
            m_ship.GetComponent<ShipBlendAnimations>().enabled = false; // set the refrenece for animations

            if (m_ship.GetComponent<ShipBlendAnimations>()) // if the ship selected has animations
                m_ship.GetComponent<ShipBlendAnimations>().enabled = false; // set the refrenece for animations

            GameManager.gManager.uAC.PlayUISound(3);

            sInfo.readyAnimator.SetTrigger(sInfo.unReadyTriggerString);
        }
    }

    /// <summary>
    /// This is a effect for the text every time the ship is switch in the selection screen
    /// </summary>
    /// <param name="shipName"> What is the ships name you are trying to print out </param>
    /// <returns></returns>
    IEnumerator NameChange(string shipName)
    {
        string aToZ = "abcdefghijklmnopqrstuvwxyz"; // a string with every letter in the alphabet       
                                                                                                        
        int stringLength = shipName.Length; // an int for the ship string length                        
                                                                                                        
        string tempName = shipName; // temp reference to the orginal name of the ship                   
        for (int j = 0; j < stringLength; j++) // for each letter in the ship name                      
        {                                                                                               
            yield return new WaitForSeconds(0.005f); // wait                                            
                                                                                                        
            char randomLetter = aToZ[Random.Range(0, 24)]; // choose a random letter from aToZ          
                                                                                                        
            tempName = tempName.Remove(j, 1); // j being the index remove the letter at point j         
            tempName = tempName.Insert(j, randomLetter.ToString()); // replace it with the random letter
                                                                                                        
            //this.shipName.text = tempName; // set the text to the new text                              
        }                                                                                               
        // this is doing the same as before but now it will slow choose the correct letter                            
        for (int i = 0; i < stringLength; i++)                                                                        
        {                                                                                                             
            tempName = tempName.Remove(i, 1); // remove the letter at index i                                         
            tempName = tempName.Insert(i, shipName.ToCharArray()[i].ToString()); // replace it with the correct letter
                                                                                                                      
            for (int j = i + 1; j < stringLength; j++) // for the remaining letters                              
            {                                                                                                    
                yield return new WaitForSeconds(0.001f); // wait                                                 
                                                                                                                 
                char randomLetter = aToZ[Random.Range(0, 24)]; // choose random letter                           
                                                                                                                 
                tempName = tempName.Remove(j, 1); // remove at index j                                           
                tempName = tempName.Insert(j, randomLetter.ToString()); // replace with random letter              
            }                                                                                                    
            //this.shipName.text = tempName; // set text to new word                                                    
            yield return new WaitForSeconds(0.008f); // wait before doing it again                                    
                                                                                                                      
        }                                                                                                             
    }
}
