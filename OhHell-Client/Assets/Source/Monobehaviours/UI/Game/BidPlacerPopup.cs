using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BidPlacerPopup : MonoBehaviour
{
    public List<Button> BidButtons;
    public Button SubmitButton;
    public Text BidPlacedText;

    private int bidIndex;

    private void Start()
    {
        for (int i = 0, count = BidButtons.Count; i < count; ++i)
        {
            Button thisButton = BidButtons[i];
            thisButton.onClick.AddListener(() => { OnBidButtonClicked(thisButton); });
        }
        SubmitButton.onClick.AddListener(OnSubmitClicked);
    }

    public void ShowBidPopup(GameData gameData)
    {
        int maxBid = gameData.NumCardsToDeal;
        for (int i = 0, count = BidButtons.Count; i < count; ++i)
        {
            BidButtons[i].gameObject.SetActive(i <= maxBid);
        }
        SubmitButton.gameObject.SetActive(true);
        BidPlacedText.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    private void OnBidButtonClicked(Button clickedButton)
    {
        Debug.Log(clickedButton.name);
        for (int i = 0, count = BidButtons.Count; i < count; ++i)
        {
            Button thisButton = BidButtons[i];
            thisButton.interactable = thisButton != clickedButton;
            bidIndex = thisButton == clickedButton ? i : bidIndex;
        }
    }

    private void OnSubmitClicked()
    {
        Service.EventManager.SendEvent(EventId.LocalBidPlaced, bidIndex);
        BidPlacedText.gameObject.SetActive(true);
        SubmitButton.gameObject.SetActive(false);
    }

    public void HideBidPlacerPopup()
    {
        gameObject.SetActive(false);
    }
}
