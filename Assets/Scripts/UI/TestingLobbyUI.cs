using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingLobbyUI : MonoBehaviour
{

    public void CreateGame()
    {
        GameMultiplayer.Instance.StartHost();

        Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
    }
    public void JoinGame()
    {
        GameMultiplayer.Instance.StartClient();
    }
}
