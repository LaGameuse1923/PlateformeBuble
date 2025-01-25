using System.Collections;
using System.Collections.Generic;
using CharTween;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class CharacterController : MonoBehaviour
{
    public char? CorrectCharacter; // The correct character to display
    public bool isCorrect; // Whether the character is already correct
    public AudioClip wrongKeySound; // Sound to play when wrong key is pressed
    public float cooldownDuration; // Time in seconds for which the user can't do any input
    
    private AudioSource _audioSource; // Audio source to play sound effects
    private bool _isLocked; // Cooldown lock


    public PlayerFreeMovement playerFreeMovement;
    public Animator animator;
    private char _replaceWith; // The correct character to display
    private TextMeshPro _textComponent; // Text of the character
    private TextManager _textManager; // Text manager with the entire text
    private bool _isPlayerInTrigger; // Whether the player is on the collider of the character
    private List<KeyCode> _alphaKeys; // Allowed alphanumerical keys 

    private void Start()
    {
        animator = GetComponent<Animator>();
        _textComponent = GetComponent<TextMeshPro>();
        _textManager = FindObjectOfType<TextManager>();
        if (!isCorrect)
            _audioSource = gameObject.AddComponent<AudioSource>();
        
        
        _alphaKeys = new List<KeyCode>() { 
            KeyCode.A, KeyCode.Z, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
            KeyCode.Q, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M,
            KeyCode.W, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N,
            KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, 
            KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9
        };
        
        CorrectCharacter ??= '\0';
    }
    
    private void Update()
    {
        // Only listen for input if player is on the collider and the character can be corrected
        if (_isPlayerInTrigger && !isCorrect && !_isLocked)
            HandleInput();
    }
    
    private void HandleInput()
    {
        var pressedCharacter = '\0';
        var alphaKeyPressed = false;

        // Check if an allowed key was pressed and which one
        foreach (var key in _alphaKeys)
        {
            if (!Input.GetKeyDown(key)) continue;
            
            pressedCharacter = (char)((int)key);
            alphaKeyPressed = true;
            break;
        }

        // If remove was pressed, and it's the character should be removed
        if ((Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)) && CorrectCharacter == '\0')
        {
            playerFreeMovement.Effacer();
            isCorrect = true;
            StartDisappearingEffect();
            ResetCollisionTweens();
            _textManager.CorrectError();
            return;
        }
        if (!alphaKeyPressed) return;
        if (pressedCharacter != CorrectCharacter)
        {
            Debug.Log($"Incorrect character selected, should be '{CorrectCharacter}'");
            PlayWrongKeySound();
            StartCoroutine(LockInputForSeconds(cooldownDuration));
            return;
        }
        
        // If correct key was pressed, start the animation and update the text with the correct character 
        StartAppearingEffect();
        isCorrect = true;
        UpdateTextComponent(pressedCharacter);
        _textManager.CorrectError();
        ResetCollisionTweens(); // Resize the character to normal size
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCorrect)
        {
            ApplyCollisionTweens();
            _isPlayerInTrigger = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        _isPlayerInTrigger = false;
        ResetCollisionTweens();
    }

    // Change the text of the character and rearrange the entire text to fit the new character
    private void UpdateTextComponent(char c)
    {
        _textComponent.text = c.ToString();
        _textComponent.color = _textManager.normalColor;
        _textComponent.fontStyle = FontStyles.Normal;
        _textManager.UpdateCharacterPositions();
    }
    
    // Fade to invisible
    private void StartDisappearingEffect()
    {
        var textColor = _textComponent.color;
        DOTween.To(() => textColor.a, x => textColor.a = x, 0, 0.3f)
            .OnUpdate(() => _textComponent.color = textColor)
            .OnComplete(() => UpdateTextComponent('\0')); // Clear text after fade-out
    }
    
    // Make new character invisible and fade to visible
    private void StartAppearingEffect()
    {
        var targetColor = _textManager.normalColor;
        targetColor.a = 0;
        DOTween.To(() => targetColor.a, x => targetColor.a = x, 1, 0.3f)
            .OnUpdate(() => _textComponent.color = targetColor);
    }
    
    // Make character bigger on entry
    private void ApplyCollisionTweens()
    {
        var tweener = _textComponent.GetCharTweener();
        tweener.DOScale(0, 1.2f, 0.5f);
    }

    // Resize character back to normal on exit
    private void ResetCollisionTweens()
    {
        var tweener = _textComponent.GetCharTweener();
        tweener.DOScale(0, 1.0f, 0.5f);
    }
    
    
    
    private void PlayWrongKeySound()
    {
        if (wrongKeySound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(wrongKeySound);
        }
    }

    private IEnumerator LockInputForSeconds(float seconds)
    {
        _isLocked = true; // Lock input
        yield return new WaitForSeconds(seconds); // Wait for the specified time
        _isLocked = false; // Unlock input
    }
}