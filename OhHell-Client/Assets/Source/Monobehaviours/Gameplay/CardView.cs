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

    private Button buttonBehavior;
    private Image background;

    public void AddButtonBehavior()
    {
        background = gameObject.GetComponent<Image>();
        buttonBehavior = gameObject.AddComponent<Button>();
        buttonBehavior.onClick.AddListener(OnCardSelected);
        Service.EventManager.AddListener(EventId.CardSelected, OnCardSelectEvent);
    }

    private void OnCardSelected()
    {
        Service.EventManager.SendEvent(EventId.CardSelected, this);
    }

    private bool OnCardSelectEvent(object cookie)
    {
        CardView selectedCard = (CardView)cookie;
        background.color = selectedCard == this ? SELECT_COLOR : Color.white;
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

    public static CardView CreateFromModel(Card card, Transform parent, bool isPlayerCard = false)
    {
        GameObject newCard = GameObject.Instantiate(
            Resources.Load<GameObject>(string.Format("Cards/{0}", card.FaceValue)),
            parent);
        CardView newCardView = newCard.GetComponent<CardView>();
        newCardView.SetCard(card);

        if (isPlayerCard)
        {
            newCardView.AddButtonBehavior();
        }

        return newCardView;
    }

    private void OnDestroy()
    {
        Service.EventManager.RemoveListener(EventId.CardSelected, OnCardSelectEvent);
    }
}
