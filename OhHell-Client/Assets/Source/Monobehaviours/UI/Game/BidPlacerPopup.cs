using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BidPlacerPopup : MonoBehaviour
{
    public List<Button> BidButtons;
    public Button SubmitButton;
    public GameObject BidPlacedText;
    public GameObject LeadNotification;

    private int bidIndex;
    private bool bidPlaced;

    private void Start()
    {
        for (int i = 0, count = BidButtons.Count; i < count; ++i)
        {
            Button thisButton = BidButtons[i];
            thisButton.onClick.AddListener(() => { OnBidButtonClicked(thisButton); });
        }
        SubmitButton.onClick.AddListener(OnSubmitClicked);
    }

    public void ShowBidPopup(GameData gameData, PlayerData player)
    {
        bidPlaced = false;
        int maxBid = gameData.NumCardsToDeal;
        for (int i = 0, count = BidButtons.Count; i < count; ++i)
        {
            Button thisButton = BidButtons[i];
            thisButton.gameObject.SetActive(i <= maxBid);
            thisButton.interactable = true;
        }
        SubmitButton.gameObject.SetActive(true);
        BidPlacedText.SetActive(false);
        LeadNotification.SetActive(gameData.Players.IndexOf(player) == gameData.CurrentLeaderIndex);
        gameObject.SetActive(true);
    }

    private void OnBidButtonClicked(Button clickedButton)
    {
        Debug.Log(clickedButton.name);
        if (!bidPlaced)
        {
            for (int i = 0, count = BidButtons.Count; i < count; ++i)
            {
                Button thisButton = BidButtons[i];
                thisButton.interactable = thisButton != clickedButton;
                bidIndex = thisButton == clickedButton ? i : bidIndex;
            }
        }
    }

    private void OnSubmitClicked()
    {
        bidPlaced = true;
        Service.EventManager.SendEvent(EventId.LocalBidPlaced, bidIndex);
        BidPlacedText.SetActive(true);
        LeadNotification.SetActive(false);
        SubmitButton.gameObject.SetActive(false);
    }

    public void HideBidPlacerPopup()
    {
        gameObject.SetActive(false);
    }
}
