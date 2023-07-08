using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lobbyNameText;
    [SerializeField] TextMeshProUGUI lobbyCodeText;

    private void Start()
    {
        Lobby lobby = GameLobby.Instance.GetLobby();

        lobbyNameText.text = "Lobby Name: " + lobby.Name;
        lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }

    public void MainMenu()
    {
        GameLobby.Instance.LeaveLobby();
        NetworkManager.Singleton.Shutdown();
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    public void Ready()
    {
        CharacterSelectReady.Instance.SetPlayerReady();
    }
}
