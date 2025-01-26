using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : BaseEnemy //TODO: make the enemy, right now it's just a copy of enemy1
{
    Animator anim;

    //distance when the player is caught
    public float followDistance;
    public float catchDistance;
    public bool iscaught = false;
    

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move()
    {
        //catch the player when the player is near
        if (Vector2.Distance(transform.position, target.position) > followDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }
}
