using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    public static GamePauseUI Instance { get; private set; }

    [SerializeField] private Button resumeButton;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameManager.Instance.OnLocalGamePaused += GameManager_OnLocalGamePaused;
        GameManager.Instance.OnLocalGameUnpaused += GameManager_OnLocalGameUnpaused;

        Hide();
    }
    private void GameManager_OnLocalGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }
    private void GameManager_OnLocalGamePaused(object sender, System.EventArgs e)
    {
        Show();
    }
    public void Show()
    {
        gameObject.SetActive(true);
        resumeButton.Select();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    public void ResumeGame()
    {
        GameManager.Instance.TogglePauseGame();
    }
    public void OpenOptions()
    {
        OptionsUI.Instance.Show();
        Hide();
    }
    public void GoToMainMenuScene()
    {
        NetworkManager.Singleton.Shutdown();
        Loader.Load(Loader.Scene.MainMenuScene);
    }
}
