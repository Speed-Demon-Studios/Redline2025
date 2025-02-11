using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoBlock : MonoBehaviour
{
    public TMP_Text playerName;
    public TMP_Text playerShip;

    public void UpdatePlayerInfo(string name, string shipName)
    {
        playerName.text = name;
        playerShip.text = shipName;
    }
}
