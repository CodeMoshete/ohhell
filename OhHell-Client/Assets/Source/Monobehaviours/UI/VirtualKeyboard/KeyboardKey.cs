using System;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardKey : MonoBehaviour
{
    public string KeyValue;
    public Action<string> onKeyPressed;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonPressed);
    }

    public void Initialize(Action<string> onKeyPressed)
    {
        this.onKeyPressed = onKeyPressed;
    }

    private void OnButtonPressed()
    {
        if (onKeyPressed != null)
        {
            onKeyPressed(KeyValue);
        }
    }
}
