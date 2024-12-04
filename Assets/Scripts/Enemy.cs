using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Card.cs - The Card ScriptableObject holding its data and effect
[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public string effect;
    public Sprite enemyArt;
    public string effectInfo;
    public int maxHealth;
    public int level;
}
