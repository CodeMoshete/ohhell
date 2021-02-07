using UnityEngine;
using UnityEngine.UI;

public class OptionsPopup : MonoBehaviour
{
    public Toggle AdvancedCardControlsToggle;
    public Toggle TurnAudioNotificationsToggle;
    public Button CloseButton;

    private void Start()
    {
        AdvancedCardControlsToggle.onValueChanged.AddListener(ToggleAdvancedCardControls);
        TurnAudioNotificationsToggle.onValueChanged.AddListener(ToggleAudioNotifications);
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
        Service.LocalPreferences.EnableAdvancedCardControls = isToggled;
    }

    private void ToggleAudioNotifications(bool isToggled)
    {
        Debug.Log("Advanced card controls: " + isToggled);
        Service.LocalPreferences.DisableTurnNotification = !isToggled;
        Service.EventManager.SendEvent(EventId.AdvancedCardControlsToggled, isToggled);
    }
}
