using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager main;
    public int maxHealth = 100;
    public int hp = 100;
    public int gold = 0;
    public TextMeshProUGUI gameOverText;
    // public List<Card> deck;
    // public List<Card> hand;

    // Check if there is already an instance of this class
    void Awake()
    {
        if (main) Destroy(gameObject);
        else main = this;
    }
    void Start()
    {
        hp = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        // positiove amount will decrease hp
        // negative amount will heal hp
        hp -= amount;
        UIManager.main.UpdatePlayerHP();
        if(hp >= maxHealth){
            hp = maxHealth;
        }
        else if(hp <= 0){
            GameOver();
        }
    }

    public void addGold(int amount){
        // Gain gold amount is positive, purchase is negative
        gold += amount;
        UIManager.main.UpdateGold();
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        gameOverText.text = "GAME OVER!";
    }
}
