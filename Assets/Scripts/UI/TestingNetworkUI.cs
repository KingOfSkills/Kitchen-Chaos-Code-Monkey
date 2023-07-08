using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestingNetworkUI : MonoBehaviour
{

    public void StartHost()
    {
        GameMultiplayer.Instance.StartHost();
        Hide();
    }
    public void StartClient()
    {
        GameMultiplayer.Instance.StartClient();
        Hide();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
