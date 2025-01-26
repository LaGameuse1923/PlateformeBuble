using System.Collections;
using UnityEngine;

public class PlayerFreeMovement : MonoBehaviour
{
    public Animator animator;
    public float vitesse = 5;
    public Camera cam;
    public float dezoomFactor = 2f; // The factor by which the camera size is multiplied on dezoom
    public float dezoomDuration = 5f; // How long the dezoom lasts
    public float transitionSpeed = 0.5f; // Speed of the zoom transitio
    public float cooldownDuration = 3f; // Cooldown duration
    
    private float initialScale; // The initial size of the camera
    private Rigidbody2D _rb;
    private Vector3 _velocity = Vector3.zero;
    private float _horizontalMovement;
    private float _verticalMovement;
    private Vector3 _change;
    private bool _isStunned = false;
    private bool _isDezooming = false;
    private bool _isCooldown = false;
    

    public bool effacer = false;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Store the camera's initial size
        if (cam is not null)
            initialScale = cam.orthographicSize;
        else
            Debug.LogError("Player Camera not assigned!");
    }

    // Update is called once per frame
    void Update()
    {
        if (_isStunned)
            return;
        
        _horizontalMovement = Input.GetAxis("Horizontal") * vitesse;
        _verticalMovement = Input.GetAxis("Vertical") * vitesse;

        _change = Vector3.zero;
        _change.x = Input.GetAxisRaw("Horizontal");
        _change.y = Input.GetAxisRaw("Vertical");

        DeplacementPlayer(_horizontalMovement, _verticalMovement);

        if (_change != Vector3.zero)
        {

            animator.SetFloat("moveX", _change.x);
            animator.SetFloat("moveY", _change.y);
            animator.SetBool("mouvement", true);
        }
        else
        {
            animator.SetBool("mouvement", false);
        }
        
        HandleDezoomInput();
    }
    
    public void DeplacementPlayer(float _horizontalMovement, float _verticalMovement)
    {
        Vector3 targetVelocity = new Vector2(_horizontalMovement, _verticalMovement);
        _rb.velocity = Vector3.SmoothDamp(_rb.velocity, targetVelocity, ref _velocity, .05f);
    }

    public void Effacer()
    {
        EffacerTrue();
        
        Invoke("EffacerFalse",0.9f);
    }

    public void EffacerTrue()
    {
        effacer = true;
        animator.SetBool("Effacer", effacer);
    }
    
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

    public void EffacerFalse()
    {
        effacer = false;
        
        animator.SetBool("Effacer", effacer);
    }
    
    
    private void HandleDezoomInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isDezooming && !_isCooldown)
        {
            StartCoroutine(DezoomCamera());
        }
    }
    
    private IEnumerator DezoomCamera()
    {
        _isDezooming = true;

        // Smoothly transition to the dezoom scale
        yield return StartCoroutine(SmoothZoom(initialScale*dezoomFactor));

        // Wait for the dezoom duration
        yield return new WaitForSeconds(dezoomDuration);

        // Smoothly transition back to the initial scale
        yield return StartCoroutine(SmoothZoom(initialScale));

        // Start cooldown
        _isDezooming = false;
        StartCoroutine(DezoomCooldown());    
    }
    
    private IEnumerator SmoothZoom(float targetScale)
    {
        while (!Mathf.Approximately(cam.orthographicSize, targetScale))
        {
            cam.orthographicSize = Mathf.MoveTowards(
                cam.orthographicSize,
                targetScale,
                transitionSpeed * Time.deltaTime
            );

            yield return null;
        }
    }
    
    private IEnumerator DezoomCooldown()
    {
        _isCooldown = true;

        // Wait for the cooldown duration
        yield return new WaitForSeconds(cooldownDuration);

        _isCooldown = false;
    }

}
