using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    private readonly Color RED_COLOR = new Color(0.851f, 0.224f, 0.173f, 1f);
    private readonly Color BLACK_COLOR = new Color(0.137f, 0.125f, 0.129f, 1f);
    private readonly Color SELECT_COLOR = new Color(0.545f, 0.984f, 1f, 1f);
    public List<Sprite> SuitTextures;
    public List<Image> SuitSymbols;
    public List<Text> ValueText;
    public Card CardData { get; private set; }
    public Button AutoPlayButton;
    public GameObject AutoPlayNotif;

    private Button buttonBehavior;
    private Image background;
    private bool isPlayerCard;
    private bool allowAutoPlay;

    public void InitializeLocalPlayerCard(bool allowAutoPlay, Card autoPlayCard)
    {
        isPlayerCard = true;
        this.allowAutoPlay = allowAutoPlay;
        AutoPlayNotif.SetActive(autoPlayCard == CardData);
        background = gameObject.GetComponent<Image>();
        buttonBehavior = gameObject.AddComponent<Button>();
        buttonBehavior.onClick.AddListener(OnCardSelected);
        AutoPlayButton.onClick.AddListener(OnAutoPlayPressed);
        Service.EventManager.AddListener(EventId.CardSelected, OnCardSelectEvent);
        Service.EventManager.AddListener(EventId.AutoPlayCardSelected, OnAutoPlaySelectEvent);
    }

    private void OnCardSelected()
    {
        Service.EventManager.SendEvent(EventId.CardSelected, this);
    }

    private void OnAutoPlayPressed()
    {
        Service.EventManager.SendEvent(EventId.AutoPlayCardSelected, this);
    }

    private bool OnCardSelectEvent(object cookie)
    {
        CardView selectedCard = (CardView)cookie;
        bool isSelected = selectedCard == this;
        if (isPlayerCard && allowAutoPlay && PlayerPrefs.GetInt("advancedCardControls", 0) == 1)
        {
            AutoPlayButton.gameObject.SetActive(isSelected);
        }
        background.color = isSelected ? SELECT_COLOR : Color.white;
        return false;
    }

    private bool OnAutoPlaySelectEvent(object cookie)
    {
        CardView autoPlayCard = (CardView)cookie;
        bool isAlreadyAutoPlay = AutoPlayNotif.activeSelf;
        bool isAutoplay = autoPlayCard == this && !isAlreadyAutoPlay;
        AutoPlayNotif.SetActive(isAutoplay);
        return false;
    }

    public void SetCard(Card card)
    {
        string valueStr = card.FaceValue;
        for (int i = 0, count = ValueText.Count; i < count; ++i)
        {
            ValueText[i].text = valueStr;
            ValueText[i].color = 
                (card.Suit == CardSuit.Clubs || card.Suit == CardSuit.Spades) ?
                BLACK_COLOR : 
                RED_COLOR;
        }

        for (int i = 0, count = SuitSymbols.Count; i < count; ++i)
        {
            SuitSymbols[i].sprite = SuitTextures[(int)card.IntSuit - 1];
        }

        CardData = card;
    }

    public void SetEnabled(bool enabled)
    {
        buttonBehavior.interactable = enabled;
    }

    public static CardView CreateFromModel(Card card, Transform parent, bool isPlayerCard = false, bool allowAutoPlay = false, Card autoPlayCard = null)
    {
        GameObject newCard = GameObject.Instantiate(
            Resources.Load<GameObject>(string.Format("Cards/{0}", card.FaceValue)),
            parent);
        CardView newCardView = newCard.GetComponent<CardView>();
        newCardView.SetCard(card);

        if (isPlayerCard)
        {
            newCardView.InitializeLocalPlayerCard(allowAutoPlay, autoPlayCard);
        }

        return newCardView;
    }

    private void OnDestroy()
    {
        Service.EventManager.RemoveListener(EventId.CardSelected, OnCardSelectEvent);
        Service.EventManager.RemoveListener(EventId.AutoPlayCardSelected, OnAutoPlaySelectEvent);
    }
}
