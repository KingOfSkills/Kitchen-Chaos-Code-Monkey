using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button singleplayerButton;
    [SerializeField] private Button multplayerButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        Time.timeScale = 1f;
    }
    private void Start()
    {
        singleplayerButton.Select();
    }
    public void PlaySingleplayerGame()
    {
        GameMultiplayer.playMultiplayer = false;
        Loader.Load(Loader.Scene.LobbyScene);
    }
    public void PlayMultiplayerGame()
    {
        GameMultiplayer.playMultiplayer = true;
        Loader.Load(Loader.Scene.LobbyScene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
