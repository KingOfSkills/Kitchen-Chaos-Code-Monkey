using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestingCharacterSelectUI : MonoBehaviour
{
   
    public void SetPlayerReady()
    {
        CharacterSelectReady.Instance.SetPlayerReady();
    }
}
