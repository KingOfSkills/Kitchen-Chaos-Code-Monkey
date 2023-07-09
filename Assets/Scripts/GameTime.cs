using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class GameTime : MonoBehaviour
{
    [SerializeField] private Slider gameTimeSlider;
    [SerializeField] private TMP_Text gameTimeText;
    [SerializeField] private TMP_InputField gameTimeInputField;
    [SerializeField] private GameObject gameTimeContainer;

    public static int gameTime;

    private void Start()
    {
        gameTime = 90;
        gameTimeSlider.value = gameTime;
        gameTimeText.text = gameTime.ToString() + " seconds";
        gameTimeInputField.text = gameTime.ToString();

        gameTimeContainer.SetActive(NetworkManager.Singleton.IsHost);
    }

    public void SetGameTimeSlider()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            gameTime = (int)gameTimeSlider.value;
            gameTimeText.text = gameTime.ToString() + " seconds";
            gameTimeInputField.text = gameTime.ToString();
        }
    }

    public void SetGameTimeInputField()
    {
        string inputFieldString = gameTimeInputField.text;
        int inputFieldInt = int.Parse(inputFieldString);
        if (inputFieldInt > gameTimeSlider.minValue && inputFieldInt < gameTimeSlider.maxValue)
        {
            gameTime = inputFieldInt;
            gameTimeSlider.value = gameTime;
            gameTimeText.text = gameTime.ToString();
        }
    }
}
