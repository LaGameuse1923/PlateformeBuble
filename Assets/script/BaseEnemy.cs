using System.Collections;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Coroutine _lifeTimeCouroutine;
    
    public float speed;
    public EnemySpawner spawner;
    public float timeToLive; // Time after which the enemy despawn
    public Transform target;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _lifeTimeCouroutine = StartCoroutine(Die());
    }
    
    private IEnumerator Die()
    {
        yield return new WaitForSeconds(timeToLive);
        spawner.HandleEnemyDespawn();
        Destroy(gameObject);
    }
}