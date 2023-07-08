using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListSingeUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI lobbyNameText;

    private Lobby lobby;

    public void JoinLobby()
    {
        GameLobby.Instance.JoinLobbyWithId(lobby.Id);
    }

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
    }
}
