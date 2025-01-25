using CharTween;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class CharacterController : MonoBehaviour
{
    public char? CorrectCharacter; // The correct character to display
    public bool isCorrect; // Whether the character is already correct
    private TextMeshPro _textComponent;
    private TextManager _textManager;
    private bool _active;

    private void Start()
    {
        _textComponent = GetComponent<TextMeshPro>();
        _textManager = FindObjectOfType<TextManager>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log($"Trigger triggered for character {_textComponent.text}. Correct?: {isCorrect}");
        if (isCorrect) return; // Do nothing if the character is already correct

        _active = true;
        ApplyTweens();
        
        // Check if the collider belongs to the player
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Space))
        {
            // Replace the incorrect character with the correct one
            CorrectCharacter ??= '\0'; //TODO: add an animation when it disappers
            _textComponent.text = CorrectCharacter.ToString();
            _textComponent.color = Color.white; // Change color to indicate it's correct
            _textComponent.fontStyle = FontStyles.Normal;

            // Mark as correct
            isCorrect = true;
            
            _textManager.UpdateCharacterPositions();
            _textManager.CorrectError();
            ResetTweens();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!_active) return;
        ResetTweens();
        _active = false;
    }

    private void ApplyTweens()
    {
        CharTweener tweener = _textComponent.GetCharTweener();
        Tween tween = tweener.DOScale(0, 1.2f, 0.5f);
    }

    private void ResetTweens()
    {
        CharTweener tweener = _textComponent.GetCharTweener();
        Tween tween = tweener.DOScale(0, 1.0f, 0.5f);
    }
}