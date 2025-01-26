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
    public Animator animator;
    private Vector3 change;
    
    private bool _isStunned = false;
    

    public bool effacer = false;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isStunned)
            return;
        
        horizontalMovement = Input.GetAxis("Horizontal") * vitesse;
        verticalMovement = Input.GetAxis("Vertical") * vitesse;

        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        DeplacementPlayer(horizontalMovement, verticalMovement);

        if (change != Vector3.zero)
        {

            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("mouvement", true);
        }
        else
        {
            animator.SetBool("mouvement", false);
        }
    }
    
    public void DeplacementPlayer(float _horizontalMovement, float _verticalMovement)
    {
        Vector3 targetVelocity = new Vector2(_horizontalMovement, _verticalMovement);
        _rb.velocity = Vector3.SmoothDamp(_rb.velocity, targetVelocity, ref velocity, .05f);
    }

    public void Effacer()
    {
        effacer = true;
        animator.SetBool("Effacer", effacer);
        effacer = false;
        animator.SetBool("Effacer", effacer);
    }
    
    // Method to apply stun effect
    // public void ApplyStun(float duration)
    // {
    //     _isStunned = true;
    //     _rb.velocity = Vector2.zero; // Stop movement
    //     Invoke(nameof(RemoveStun), duration);
    // }
    //
    // private void RemoveStun()
    // {
    //     _isStunned = false;
    // }

    public void teleportToPoint(Vector2 point)
    {
        transform.position = point;
    }
    
    public void ApplyPushback(Vector2 direction, float magnitude)
    {
        StartCoroutine(PushbackCoroutine(direction, magnitude));
    }
    
    private IEnumerator PushbackCoroutine(Vector2 direction, float magnitude)
    {
        var duration = 0.2f; // Pushback lasts for 0.2 seconds
        var elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position += (Vector3)(direction * (magnitude * Time.deltaTime));
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    
}
