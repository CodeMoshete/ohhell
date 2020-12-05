using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKeyboard : MonoBehaviour
{
    private readonly Color SELECT_COLOR = new Color(0.545f, 0.984f, 1f, 1f);

    public Button CloseButton;
    public Image ShiftButton;
    private List<KeyboardKey> Keys;
    private bool isShiftToggled;
    private bool firstKeyTyped;
    private InputField targetField;
    private Action onKeyboardHidden;

    void Start()
    {
        CloseButton.onClick.AddListener(HideKeyboard);
        Keys = new List<KeyboardKey>(GetComponentsInChildren<KeyboardKey>());
        for (int i = 0, count = Keys.Count; i < count; ++i)
        {
            Keys[i].Initialize(OnKeyTyped);
        }
    }

    private void OnKeyTyped(string keyVal)
    {
        if (targetField != null)
        {
            switch (keyVal)
            {
                case "Shift":
                    if (!firstKeyTyped)
                    {
                        isShiftToggled = !isShiftToggled;
                        ShiftButton.color = isShiftToggled ? SELECT_COLOR : Color.white;
                    }
                    break;
                case "Space":
                    targetField.text += " ";
                    break;
                case "Backspace":
                    if (targetField.text.Length > 0)
                    {
                        targetField.text = targetField.text.Remove(targetField.text.Length - 1);
                    }
                    break;
                default:
                    if (isShiftToggled)
                    {
                        targetField.text += keyVal.ToUpper();
                    }
                    else
                    {
                        targetField.text += keyVal.ToLower();
                    }
                    break;
            }
        }

        if (firstKeyTyped)
        {
            firstKeyTyped = false;

            if (keyVal != "Shift")
            {
                ShiftButton.color = Color.white;
                isShiftToggled = false;
            }
        }
    }

    public void ShowKeyboard(InputField targetField, Action onKeyboardHidden)
    {
        firstKeyTyped = true;
        isShiftToggled = true;
        ShiftButton.color = SELECT_COLOR;
        this.targetField = targetField;
        this.onKeyboardHidden = onKeyboardHidden;
        gameObject.SetActive(true);
    }

    private void HideKeyboard()
    {
        gameObject.SetActive(false);
        if (onKeyboardHidden != null)
        {
            onKeyboardHidden();
        }
    }
}
