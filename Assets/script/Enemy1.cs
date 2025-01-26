using UnityEngine;

public class Enemy1 : BaseEnemy
{
    Animator anim;

    //distance when the player is caught
    public float followDistance;
    public float pushbackForce = 5f; // Force applied to the player on collision
    // public float stunDuration = 0.5f;
    

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        //catch the player when the player is near
        if (Vector2.Distance(transform.position, target.position) > followDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
        var player = collision.GetComponent<PlayerFreeMovement>();
        var playerRb = player.GetComponent<Rigidbody2D>();
        
        if (playerRb is not null)
        {
            Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
            player.ApplyPushback(pushDirection, pushbackForce); // Pushback with a force of 5 units
            // player.ApplyStun(stunDuration);
        }

        // Destroy this enemy
        spawner.HandleEnemyDespawn();
        Destroy(gameObject);
    }

    
}