using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicText;

    [SerializeField] private Button moveUpButton;
    [SerializeField] private TextMeshProUGUI moveUpButtonText;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private TextMeshProUGUI moveRightButtonText;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private TextMeshProUGUI moveDownButtonText;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private TextMeshProUGUI moveLeftButtonText;
    [SerializeField] private Button interactButton;
    [SerializeField] private TextMeshProUGUI interactButtonText;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private TextMeshProUGUI interactAlternateButtonText;
    [SerializeField] private Button pauseKeybindButton;
    [SerializeField] private TextMeshProUGUI pauseKeybindButtonText;
    [SerializeField] private Transform pressToRebindKey;

    [SerializeField] private Button gamepadInteractButton;
    [SerializeField] private TextMeshProUGUI gamepadInteractButtonText;
    [SerializeField] private Button gamepadInteractAlternateButton;
    [SerializeField] private TextMeshProUGUI gamepadInteractAlternateButtonText;
    [SerializeField] private Button gamepadPauseButton;
    [SerializeField] private TextMeshProUGUI gamepadPauseButtonText;

    private void Awake()
    {
        Instance = this;
        // Keyboard
        moveUpButton.onClick.AddListener(() =>
       {
           RebindingBinding(GameInput.Binding.MoveUp);
       });
        moveDownButton.onClick.AddListener(() =>
       {
           RebindingBinding(GameInput.Binding.MoveDown);
       });
        moveLeftButton.onClick.AddListener(() =>
       {
           RebindingBinding(GameInput.Binding.MoveLeft);
       });
        moveRightButton.onClick.AddListener(() =>
       {
           RebindingBinding(GameInput.Binding.MoveRight);
       });
        interactButton.onClick.AddListener(() =>
       {
           RebindingBinding(GameInput.Binding.Interact);
       });
        interactAlternateButton.onClick.AddListener(() =>
       {
           RebindingBinding(GameInput.Binding.InteractAlternate);
       });
        pauseKeybindButton.onClick.AddListener(() =>
       {
           RebindingBinding(GameInput.Binding.Pause);
       });
        // Gamepad
        gamepadInteractButton.onClick.AddListener(() =>
       {
           RebindingBinding(GameInput.Binding.GamepadInteract);
       });
        gamepadInteractAlternateButton.onClick.AddListener(() =>
       {
           RebindingBinding(GameInput.Binding.GamepadInteractAlternate);
       });
        gamepadPauseButton.onClick.AddListener(() =>
       {
           RebindingBinding(GameInput.Binding.GamepadPause);
       });
    }
    private void Start()
    {
        GameManager.Instance.OnLocalGameUnpaused += GameManager_OnGameUnpaused;

        UpdateVisual();

        Hide();
    }

    private void GameManager_OnGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    public void ChangeSoundEffectVolume()
    {
        SoundManager.Instance.ChangeVolume();
        UpdateVisual();
    }
    public void ChangeMusicVolume()
    {
        MusicManager.Instance.ChangeVolume();
        UpdateVisual();
    }
    private void UpdateVisual()
    {
        soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);
        // Keyboard
        moveUpButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        moveDownButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        moveLeftButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        moveRightButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
        interactButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAlternateButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        pauseKeybindButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        // Gamepad
        gamepadInteractButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadInteract);
        gamepadInteractAlternateButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadInteractAlternate);
        gamepadPauseButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamepadPause);
    }
    public void BackToPauseUI()
    {
        GamePauseUI.Instance.Show();
        Hide();
    }
    public void Show()
    {
        gameObject.SetActive(true);
        soundEffectsButton.Select();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void ShowPressToRebindKey()
    {
        pressToRebindKey.gameObject.SetActive(true);
    }private void HidePressToRebindKey()
    {
        pressToRebindKey.gameObject.SetActive(false);
    }
    private void RebindingBinding(GameInput.Binding binding)
    {
        ShowPressToRebindKey();
        GameInput.Instance.RebindBinding(binding, () =>
        {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }
}
