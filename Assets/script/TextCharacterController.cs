using System.Collections.Generic;
using CharTween;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class CharacterController : MonoBehaviour
{
    public char? CorrectCharacter; // The correct character to display
    private char _replaceWith; // The correct character to display
    public bool isCorrect; // Whether the character is already correct
    private TextMeshPro _textComponent;
    private TextManager _textManager;
    private bool _isPlayerInTrigger;
    private List<KeyCode> _alphaKeys;

    private void Start()
    {
        _textComponent = GetComponent<TextMeshPro>();
        _textManager = FindObjectOfType<TextManager>();
        
        _alphaKeys = new List<KeyCode>() { 
            KeyCode.A, KeyCode.Z, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
            KeyCode.Q, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M,
            KeyCode.W, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N,
            KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, 
            KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9
        };
        
        
        CorrectCharacter ??= '\0'; //TODO: add an animation when it disappers

    }
    
    private void Update()
    {
        if (_isPlayerInTrigger && !isCorrect)
        {
            HandleInput();
        }
    }
    
    private void HandleInput()
    {
        char newChar = '\0';
        bool alphaKeyPressed = false;

        foreach (var key in _alphaKeys)
        {
            if (Input.GetKeyDown(key))
            {
                newChar = (char)((int)key);
                alphaKeyPressed = true;
                break;
            }
        }
        
        if ((Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)) && CorrectCharacter == '\0')
            _textComponent.text = '\0'.ToString();
        else if (alphaKeyPressed)
        {
            if (newChar == CorrectCharacter)
            {
                _textComponent.text = newChar.ToString();
            }
            else
            {
                Debug.Log($"Incorrect character selected, should be '{CorrectCharacter}'");
                return;
            }
        }
        else return;
        
        isCorrect = true;
        _textComponent.color = _textManager.normalColor;
        _textComponent.fontStyle = FontStyles.Normal;

        _textManager.CorrectError();
        _textManager.UpdateCharacterPositions();
        ResetTweens();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCorrect)
        {
            ApplyTweens();
            _isPlayerInTrigger = true;
        }
    }
            

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInTrigger = false;
            ResetTweens();
        }
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