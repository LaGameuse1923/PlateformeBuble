using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public float speed;
    public EnemySpawner spawner;
    public float timeToLive; // Time after which the enemy despawn
    public Transform target;
    
    //TODO: Add despawn
}