using System;
using UnityEngine;
using UnityEngine.UI;

public class CreateGamePopup : MonoBehaviour
{
    public InputField GameNameField;
    public InputField PlayerNameField;
    public Button CreateButton;
    public Button CancelButton;
    public Button NameKeyboardButton;
    public Button GameNameKeyboardButton;
    public Transform UIContainer;
    public VirtualKeyboard Keyboard;
    public Transform KeyboardPositioner;

    private Action<string, string> onCreate;

    private void Start()
    {
        CreateButton.onClick.AddListener(OnCreateButtonClicked);
        CancelButton.onClick.AddListener(OnCancelButtonClicked);
        NameKeyboardButton.onClick.AddListener(OnShowKeyboardPressedName);
        GameNameKeyboardButton.onClick.AddListener(OnShowKeyboardPressedGame);
    }

    public void Initialize(Action<string, string> onCreate)
    {
        this.onCreate = onCreate;
    }

    public void ShowPopup()
    {
        gameObject.SetActive(true);
    }

    private void OnShowKeyboardPressedName()
    {
        Keyboard.ShowKeyboard(PlayerNameField, OnKeyboardHidden);
        UIContainer.position = KeyboardPositioner.position;
    }

    private void OnShowKeyboardPressedGame()
    {
        Keyboard.ShowKeyboard(GameNameField, OnKeyboardHidden);
        UIContainer.position = KeyboardPositioner.position;
    }

    private void OnKeyboardHidden()
    {
        UIContainer.localPosition = Vector3.zero;
    }

    private void OnCreateButtonClicked()
    {
        if (!string.IsNullOrEmpty(GameNameField.text) && 
            !string.IsNullOrEmpty(PlayerNameField.text))
        {
            onCreate(GameNameField.text, PlayerNameField.text);
        }
    }

    private void OnCancelButtonClicked()
    {
        gameObject.SetActive(false);
    }
}
