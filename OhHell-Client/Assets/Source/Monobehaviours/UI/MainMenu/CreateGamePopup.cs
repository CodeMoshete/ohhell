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

    private Action<string, string> onCreate;

    private void Start()
    {
        CreateButton.onClick.AddListener(OnCreateButtonClicked);
        CancelButton.onClick.AddListener(OnCancelButtonClicked);
    }

    public void Initialize(Action<string, string> onCreate)
    {
        this.onCreate = onCreate;
    }

    public void ShowPopup()
    {
        gameObject.SetActive(true);
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
