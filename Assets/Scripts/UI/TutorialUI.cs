using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    // Keyboard
    [SerializeField] private TextMeshProUGUI keyMoveUpText;
    [SerializeField] private TextMeshProUGUI keyMoveLeftText;
    [SerializeField] private TextMeshProUGUI keyMoveDonwText;
    [SerializeField] private TextMeshProUGUI keyMoveRightText;
    [SerializeField] private TextMeshProUGUI keyInteractText;
    [SerializeField] private TextMeshProUGUI keyInteractAlternateText;
    [SerializeField] private TextMeshProUGUI keyPauseText;
    // Gamepad
    [SerializeField] private TextMeshProUGUI keyGamepadInteractText;
    [SerializeField] private TextMeshProUGUI keyGamepadInteractAlternateText;
    [SerializeField] private TextMeshProUGUI keyGamepadPauseText;

    private void Start()
    {
        GameInput.Instance.OnRebind += GameInput_OnRebind;
        //GameManager.Instance.OnStateChange += GameManager_OnStateChange;
        GameManager.Instance.OnLocalPlayerReady += GameManager_OnLocalPlayerReady;

        UpdateVisual();

        Show();
    }

    private void GameManager_OnLocalPlayerReady(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady())
        {
            Hide();
        }
    }

    //private void GameManager_OnStateChange(object sender, System.EventArgs e)
    //{
    //    if (GameManager.Instance.IsCountingdownToStart())
    //    {
    //        Hide();
    //    }
    //}

    private void GameInput_OnRebind(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // Keyboard
        keyMoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        keyMoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        keyMoveDonwText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        keyMoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
        keyInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        keyInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        keyPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        // Gamepad
        keyGamepadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadInteract);
        keyGamepadInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadInteractAlternate);
        keyGamepadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadPause);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
