using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyInfoBlock : MonoBehaviour
{
    public TMP_Text nameOfLobby;
    public TMP_Text mapName;
    public TMP_Text playerNumber;
    public TMP_Text ping;

    public void UpdateLobbyInfo(string lobbyName, string map, string playerCount, string lobbyPing)
    {
        nameOfLobby.text = lobbyName;
        mapName.text = map;
        playerNumber.text = playerCount;
        ping.text = lobbyPing;
    }

    public void TestButton()
    {
        Debug.Log("button Pressed");
    }
}
