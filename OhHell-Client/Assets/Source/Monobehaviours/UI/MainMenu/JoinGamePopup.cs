﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class JoinGamePopup : MonoBehaviour
{
    public InputField PlayerNameField;
    public Button JoinButton;
    public Button CancelButton;
    public Button NameKeyboardButton;
    public Transform KeyboardPositioner;
    public Transform UIContainer;
    public VirtualKeyboard Keyboard;

    private Action<string, string> onJoin;
    private string gameName;

    private void Start()
    {
        JoinButton.onClick.AddListener(OnJoinButtonClicked);
        CancelButton.onClick.AddListener(OnCancelButtonClicked);
        NameKeyboardButton.onClick.AddListener(OnShowKeyboardPressed);
    }

    public void Initialize(Action<string, string> onJoin)
    {
        this.onJoin = onJoin;
    }

    public void ShowPopup(string gameName)
    {
        this.gameName = gameName;
        gameObject.SetActive(true);
    }

    private void OnShowKeyboardPressed()
    {
        Keyboard.ShowKeyboard(PlayerNameField, OnKeyboardHidden);
        UIContainer.position = KeyboardPositioner.position;
    }

    private void OnKeyboardHidden()
    {
        UIContainer.localPosition = Vector3.zero;
    }

    private void OnJoinButtonClicked()
    {
        if (!string.IsNullOrEmpty(PlayerNameField.text))
        {
            onJoin(gameName, PlayerNameField.text);
        }
    }

    private void OnCancelButtonClicked()
    {
        gameObject.SetActive(false);
    }
}
