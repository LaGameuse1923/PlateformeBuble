using System;
using System.Collections.Generic;
using CharTween;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public string initialText = "Ths is a tst!";
    public string correctText = "This is a text!";
    public GameObject characterPrefab; // Prefab with a TextMeshPro component
    public float characterSpacingFactor = 0.1f; // Extra space between characters
    public float maxWidth = 100.0f; // Nb of pixels before line break
    public float lineHeight = 20.0f; // Nb of pixels before line break
    
    private int _totalErrors = 0;
    private int _errorsLeft = 0;
    private int _errorsCorrected = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GenerateText()
    {
        float currentXOffset = 0;
        float currentYOffset = 0;
        
        StringComparer comparer = new StringComparer();
        var comparison = comparer.CompareStrings(initialText, correctText);
        
        foreach (var result in comparison)
        {
            if (result.result != StringComparer.ComparisonResult.Correct)
            {
                _errorsLeft++;
                _totalErrors++;
            }
            // Instantiate the character object
            GameObject charObject = Instantiate(characterPrefab, transform);
            TextMeshPro textComponent = charObject.GetComponent<TextMeshPro>();
            

            // Debug.Log($"Position {result.position}: {result.result} (Expected: '{result.expected}', Actual: '{result.actual}')");
            
            // Determine displayed character and color
            if (result.result == StringComparer.ComparisonResult.Correct)
            {
                textComponent.text = result.expected == '\0' ? "" : result.expected.ToString(); // Hide visual for spaces
                textComponent.color = Color.white; // Correct characters in default color
            }
            else if (result.result == StringComparer.ComparisonResult.Missing) // Missing in `initialText`
            {
                textComponent.text = "!";
                textComponent.color = Color.red; // Red exclamation for missing characters
            }
            else if (result.result == StringComparer.ComparisonResult.Extra) // Extra in `initialText`
            {
                textComponent.text = result.actual.ToString();
                textComponent.color = Color.red; // Red exclamation for missing characters
                textComponent.fontStyle = FontStyles.Underline;
            }
            else // Incorrect character
            {
                textComponent.text = result.actual.ToString();
                textComponent.color = Color.red; // Highlight incorrect characters
                textComponent.fontStyle = FontStyles.Bold;
            }

            char finalChar = textComponent.text == "" ? '\0' : textComponent.text[0];
            
            // Measure character size (spaces included)
            Vector2 charSize = textComponent.GetPreferredValues(finalChar.ToString());
            // Estimate space width. White spaces have no size so have to use font size.
            float charWidth = finalChar == ' ' ? textComponent.fontSize * 0.025f : charSize.x; 

            // Set position based on calculated width
            charObject.transform.localPosition = new Vector3(currentXOffset, currentYOffset, 0);
            
            // Update offset for the next character
            currentXOffset += charWidth + characterSpacingFactor;
            
            // Multiline display
            if (currentXOffset + charWidth > maxWidth)
            {
                Debug.Log("Max Width Reached");
                currentXOffset = 0; // Reset X offset
                currentYOffset -= lineHeight; // Move down a line
            }
            
            textComponent.ForceMeshUpdate();
            
            CharacterController charController = charObject.AddComponent<CharacterController>();
            charController.CorrectCharacter = result.expected; // Set the correct character
            charController.isCorrect = result.result == StringComparer.ComparisonResult.Correct;
            
            // Add and configure BoxCollider2D
            BoxCollider2D charCollider = charObject.AddComponent<BoxCollider2D>();
            charCollider.isTrigger = true;

            // Adjust collider size and offset to match the character's size
            Bounds textBounds = textComponent.textBounds;
            charCollider.size = new Vector2(textBounds.size.x, textBounds.size.y); // Match width and height
            charCollider.offset = new Vector2(textBounds.center.x, textBounds.center.y); // Center the collider
        }
    }
    
    public void UpdateCharacterPositions()
    {
        float currentXOffset = 0;
        float currentYOffset = 0;

        foreach (Transform child in transform)
        {
            var textComponent = child.GetComponent<TextMeshPro>();
            if (textComponent == null) continue;

            // Measure the character's width
            char finalChar = textComponent.text == "" ? '\0' : textComponent.text[0];
            Vector2 charSize = textComponent.GetPreferredValues(finalChar.ToString());
            float charWidth = finalChar == ' ' ? textComponent.fontSize * 0.025f : charSize.x;

            // Set the new position
            child.localPosition = new Vector3(currentXOffset, currentYOffset, 0);

            // Update the X offset
            currentXOffset += charWidth + characterSpacingFactor;

            // Check for line breaks
            if (currentXOffset + charWidth > maxWidth)
            {
                currentXOffset = 0;
                currentYOffset -= lineHeight;
            }
            
            
            // Update the colliders to match the new size
            BoxCollider2D charCollider = child.GetComponent<BoxCollider2D>();

            textComponent.ForceMeshUpdate();
            Bounds textBounds = textComponent.textBounds;
            charCollider.size = new Vector2(textBounds.size.x, textBounds.size.y); // Match width and height
            charCollider.offset = new Vector2(textBounds.center.x, textBounds.center.y); // Center the collider
        }
    }

    public void CorrectError()
    {
        _errorsCorrected++;
        _errorsLeft--;
    }
}



class StringComparer
{
    public enum ComparisonResult
    {
        Correct,
        Incorrect,
        Missing,
        Extra
    }

    
    // Uses Levenshtein distance (magic proposed by ChatGPT)
    public List<(int position, ComparisonResult result, char? expected, char? actual)> CompareStrings(String initialText, String correctText)
    {
        List<(int position, ComparisonResult result, char? expected, char? actual)> comparisonResults = new List<(int position, ComparisonResult result, char? expected, char? actual)>();

        int[,] dp = new int[initialText.Length + 1, correctText.Length + 1];

        for (int i = 0; i <= initialText.Length; i++)
        {
            for (int j = 0; j <= correctText.Length; j++)
            {
                if (i == 0)
                    dp[i, j] = j;
                else if (j == 0)
                    dp[i, j] = i;
                else if (initialText[i - 1] == correctText[j - 1])
                    dp[i, j] = dp[i - 1, j - 1];
                else
                    dp[i, j] = 1 + Math.Min(dp[i - 1, j - 1], Math.Min(dp[i - 1, j], dp[i, j - 1]));
            }
        }

        int a = initialText.Length, b = correctText.Length;
        while (a > 0 || b > 0)
        {
            if (a > 0 && b > 0 && initialText[a - 1] == correctText[b - 1])
            {
                comparisonResults.Add((b, ComparisonResult.Correct, correctText[b - 1], initialText[a - 1]));
                a--;
                b--;
            }
            else if (b > 0 && (a == 0 || dp[a, b - 1] < dp[a - 1, b]))
            {
                comparisonResults.Add((b, ComparisonResult.Missing, correctText[b - 1], null));
                b--;
            }
            else if (a > 0 && (b == 0 || dp[a, b - 1] >= dp[a - 1, b]))
            {
                comparisonResults.Add((a, ComparisonResult.Extra, null, initialText[a - 1]));
                a--;
            }
            else
            {
                comparisonResults.Add((b, ComparisonResult.Incorrect, correctText[b - 1], initialText[a - 1]));
                a--;
                b--;
            }
        }

        comparisonResults.Reverse();
        return comparisonResults;
    }

}