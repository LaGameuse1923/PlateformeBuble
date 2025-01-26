using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : BaseEnemy //TODO: make the enemy, right now it's just a copy of enemy1
{
    Animator anim;
    
    private PolygonCollider2D _teleportArea;

    //distance when the player is caught
    public float followDistance;
    
    public override void HandleStart()
    {
        if (spawner is null)
        {
            Debug.LogError("Bubble not assigned!");
            return;
        }
        _teleportArea = spawner.bubble;
    }

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
        
        // Teleport the player to a random position within the polygon collider
        var player = collision.GetComponent<PlayerFreeMovement>();
        if (player is not null)
        {
            var newPosition = GetRandomPointInPolygon();
            Debug.Log($"Enemy2 collided with player! Teleporting to {newPosition}");
            player.teleportToPoint(newPosition);
        }

        // Destroy this enemy
        spawner.HandleEnemyDespawn();
        Destroy(gameObject);
    }
    
    private Vector2 GetRandomPointInPolygon()
    {
        if (_teleportArea is null)
        {
            Debug.LogError("Teleport area not assigned!");
            return Vector2.zero;
        }

        // Get the bounds of the PolygonCollider2D
        var bounds = _teleportArea.bounds;

        // Try to find a random point within the collider
        for (int i = 0; i < 100; i++) // Try up to 100 times
        {
            var randomPoint = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            if (IsPointInPolygon(randomPoint))
            {
                return randomPoint;
            }
        }

        // If no valid point is found, return the center of the collider as a fallback
        return bounds.center;
    }

    private bool IsPointInPolygon(Vector2 point)
    {
        // Check if the point is inside the PolygonCollider2D
        return _teleportArea.OverlapPoint(point);
    }
}
