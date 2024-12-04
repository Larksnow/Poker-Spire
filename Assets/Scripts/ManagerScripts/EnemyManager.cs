using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager main;
    public GameObject enemyPrefab;
    public GameObject currentEnemy;
    public Transform spawnPosition; 
    public bool effectEnable = true;
     float targetScale = 0.4f;  // Adjust to desired size
    public List<Enemy> enemies = new List<Enemy>();
    void Awake()
    {
        if (main) Destroy(gameObject);
        else main = this;
    }
    void start(){
    }
    public void LoadEnemies()
    {
        enemies.AddRange(Resources.LoadAll<Enemy>("ScriptableObjects/Enemies"));
        Debug.Log("Base cards loaded: " + enemies.Count);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            // SpawnEnemies();
        }
    }
    // Spawns a wave of enemies
    public void SpawnEnemies()
    {
        // if (currentEnemy != null) return; // only one enemy exist at the same time
        int layerNumber = BattleManager.main.layer;
        List<Enemy> eligibleEnemies = new List<Enemy>();

        // Select enemies based on the current layer
        if(layerNumber < 4)
        {
            eligibleEnemies = enemies.FindAll(e => e.level == 1);
        }
        else if(layerNumber < 7)
        {
            eligibleEnemies = enemies.FindAll(e => e.level == 2);
        }
        else if(layerNumber < 11)
        {
            eligibleEnemies = enemies.FindAll(e => e.level == 3);
        }
        else if(layerNumber == 13)
        {
            eligibleEnemies = enemies.FindAll(e => e.level == 4); // Assuming level 4 is the boss level
        }

        if(eligibleEnemies.Count > 0)
        {
            // Randomly select an enemy from eligible list
            Enemy selectedEnemyData = eligibleEnemies[Random.Range(0, eligibleEnemies.Count)];

            // Instantiate enemy prefab and set its data
            currentEnemy = Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity);
            currentEnemy.transform.localScale = new Vector3(targetScale, targetScale, 1);
            BaseEnemy enemyComponent = currentEnemy.GetComponent<BaseEnemy>();
            currentEnemy.GetComponent<SpriteRenderer>().sprite = selectedEnemyData.enemyArt;
            // Initialize enemy stats based on ScriptableObject
            enemyComponent.hp = selectedEnemyData.maxHealth;
            // Other properties, like damage, can be added if needed
            
            Debug.Log($"{selectedEnemyData.name} has spawned with {enemyComponent.hp} HP.");
        }
        else
        {
            Debug.LogWarning("No eligible enemies found for the current layer.");
        }
    }
    
}


