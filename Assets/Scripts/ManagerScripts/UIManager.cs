using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager main;

    [Header("UI Text Elements")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public TextMeshProUGUI layerNum;

    private void Awake()
    {
        // Singleton pattern to access UIManager globally
        if (main == null) main = this;
        else Destroy(gameObject);
    }
    public void InitializeUI()
    {
        UpdateGold();
        UpdatePlayerHP();
        UpdateLayer();
        // Optional: Check if an enemy is already spawned, then set enemy HP UI
        if (EnemyManager.main.currentEnemy != null)
        {
            UpdateEnemyHP();
        }
        else
        {
            enemyHPText.text = "Enemy HP: N/A"; // Or some default text
        }
    }
    // Method to update player gold
    public void UpdateGold()
    {
        goldText.text = "Gold: " + PlayerManager.main.gold;
    }

    // Method to update player HP
    public void UpdatePlayerHP()
    {
        playerHPText.text = "Player HP: " + PlayerManager.main.hp + " / " + PlayerManager.main.maxHealth;
    }

    // Method to update enemy HP
    public void UpdateEnemyHP()
    {
        enemyHPText.text = "Enemy HP: " + EnemyManager.main.currentEnemy.GetComponent<BaseEnemy>().hp;
    }

    public void UpdateLayer()
    {
        layerNum.text = "Current Layer: " + BattleManager.main.layer;
    }
}
