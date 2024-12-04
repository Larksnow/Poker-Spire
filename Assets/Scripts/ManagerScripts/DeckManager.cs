using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
public class DeckManager : MonoBehaviour
{
    // References to the card ScriptableObjects
    public static DeckManager main;
    private int currentID = 0;
    private int hierarchyIndex;
    [SerializeField]private float boundary = 600;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject handArea;
    [SerializeField] private GameObject pickArea;
    [SerializeField] private GameObject pickPannel;
    private int noCardDanamge = 1;
    public Button[] cardButtons;
    public List<Card> playerDeck = new List<Card>();
    public List<GameObject> cardInstances = new List<GameObject>();
    public Dictionary<int, Card> cardToIDMap = new Dictionary<int, Card>();


    void Awake()
    {
        if (main) Destroy(gameObject);
        else main = this;
    }

    void Update(){
        if(playerDeck.Count == 0)
        {
            //Ran out of cards
            PlayerManager.main.TakeDamage(noCardDanamge);
            Card randomCard = CardManager.main.GenerateRandomCard();
            GivePlayerCard(randomCard);
            noCardDanamge ++;
        }
    }
    public void GivePlayerCard(Card card)
    {
        // Add the specified card to the player's deck
        pickPannel.SetActive(false);
        GameObject newCardUI = Instantiate(cardPrefab, handArea.transform);
        // Apply card data to its UI element
        newCardUI.GetComponent<CardDisplay>().Setup(card);
        newCardUI.GetComponent<CardDisplay>().cardID = currentID;
        hierarchyIndex = newCardUI.transform.GetSiblingIndex();
        newCardUI.GetComponent<HoverableCard>().Initialize(newCardUI, hierarchyIndex);
        cardToIDMap[currentID] = card;
        playerDeck.Add(card);
        currentID ++;
        // Track the new card UI instance
        cardInstances.Add(newCardUI);
        PositionCards();
        Debug.Log("Player received card: " + card.name);
    }

    public void GenerateGiftCard()
    {
        int layer = BattleManager.main.layer;
        pickPannel.SetActive(true);
        int rarity = 0;
        if(layer < 4){
            rarity = 1;
        }else if(layer < 7){
            rarity = 2;
        }else if(layer < 11){
            rarity = 3;
        }
        List<GameObject> cardInstances = new List<GameObject>();
        Card card1 = CardManager.main.GenerateRandomCardWithRarity(rarity);
        Card card2 = CardManager.main.GenerateRandomCardWithRarity(rarity);
        Card card3 = CardManager.main.GenerateRandomCardWithRarity(rarity);
        GameObject newCard1 = Instantiate(cardPrefab, pickArea.transform);
        GameObject newCard2 = Instantiate(cardPrefab, pickArea.transform);
        GameObject newCard3 = Instantiate(cardPrefab, pickArea.transform);
        newCard1.GetComponent<CardDisplay>().Setup(card1);
        newCard2.GetComponent<CardDisplay>().Setup(card2);
        newCard3.GetComponent<CardDisplay>().Setup(card3);
        cardInstances.Add(newCard1);
        cardInstances.Add(newCard2);
        cardInstances.Add(newCard3);
        int count = 3;
        Vector3 pickCenter = pickArea.transform.position; // Get the center position of the hand area
        float totalWidth = pickArea.GetComponent<RectTransform>().rect.width - boundary; // Get the width of the hand area
        // Clear previous positions
        for (int i = 0; i < count; i++)
        {
            float xPosition = CalculateCardPosition(i, count, totalWidth);
            int index = i; 
            cardInstances[i].transform.position = new Vector3(xPosition, pickCenter.y, pickCenter.z); // Set card 
            cardButtons[i].interactable = true; // Ensure the button is interactable
            cardButtons[i].onClick.AddListener(() => GivePlayerCard(cardInstances[index].GetComponent<CardDisplay>().data));
        }
    }
    public void GenerateShop()
    {
        pickPannel.SetActive(true);
        List<GameObject> cardInstances = new List<GameObject>();
        Card card1 = CardManager.main.jokerCards[0];
        Card card2 = CardManager.main.jokerCards[1];
        Card card3 = CardManager.main.jokerCards[2];
        GameObject newCard1 = Instantiate(cardPrefab, pickArea.transform);
        GameObject newCard2 = Instantiate(cardPrefab, pickArea.transform);
        GameObject newCard3 = Instantiate(cardPrefab, pickArea.transform);
        newCard1.GetComponent<CardDisplay>().Setup(card1);
        newCard2.GetComponent<CardDisplay>().Setup(card2);
        newCard3.GetComponent<CardDisplay>().Setup(card3);
        cardInstances.Add(newCard1);
        cardInstances.Add(newCard2);
        cardInstances.Add(newCard3);
        int count = 3;
        Vector3 pickCenter = pickArea.transform.position; // Get the center position of the hand area
        float totalWidth = pickArea.GetComponent<RectTransform>().rect.width - boundary; // Get the width of the hand area
        // Clear previous positions
        for (int i = 0; i < count; i++)
        {
            float xPosition = CalculateCardPosition(i, count, totalWidth);
            int index = i; 
            cardInstances[i].transform.position = new Vector3(xPosition, pickCenter.y, pickCenter.z); // Set card 
            cardButtons[i].interactable = true; // Ensure the button is interactable
            cardButtons[i].onClick.AddListener(() => GivePlayerCard(cardInstances[index].GetComponent<CardDisplay>().data));
        }
    }
    public void InitializeStartDeck()
    {
        clearDeck();
        // Add all base cards to the player's deck
        if(CardManager.main.baseCards == null)
        {
            Debug.LogWarning("Base cards haven't been loaded before initialization");
            return;
        }
        foreach(var card in CardManager.main.baseCards)
        {
            GivePlayerCard(card);
        }
        Debug.Log("Player's deck initialized with starting cards.");
    }

