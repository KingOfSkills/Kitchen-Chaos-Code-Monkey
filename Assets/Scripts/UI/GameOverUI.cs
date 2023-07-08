using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class GameOverUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI totalRecipesDeliveredText;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Loader.Scene nextLevelScene;

    private void Start()
    {
        GameManager.Instance.OnStateChange += GameManager_OnStateChange;
        Hide();
    }

    private void GameManager_OnStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();
            totalRecipesDeliveredText.text = DeliveryCounter.GetSuccessfulRecipeAmount().ToString();//DeliveryCounter.GetSuccessfulRecipeAmount().ToString();
        }
        else
        {
            Hide();
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void NextLevel()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            GameManager.Instance.DestroySelf();
            Loader.LoadNetwork(nextLevelScene);
        }
    }

    public void MainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        Loader.Load(Loader.Scene.MainMenuScene);
    }
}
