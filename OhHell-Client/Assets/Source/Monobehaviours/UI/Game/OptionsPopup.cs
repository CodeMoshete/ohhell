using UnityEngine;
using UnityEngine.UI;

public class OptionsPopup : MonoBehaviour
{
    public Toggle AdvancedCardControlsToggle;
    public Button CloseButton;

    private void Start()
    {
        AdvancedCardControlsToggle.onValueChanged.AddListener(ToggleAdvancedCardControls);
        CloseButton.onClick.AddListener(HidePopup);
    }

    public void ShowPopup()
    {
        gameObject.SetActive(true);
    }

    private void HidePopup()
    {
        gameObject.SetActive(false);
    }

    private void ToggleAdvancedCardControls(bool isToggled)
    {
        Debug.Log("Advanced card controls: " + isToggled);
        Service.EventManager.SendEvent(EventId.AdvancedCardControlsToggled, isToggled);
        PlayerPrefs.SetInt("advancedCardControls", isToggled ? 1 : 0);
    }
}
