using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rb;

    public float vitesse = 3;
    private Vector3 velocity = Vector3.zero;
    private float horizontalMovement;
    public float jump = 3;
    private Vector3 change;


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        horizontalMovement = Input.GetAxis("Horizontal") * vitesse;
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");

        DeplacementPlayer(horizontalMovement);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump(jump);
        }
    }

    private void Jump(float jumpForce)
    {
        _rb.velocity += new Vector2(_rb.velocity.x, jumpForce);
    }

    public void DeplacementPlayer(float _horizontalMovement)
    {
        Vector3 targetVelocity = new Vector2(_horizontalMovement, _rb.velocity.y);
        _rb.velocity = Vector3.SmoothDamp(_rb.velocity, targetVelocity, ref velocity, .15f);
    }
}
