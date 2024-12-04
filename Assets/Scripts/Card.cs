using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Card.cs - The Card ScriptableObject holding its data and effect
[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public Sprite cardArt;
    public string cardInfo;
    public int value;
    public int rarity;
}
