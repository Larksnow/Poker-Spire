using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image artwork; // Reference to the Image component on the card prefab
    public TextMeshProUGUI discriptionText;
    public TextMeshProUGUI nameText;
    public int cardID;
    public Card data;
    private void Awake()
    {
        // Automatically find components if they are not assigned in the Inspector
        if (artwork == null)
            artwork = GetComponent<Image>();
    }

    public void Setup(Card cardData)
    {
        if (cardData == null)
        {
            Debug.LogError("cardData is null");
            return;
        }
        data = cardData;
        artwork.sprite = cardData.cardArt; // Set sprite from ScriptableObject
        discriptionText.text = cardData.cardInfo;
        nameText.text = cardData.cardName;
    }
}
