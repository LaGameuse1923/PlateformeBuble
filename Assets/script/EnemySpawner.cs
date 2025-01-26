using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform player; // Reference to the player's position
    public PolygonCollider2D bubble; // Reference to the bubble's polygon collider
    public float spawnRadius = 5f; // Radius around the player where enemies can spawn
    public float spawnInterval = 2f; // Time (in seconds) between spawns
    public GameObject[] enemyPrefabs; // Array of possible enemy prefabs
    public int maxEnemyCount; // Maximum number of enemies for this level

    private bool _invalid = false; // To only log error once when prefabs are not set 
    private int _currentEnemyCount = 0;
    private Coroutine _spawnRoutine;
    
    private void Start()
    {
        // Check if there are any prefabs to spawn
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogError("Spawner is deactivated because no prefabs were set");
            _invalid = true;
            return;
        }

        // Start the spawning coroutine
        _spawnRoutine = StartCoroutine(SpawnEnemiesCoroutine());
    }
    
    private IEnumerator SpawnEnemiesCoroutine()
    {
        var spawned = false;
        while (!_invalid)
        {
            // Spawn enemies until the max count for the level is reached
            if (_currentEnemyCount < maxEnemyCount)
            {
                spawned = SpawnEnemy();
            }

            // Wait for the specified interval before spawning again if the enemy was spawned, else retry
            if (spawned)
                yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Return true if the enemy was succesufully spawned
    private bool SpawnEnemy()
    {
        // Choose a random enemy prefab (one of the allowed for level)
        var enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // Generate a random position along the spawn radius
        var spawnPosition2D = Random.insideUnitCircle.normalized * spawnRadius;
        var spawnPosition = new Vector3(spawnPosition2D.x, spawnPosition2D.y, 0f) + player.position;

        // Check if the spawn position is inside the bubble
        if (!IsInsideBubble(spawnPosition)) return  false; // Retry spawn if position is outside the bubble

        // Spawn the enemy
        var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        var enemyScript = enemy.GetComponent<BaseEnemy>();
        enemyScript.spawner = this;
        enemyScript.target = player;
        _currentEnemyCount++;

        return true;
    }

    private bool IsInsideBubble(Vector3 position)
    {
        // Check if the position is within the polygon collider
        if (bubble is not null) return bubble.OverlapPoint(position);
        
        Debug.LogError("Bubble collider not assigned!");
        return false;
    }

    public void HandleEnemyDespawn()
    {
        _currentEnemyCount--;
    }
    
    private void OnDestroy()
    {
        // Stop the coroutine when the spawner is destroyed
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
        }
    }
}
