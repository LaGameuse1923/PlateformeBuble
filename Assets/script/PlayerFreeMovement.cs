using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovement : MonoBehaviour
{
    private Rigidbody2D _rb;

    public float vitesse = 5;
    private Vector3 velocity = Vector3.zero;
    private float horizontalMovement;
    private float verticalMovement;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMovement = Input.GetAxis("Horizontal") * vitesse;
        verticalMovement = Input.GetAxis("Vertical") * vitesse;

        DeplacementPlayer(horizontalMovement, verticalMovement);
    }
    
    public void DeplacementPlayer(float _horizontalMovement, float _verticalMovement)
    {
        Vector3 targetVelocity = new Vector2(_horizontalMovement, _verticalMovement);
        _rb.velocity = Vector3.SmoothDamp(_rb.velocity, targetVelocity, ref velocity, .05f);
    }
}
