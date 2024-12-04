using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public int hp;
    private int goldAmount = 0;
    public void TakeDamage(int amount)
    {
        // positiove amount will decrease hp
        // negative amount will heal hp
        hp -= amount;
        UIManager.main.UpdateEnemyHP();
        if (hp <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        goldAmount = 0 - hp;
        Debug.Log($"{gameObject.name} has been defeated.");
        PlayerManager.main.addGold(goldAmount);
        BattleManager.main.FinishBattle();
    }
}
