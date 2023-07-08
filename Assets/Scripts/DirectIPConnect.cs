using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DirectIPConnect : MonoBehaviour
{
    private const string DEFAULT_IP_ADDRESS = "127.0.0.1";
    private const string DEFAULT_PORT_NUMBER = "7777";

    [SerializeField] UnityTransport unityTransport;
    [SerializeField] TMP_InputField ipAddressInputField;
    [SerializeField] TMP_InputField portInputField;

    private void Start()
    {
        ipAddressInputField.text = unityTransport.ConnectionData.Address.ToString();
        portInputField.text = unityTransport.ConnectionData.Port.ToString();
    }

    public void CreateLobby()
    {
        unityTransport.ConnectionData.Address = DEFAULT_IP_ADDRESS;

        GameMultiplayer.Instance.StartHost();

        Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
    }

    public void JoinLobby()
    {
        GameMultiplayer.Instance.StartClient();
    }
}
