using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // Singleton instance
    public static CardManager main;
    private int evenBonus = 0; // Bonus for next even card
    private int oddBonus = 0;  // Bonus for next odd card
    private int doubleBouns = 0; // Bouns for double damage
    void Awake()
    {
        if (main) Destroy(gameObject);
        else main = this;
    }
    public List<Card> baseCards = new List<Card>();   // Pool for base rarity cards
    public List<Card> epicCards = new List<Card>();   // Pool for epic rarity cards
    public List<Card> legendCards = new List<Card>(); // Pool for legend rarity cards
    public List<Card> jokerCards = new List<Card>();  // Pool for joker rarity cards
    
    private void Start()
    {
        LoadCards();
        DeckManager.main.InitializeStartDeck();
    }

    private void LoadCards()
    {
        baseCards.AddRange(Resources.LoadAll<Card>("ScriptableObjects/Cards/Base"));
        Debug.Log("Base cards loaded: " + baseCards.Count);

        epicCards.AddRange(Resources.LoadAll<Card>("ScriptableObjects/Cards/Epic"));
        Debug.Log("Epic cards loaded: " + epicCards.Count);

        legendCards.AddRange(Resources.LoadAll<Card>("ScriptableObjects/Cards/Legend"));
        Debug.Log("Legend cards loaded: " + legendCards.Count);

        jokerCards.AddRange(Resources.LoadAll<Card>("ScriptableObjects/Cards/Joker"));
        Debug.Log("Joker cards loaded: " + jokerCards.Count);
    }

    // 1. Generate random card with specified rarity
    public Card GenerateRandomCardWithRarity(int rarity)
    {
        List<Card> pool;
        switch (rarity)
        {
            case 1: pool = baseCards; break;
            case 2: pool = epicCards; break;
            case 3: pool = legendCards; break;
            default: return null;
        }
        return pool[Random.Range(0, pool.Count)];
    }

    // 2. Generate random card among all rarities with defined probabilities
    public Card GenerateRandomCard()
    {
        float randomValue = Random.value * 100;

        if (randomValue < 70)
        {
            return GenerateRandomCardWithRarity(1); // 70% chance for base card
        }
        else if (randomValue < 90)
        {
            return GenerateRandomCardWithRarity(2); // 20% chance for epic card
        }
        else
        {
            return GenerateRandomCardWithRarity(3); // 10% chance for legend card
        }
    }

    // 3. Function to generate a Joker card
    public Card GenerateJokerCard()
    {
        return jokerCards[Random.Range(0, jokerCards.Count)];
    }

    // Method to apply the effect based on the card's name or type
    public void ApplyCardEffect(Card card, PlayerManager player, BaseEnemy enemy)
    {
        string cardName = card.name;
        int cardRairty = card.rarity;
        int damageAmount = card.value;
        List<GameObject> cardList;
        Card randomCard;
        // Apply bonus damage first
        if (damageAmount % 2 == 0) // Even card
        {
            damageAmount += evenBonus; // Apply even bonus
            evenBonus = 0; // Reset the bonus after use
        }
        else // Odd card
        {
            damageAmount += oddBonus; // Apply odd bonus
            oddBonus = 0; // Reset the bonus after use
        }
        if (doubleBouns != 0)
        {
            damageAmount *= doubleBouns;
            doubleBouns = 0;
        }
        switch (cardName)
        {
            // Epic cards start
            case "EpicA":
                EnemyManager.main.effectEnable = false;
                break;
            // Makes your next card immune to enemy effects
            case "Epic2":
            // Randomly play one of your other cards
                cardList = DeckManager.main.cardInstances;
                List<GameObject> otherCards = cardList.FindAll(c => 
                {
                    CardDisplay cardDisplay = c.GetComponent<CardDisplay>();
                    return cardDisplay != null && cardDisplay.data.name != "Epic2";
                });
                // Make sure there's at least one other card to choose from
                if (otherCards.Count > 0)
                {
                    GameObject selectedCard = otherCards[Random.Range(0, otherCards.Count)];
                    Debug.Log("Card " + selectedCard.GetComponent<CardDisplay>().data.name + " has beed chooses");
                    DeckManager.main.PlayCard(selectedCard, player, enemy);
                }
                else
                {
                    Debug.Log("No other cards available to play.");
                }
                break;
            case "Epic3":
            // If your card count is a multiple of 3, deal triple damage
                cardList = DeckManager.main.cardInstances;
                if(cardList.Count % 3 == 0)
                {
                    damageAmount = 3 * damageAmount;
                }
                break;
            case "Epic4":
            // Your next even card deals +4 damage
                evenBonus += 4;
                break;
            case "Epic5":
            // Your next odd card deals +5 damage
                oddBonus += 5;
                break;
            case "Epic6":
            // If the enemy's health contains the number 6, deal an additional 6 damage
                if (enemy.hp.ToString().Contains("6"))
                {
                    damageAmount += 6; // Add the extra damage
                    Debug.Log("Epic6 activated! Additional 6 damage applied.");
                }
                break;
            case "Epic7":
            // Gain a card with a value greater than 7
                do{
                    randomCard = GenerateRandomCard();
                } while (randomCard.value <= 7);
                DeckManager.main.GivePlayerCard(randomCard);
                break;
            case "Epic8":
            // Gain a card with a value less than 8
                do{
                    randomCard = GenerateRandomCard();
                } while (randomCard.value >= 8);
                DeckManager.main.GivePlayerCard(randomCard);
                break;
            case "Epic9":
            // For each card played in this battle, deal an extra 1 damage
                damageAmount += BattleManager.main.cardPlayedBattle;
                break;
            case "Epic10":
            // If this card is your highest card, deal an additional 10 damage
                bool isHighest = true;
                foreach (Card otherCard in DeckManager.main.playerDeck)
                {
                    if (otherCard.value > card.value)
                    {
                        isHighest =  false;
                    }
                }
                if(isHighest)
                {
                    damageAmount += 10;
                }
                break;
            case "EpicJ":
            // If this is your last card, instantly defeat the enemy (does not apply to bosses)
                if(DeckManager.main.playerDeck.Count == 1)
                {
                    enemy.OnDeath();
                }
                break;
            case "EpicQ":
            // If you deal exactly 12 damage this battle, deals an additional 12 damage
                if(BattleManager.main.damageTotalBattle == 12)
                {
                    damageAmount += 12;
                }
                break;
            case "EpicK":
            // Deal 13 damage to yourself and double the damage dealt to the enemy
                if(player.hp > 13){
                    player.TakeDamage(13);
                    damageAmount = damageAmount * 2;
                }
                break;
            
            // Legend cards start
            case "LegendA":
            // You will not be fatigued in this battle
                BattleManager.main.enableFatigue = false;
                break;
            case "Legend2":
            // Heal yourself equal to the damage dealt by this card
                player.TakeDamage(-damageAmount);
                break;
            case "Legend3":
            // Gain random cards equal to the damage dealt
                for(int i = 0; i < damageAmount; ++i)
                {
                    randomCard = GenerateRandomCard();
                    DeckManager.main.GivePlayerCard(randomCard);
                }
                break;
            case "Legend4":
            // Deal extra damage for each card played in this game round
                damageAmount += BattleManager.main.cardPlayedGame;
                break;
            case "Legend5":
            // Deal extra damage equal to the number of cards in your deck
                damageAmount += DeckManager.main.playerDeck.Count;
                break;
            case "Legend6":
            // If the enemy's health contains the number 6, deal 6 times damage
                if (player.hp.ToString().Contains("6"))
                {
                    damageAmount *= 6;
                    Debug.Log("Legend6 activated! Additional 6 x damage applied.");
                }
                break;
            case "Legend7":
            // Deal extra damage equal to your lost health
                damageAmount += player.maxHealth - player.hp;
                break;
            case "Legend8":
            // Gain an 8-point increase to your maximum health
                player.maxHealth += 8;
                break;
            case "Legend9":
            // If your health, the enemy's health, and your card count all contain the number 9, deal 9 times damage
                if(player.hp.ToString().Contains("9") && 
                   enemy.hp.ToString().Contains("9") && 
                   DeckManager.main.playerDeck.Count.ToString().Contains("9"))
                {
                    damageAmount *= 9;
                    Debug.Log("Legend9 activated! Additional 9 x damage applied.");
                }
                break;
            case "Legend10":
            // If your health, the enemy's health and your card count are multiples of 10, gain a Joker card
                if(player.hp % 10 == 0 && enemy.hp % 10 == 0 && DeckManager.main.playerDeck.Count % 10 == 0)
                {
                    randomCard = GenerateJokerCard();
                    DeckManager.main.GivePlayerCard(randomCard);
                }
                break;
            case "LegendJ":
            // Skip the current battle (does not apply to bosses)
                BattleManager.main.SkipBattle();
                break;
            case "LegendQ":
            // This card does not deal damage, instead, it restores health
                player.TakeDamage(-damageAmount);
                damageAmount = 0;
                break;
            case "LegendK":
            // When this card defeats an enemy, gain double gold
                doubleBouns = 2;
                break;
            default:
                break;

            // Joker cards start
            case "JokerA":
            // Deal damage equal to the total damage you have dealt in this game round
                damageAmount += BattleManager.main.damageTotalGame;
                break;
            case "JokerB":
            // Fully restore your health
                player.hp = player.maxHealth;
                damageAmount = 0;
                break;
            case "JokerC":
            // Discard all your cards and replace them with rarer random cards
                ///stats all rarity cards first
                // Step 1: Count cards of each rarity
                int baseCount = 0;
                int epicCount = 0;
                int legendCount = 0;
                int jokerCount = -1;
                foreach (var entry in DeckManager.main.playerDeck)
                {
                    switch (entry.rarity)
                    {
                        case 1: // Base rarity
                            baseCount++;
                            break;
                        case 2: // Epic rarity
                            epicCount++;
                            break;
                        case 3: // Legend rarity
                            legendCount++;
                            break;
                        case 4: // Joker rarity
                            jokerCount++;
                            break;
                    }
                }
                DeckManager.main.clearDeck();
                for (int i = 0; i < baseCount; i++)
                {
                    randomCard = GenerateRandomCardWithRarity(2); // Generate an Epic card
                    DeckManager.main.GivePlayerCard(randomCard);
                }
                for (int i = 0; i < epicCount; i++)
                {
                    randomCard = GenerateRandomCardWithRarity(3); // Generate a Legend card
                    DeckManager.main.GivePlayerCard(randomCard);
                }
                for (int i = 0; i < legendCount; i++)
                {
                    randomCard= GenerateRandomCardWithRarity(3); // Generate a Legend card
                    DeckManager.main.GivePlayerCard(randomCard);
                }
                for (int i = 0; i < jokerCount; i++)
                {
                    randomCard = GenerateJokerCard(); // Generate a joker card
                    DeckManager.main.GivePlayerCard(randomCard);
                }
                break;
        }
        // All cards will deal damage of its value
        Debug.Log("Card " + cardName + " deal " + damageAmount + " damage!");
        enemy.TakeDamage(damageAmount);
        BattleManager.main.UpdateDamageCounter(damageAmount);
    }
}
