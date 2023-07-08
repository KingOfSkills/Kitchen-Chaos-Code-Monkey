using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] LobbyCreateUI lobbyCreateUI;
    [SerializeField] TMP_InputField codeInputField;
    [SerializeField] TMP_InputField playerNameInputField;
    [SerializeField] Transform lobbyContainer;
    [SerializeField] Transform lobbyTemplate;


    private void Start()
    {
        playerNameInputField.text = GameMultiplayer.Instance.GetPlayerName();

        GameLobby.Instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());

        lobbyTemplate.gameObject.SetActive(false);
    }

    private void GameLobby_OnLobbyListChanged(object sender, GameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingeUI>().SetLobby(lobby);
        }
    }

    public void SetPlayerName()
    {
        GameMultiplayer.Instance.SetPlayerName(playerNameInputField.text);
    }

    public void MainMenu()
    {
        GameLobby.Instance.LeaveLobby();
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    public void CreateLobby()
    {
        lobbyCreateUI.Show();
    }

    public void QuickJoin()
    {
        GameLobby.Instance.QuickJoin();
    }

    public void JoinWithCode()
    {
        GameLobby.Instance.JoinLobbyWithCode(codeInputField.text);
    }

    private void OnDestroy()
    {
        GameLobby.Instance.OnLobbyListChanged -= GameLobby_OnLobbyListChanged;
    }
}
