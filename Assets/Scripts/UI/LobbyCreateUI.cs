using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;

    private void Start()
    {
        Hide();
    }

    public void CreatePublic()
    {
        GameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
    }

    public void CreatePrivate()
    {
        GameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
