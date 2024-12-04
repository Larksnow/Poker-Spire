using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleManager : MonoBehaviour
{
    public static BattleManager main;
    public int cardPlayedBattle = 0;
    public int cardPlayedGame = 0;
    public int damageTotalBattle = 0;
    public int damageTotalGame = 0;
    // We plan to have 13 layers for each game
    public int layer = 1;

    public bool fatigued = false;
    public bool enableFatigue = true;
    // Start is called before the first frame update
    void Awake()
    {
        if (main) Destroy(gameObject);
        else main = this;
    }
    void Start()
    {
        EnemyManager.main.LoadEnemies();
        EnemyManager.main.SpawnEnemies();
        UIManager.main.InitializeUI();
    }
    [SerializeField]Card card;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("UerInput");
            DeckManager.main.GenerateGiftCard();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            DeckManager.main.GivePlayerCard(card);
        }
    }

    public void UpdateCardCounter()
    {
        cardPlayedBattle ++;
        cardPlayedGame ++;
    }
    public void UpdateDamageCounter(int damageAmount)
    {
        damageTotalBattle += damageAmount;
        damageTotalGame += damageAmount;
    }

    public void ApplyFatigue()
    {
        if(cardPlayedBattle > 3 && enableFatigue){
            PlayerManager.main.TakeDamage(cardPlayedBattle - 3);
            Debug.Log("!!! Taking fatigued damage " + (cardPlayedBattle - 3));
        }
    }
    public void FinishBattle()
    {
        layer ++;
        UIManager.main.UpdateLayer();
        
        if(EnemyManager.main.currentEnemy != null){
            Destroy(EnemyManager.main.currentEnemy);
        }            DeckManager.main.GenerateGiftCard();

        ResetBattle();
        if(layer == 7 || layer == 12){
            DeckManager.main.GenerateShop();
            FinishBattle();
        }else if(layer == 11){
            PlayerManager.main.TakeDamage(-20);
            FinishBattle();
        }else{
            StartBattle();
        }
    }
    public void StartBattle()
    {
        EnemyManager.main.SpawnEnemies();
        UIManager.main.UpdateEnemyHP();
    }
    public void ResetBattle()
    {
        cardPlayedBattle = 0;
        damageTotalBattle = 0;
        enableFatigue = true;
        fatigued = false;
    }

    public void SkipBattle()
    {
        Destroy(EnemyManager.main.currentEnemy);
        FinishBattle();
    }
    
}
