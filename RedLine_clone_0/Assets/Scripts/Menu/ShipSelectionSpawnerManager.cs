using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MenuManagement;

public class ShipSelectionSpawnerManager : MonoBehaviour
{
    public GameObject onePrefab;
    public GameObject twoPrefab;
    public GameObject threeAndFourPrefab;

    private void SpawnSelectionScreens(int number, GameObject prefab)
    {
        GameManager.gManager.players[number].GetComponent<SelectionScreenSpawn>().SpawnShipSelection(prefab, number);
    }

    public void ReOrderShipSelection()
    {
        GameManager.gManager.uiCInput.shipSelectionMenu.menuStartButtons.Clear();   
        int index = 0;
        foreach(GameObject playerOBJ in GameManager.gManager.players)
        {
            ShipSelection tempSelection = playerOBJ.GetComponent<PlayerInputScript>().GetShipSelection();

            switch (GameManager.gManager.players.Count)
            {
                case 1:
                    SpawnSelectionScreens(index, onePrefab);
                    break;
                case 2:
                    SpawnSelectionScreens(index, twoPrefab);
                    break;
                case 3:
                    SpawnSelectionScreens(index, threeAndFourPrefab);
                    break;
                case 4:
                    SpawnSelectionScreens(index, threeAndFourPrefab);
                    break;
            }

            if (tempSelection != null)
            {
                playerOBJ.GetComponent<PlayerInputScript>().GetShipSelection().SetPlayerNum(tempSelection.PlayerNuumber);
                playerOBJ.GetComponent<PlayerInputScript>().GetShipSelection().SetShipIndex(tempSelection.ShipIndex);
                playerOBJ.GetComponent<PlayerInputScript>().GetShipSelection().SetMaterialIndex(tempSelection.MaterialIndex);

                if (playerOBJ.GetComponent<PlayerInputScript>().playerReadyInMenu)
                {
                    playerOBJ.GetComponent<PlayerInputScript>().GetShipSelection().sInfo.readyAnimator.SetTrigger(playerOBJ.GetComponent<PlayerInputScript>().GetShipSelection().sInfo.readyTriggerString);
                }

                Destroy(tempSelection.gameObject);
            }

            playerOBJ.GetComponent<PlayerInputScript>().GetShipSelection().SetUp();

            index++;
        }
    }
}
