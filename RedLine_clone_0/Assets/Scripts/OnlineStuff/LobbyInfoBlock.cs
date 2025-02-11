using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyInfoBlock : MonoBehaviour
{
    public TMP_Text nameOfLobby;
    public TMP_Text mapName;
    public TMP_Text playerNumber;
    public int lobbieID;

    public void UpdateLobbyInfo(string lobbyName, string map, string playerCount, int ID)
    {
        nameOfLobby.text = lobbyName;
        mapName.text = map;
        playerNumber.text = playerCount;
        lobbieID = ID;
    }

    public void JoinLobby()
    {
        GameManager.gManager.lbManager.JoinLobby(this);
    }

    public void TestButton()
    {
        Debug.Log("button Pressed");
    }
}
