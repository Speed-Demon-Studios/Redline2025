using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [Header("Lobby block info")]
    public GameObject lobbyBlockParent;
    public GameObject lobbyInfoBlock;

    [Header("Player block info")]
    public GameObject playerBlockParent;
    public GameObject playerInfoBlock;
    public ShipSelection shipSelection;

    public TMP_Text lobbyNameText;
    public Slider playerNumberSlider;
    public UnityEvent joinEvent;

    private string m_playerName;
    private Lobby m_hostLobby;
    private Lobby m_joinedLobby;
    private float m_heartBeat;
    private float m_updateBeat;

    public void Initialized()
    {
        GameManager.gManager.lbManager = this;
    }
    private void Update()
    {
        if(m_hostLobby != null)
        {
            Timer();
        }
        if (m_joinedLobby != null)
            UpdateTimer();
    }
    async void Timer()
    {
        m_heartBeat -= Time.deltaTime;

        if(m_heartBeat < 0f)
        {
            float max = 20;
            m_heartBeat = max;

            await LobbyService.Instance.SendHeartbeatPingAsync(m_hostLobby.Id);
        }
    }

    async void UpdateTimer()
    {
        m_updateBeat -= Time.deltaTime;
        if (m_joinedLobby != null)
        {
            if (m_updateBeat < 0f)
            {
                float max = 1.1f;
                m_updateBeat = max;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(m_joinedLobby.Id);
                m_joinedLobby = lobby;
            }
            UpdateLobbyStuff(m_joinedLobby);
        }
    }
    public async void Initialize()
    {
        m_playerName = "Player" + Random.Range(0, 100);
        var options = new InitializationOptions();
        options.SetProfile(m_playerName);
        await UnityServices.InitializeAsync(options);

        GameManager.gManager.players[0].GetComponent<PlayerInputScript>().SetSelection(shipSelection);

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log(AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "My Looby";
            if (lobbyNameText = null)
                lobbyName = lobbyNameText.text;

            int maxPlayers = (int)playerNumberSlider.value;
            
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "MapName", new DataObject(DataObject.VisibilityOptions.Public, "Axxon City") }
                },

                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, m_playerName) },
                        {"ShipName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "Splitwing") }
                    }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);

            m_hostLobby = lobby;
            m_joinedLobby = m_hostLobby;

            UpdateLobbyStuff(m_joinedLobby);

            Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
        }
        catch (LobbyServiceException exception)
        {
            Debug.LogError(exception);
        }
    }

    public async void JoinLobby(LobbyInfoBlock blockInfo)
    {
        try
        {
            JoinLobbyByIdOptions joinedLobbyOptions = new JoinLobbyByIdOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, m_playerName) },
                        {"ShipName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "Splitwing") }
                    }
                }
            };

            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();
            Lobby tempJoinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(response.Results[blockInfo.lobbieID].Id, joinedLobbyOptions);

            m_joinedLobby = tempJoinedLobby;

            joinEvent.Invoke();

            UpdateLobbyStuff(m_joinedLobby);

        }
        catch(LobbyServiceException exception)
        {
            Debug.LogError(exception);
        }
    }

    public async void ListLobbies()
    {
        try
        {
            foreach(Transform child in lobbyBlockParent.transform)
            {
                Destroy(child.gameObject);
            }

            QueryResponse query =  await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log(query.Results.Count);
            int lobbyNumber = 0;
            foreach(Lobby lobby in query.Results)
            {
                GameObject tempBlock = Instantiate(lobbyInfoBlock, lobbyBlockParent.transform);
                LobbyInfoBlock infoBlock = tempBlock.GetComponent<LobbyInfoBlock>();

                infoBlock.UpdateLobbyInfo(lobby.Name, "Axxon City", lobby.Players.Count + " / " + lobby.MaxPlayers.ToString(), lobbyNumber);

                lobbyNumber++;
            }
        }
        catch (LobbyServiceException exception)
        {
            Debug.LogError(exception);
        }
    }

    public async void QuickJoin()
    {
        try
        {
            await Lobbies.Instance.QuickJoinLobbyAsync();
            UpdateLobbyStuff(m_joinedLobby);
        }
        catch (LobbyServiceException exception)
        {
            Debug.Log(exception);
        }
    }

    public void UpdateLobbyStuff(Lobby lobby)
    {
        foreach(Transform child in playerBlockParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(Player player in lobby.Players)
        {
            GameObject playerBlock = Instantiate(playerInfoBlock, playerBlockParent.transform);
            PlayerInfoBlock playerInfo = playerBlock.GetComponent<PlayerInfoBlock>();

            playerInfo.UpdatePlayerInfo(player.Data["PlayerName"].Value , player.Data["ShipName"].Value);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(m_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException exception)
        {
            Debug.LogError(exception);
        }
    }

    public async void SwitchShipType(string newShipName)
    {
        try 
        {
            await LobbyService.Instance.UpdatePlayerAsync(m_joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "ShipName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newShipName) }
                }
            });
        }
        catch(LobbyServiceException exception)
        {
            Debug.Log(exception);
        }
    }
}
