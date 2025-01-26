using System.Collections;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Coroutine _lifeTimeCouroutine;
    
    public float speed;
    public EnemySpawner spawner;
    public float timeToLive; // Time after which the enemy despawn
    public Transform target;
    
    private void Start()
    {
        HandleStart();
        _rigidbody = GetComponent<Rigidbody2D>();
        // target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _lifeTimeCouroutine = StartCoroutine(Die());
    }

    // Each Enemy has to implement this method with everything it wants to do in Start method
    public abstract void HandleStart();
    
    private IEnumerator Die()
    {
        yield return new WaitForSeconds(timeToLive);
        spawner.HandleEnemyDespawn();
        Destroy(gameObject);
    }
}