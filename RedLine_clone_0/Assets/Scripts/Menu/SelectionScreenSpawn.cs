using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionScreenSpawn : MonoBehaviour
{
    private int m_playerNumber;

    public void SpawnShipSelection(GameObject prefabToSpawn, int number)
    {
        m_playerNumber = number;// set player number

        GameObject selectionMenu = Instantiate(prefabToSpawn, GameManager.gManager.uiCInput.selectionMenuGrid);// Instantiate the selectionMenu

        selectionMenu.GetComponent<ShipSelection>().SetShipSelectionNumbers(m_playerNumber);

        GameManager.gManager.uiCInput.shipSelectionMenu.menuStartButtons.Add(selectionMenu.GetComponent<ShipSelection>().readyButton); // Adds the button to the GameManager list

        GameObject player = GameManager.gManager.players[m_playerNumber];
        player.GetComponent<PlayerInputScript>().SetSelection(selectionMenu.GetComponent<ShipSelection>());

        selectionMenu.GetComponent<ShipSelection>().SetShip(player);
    }
}
