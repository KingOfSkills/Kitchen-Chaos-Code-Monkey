using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start()
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
        }
        else
        {
            Player.OnAnyPlayerSpawn += Player_OnAnyPlayerSpawn;
        }
    }

    private void Player_OnAnyPlayerSpawn(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChange -= Player_OnSelectedCounterChange;
            Player.LocalInstance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
        }
    }

    private void Player_OnSelectedCounterChange(object sender, Player.OnSelectedCounterChangeEventArgs e)
    {
        if (e.selectedCounter == baseCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    private void Show()
    {
        foreach(GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }
    }
    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Player.OnAnyPlayerSpawn -= Player_OnAnyPlayerSpawn;
        Player.LocalInstance.OnSelectedCounterChange -= Player_OnSelectedCounterChange;
    }
}
