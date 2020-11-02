using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public List<Sprite> SuitTextures;
    public List<Image> SuitSymbols;
    public List<Text> ValueText;

    public void SetCard(Card card)
    {
        string valueStr = card.FaceValue;
        for (int i = 0, count = ValueText.Count; i < count; ++i)
        {
            ValueText[i].text = valueStr;
        }

        for (int i = 0, count = SuitSymbols.Count; i < count; ++i)
        {
            SuitSymbols[i].sprite = SuitTextures[(int)card.IntSuit];
        }
    }

    public static CardView CreateFromModel(Card card, Transform parent)
    {
        Debug.Log("Spawn card: " + card.FaceValue);
        GameObject newCard = GameObject.Instantiate(
            Resources.Load<GameObject>(string.Format("Cards/{0}", card.FaceValue)),
            parent);
        CardView newCardView = newCard.GetComponent<CardView>();
        newCardView.SetCard(card);
        return newCardView;
    }
}