    public void clearDeck()
    {
        playerDeck.Clear();
        cardInstances.Clear();
        cardToIDMap.Clear();
        foreach (Transform child in handArea.transform)
        {
            Destroy(child.gameObject);
        }
        PositionCards();
        Debug.Log("Player deck has been cleared");
    }
    public void PlayCard(GameObject cardInstance, PlayerManager player, BaseEnemy enemy)
    {
        // Find the associated Card data by searching for the cardInstance in the map values
        Card cardData = null;
        int cardID = cardInstance.GetComponent<CardDisplay>().cardID;
        cardData = cardToIDMap[cardID];

        // If we found a corresponding card
        if (cardData != null)
        {
            // Execute the card's effect using CardManager
            CardManager.main.ApplyCardEffect(cardData, player, enemy);
            Debug.Log("Card: " + cardData.name + " has been played");
       
            // Remove the card from the player's deck
            playerDeck.Remove(cardData);
            // Remove the card from the map and destroy the UI instance
            cardToIDMap.Remove(cardID);
            cardInstances.Remove(cardInstance);
            Destroy(cardInstance);
            // Update Battle data
            BattleManager.main.UpdateCardCounter();
            BattleManager.main.ApplyFatigue();
            PositionCards();
        }
        else
        {
            Debug.LogWarning("Card data not found for the given card instance.");
        }
    }

    public void PositionCards()
    {
        int count = cardInstances.Count;
        Vector3 handAreaCenter = handArea.transform.position; // Get the center position of the hand area
        float totalWidth = handArea.GetComponent<RectTransform>().rect.width - boundary; // Get the width of the hand area
        // Clear previous positions
        for (int i = 0; i < count; i++)
        {
            float xPosition = CalculateCardPosition(i, count, totalWidth);
            cardInstances[i].transform.position = new Vector3(xPosition, handAreaCenter.y, handAreaCenter.z); // Set card position
            cardInstances[i].GetComponent<HoverableCard>().origionalPosition = cardInstances[i].transform.position;
        }
    }

    private float CalculateCardPosition(int index, int count, float totalWidth)
    {
        float leftEdge = handArea.transform.position.x - totalWidth / 2;
        if (count == 1)
        {
            // Centered position for a single card
            return handArea.transform.position.x;
        }
        else
        {
            // Calculate position based on the number of cards
            return leftEdge + index * (totalWidth / (count - 1));
        }
    }
    public void BringToFront(GameObject card)
    {
        card.transform.SetAsLastSibling(); // Moves the card to the top of the hierarchy
    }

    public void BringToOrigin(GameObject card, int index)
    {
        card.transform.SetSiblingIndex(index);
    }

}
