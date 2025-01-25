using System;
using System.Collections.Generic;
using CharTween;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public string initialText = "Moi, votre nouveau présigent, je vous projet l'égalité et la libelté dans une nation uni et solidaires ou l'indiwidu seul n'aura pas sa plase!";
    public string correctText = "Moi, votre nouveau président, je vous projet l'égalité et la liberté dans une nation unie et solidaires ou l'individu seul n'aura pas sa place!";
    private string _currentText;
    public GameObject characterPrefab; // Prefab with a TextMeshPro component
    public float characterSpacingFactor = 0.1f; // Extra space between characters
    public float maxWidth = 100.0f; // Nb of units before line break
    public float lineHeight = 20.0f; // Nb of units before line break
    
    public Color normalColor = Color.black;
    public Color errorColor = Color.red;
    
    private int _totalErrors = 0;
    private int _errorsLeft = 0;
    private int _errorsCorrected = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateText();
        _currentText = initialText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GenerateText()
    {
        float currentXOffset = 0;
        float currentYOffset = 0;
        float spaceWidth = 0.0f; // Width of a space character
        
        StringComparer comparer = new StringComparer();
        var comparison = comparer.CompareStrings(initialText, correctText);

        List<(List<(int position, StringComparer.ComparisonResult result, char? expected, char? actual)> characters, bool isSpace)> words = new();
        List<(int position, StringComparer.ComparisonResult result, char? expected, char? actual)> currentWord = new();
        
        // Group characters into words or spaces
        foreach (var result in comparison)
        {
            if (result.actual == ' ' || result.expected == ' ')
            {
                if (currentWord.Count > 0)
                {
                    words.Add((currentWord, false)); // Add the current word
                    currentWord = new(); // Reset the current word
                }

                // Add the space as its own entity
                words.Add((new List<(int, StringComparer.ComparisonResult, char?, char?)> { result }, true));
            }
            else
            {
                currentWord.Add(result); // Add character to the current word
            }
        }

        if (currentWord.Count > 0)
        {
            words.Add((currentWord, false)); // Add the final word
        }

        foreach (var word in words)
        {
            float wordWidth = 0;
            List<(GameObject charObject, float charWidth)> charObjects = new();

            // Measure the total width of the word or space
            foreach (var character in word.characters)
            {
                GameObject tempObject = Instantiate(characterPrefab, transform);
                TextMeshPro tempText = tempObject.GetComponent<TextMeshPro>();
                tempText.text = character.actual?.ToString() ?? ""; // Use actual character or empty string
                
                if (character.actual == ' ')
                {
                    spaceWidth = tempText.fontSize * 0.025f; // Cache space width
                    wordWidth += spaceWidth;
                }
                else
                {
                    wordWidth += tempText.GetPreferredValues(tempText.text).x;
                }

                Destroy(tempObject); // Clean up temporary object
            }

            // Check if the word fits on the current line
            if (currentXOffset + wordWidth > maxWidth && !word.isSpace)
            {
                currentXOffset = 0; // Reset X offset
                currentYOffset -= lineHeight; // Move to the next line
            }

            // Process each character in the word or space
            foreach (var character in word.characters)
            {
                GameObject charObject = Instantiate(characterPrefab, transform);
                TextMeshPro textComponent = charObject.GetComponent<TextMeshPro>();
                textComponent.sortingOrder = 2;

                // Determine displayed character and color
                if (character.result == StringComparer.ComparisonResult.Correct)
                {
                    textComponent.text = character.expected?.ToString() ?? "";
                    textComponent.color = normalColor; // Correct characters in default color
                }
                else if (character.result == StringComparer.ComparisonResult.Missing) // Missing in `initialText`
                {
                    textComponent.text = "!";
                    textComponent.color = errorColor; // Red exclamation for missing characters
                }
                else if (character.result == StringComparer.ComparisonResult.Extra) // Extra in `initialText`
                {
                    textComponent.text = character.actual?.ToString() ?? "";
                    textComponent.color = errorColor; // Highlight extra characters
                    textComponent.fontStyle = FontStyles.Underline;
                }
                else // Incorrect character
                {
                    textComponent.text = character.actual?.ToString() ?? "";
                    textComponent.color = errorColor; // Highlight incorrect characters
                    textComponent.fontStyle = FontStyles.Bold;
                }

                // Measure character size
                Vector2 charSize = textComponent.GetPreferredValues(textComponent.text);
                float charWidth = (character.actual == ' ') ? spaceWidth : charSize.x;

                // Set position and update offsets
                charObject.transform.localPosition = new Vector3(currentXOffset, currentYOffset, 0);
                currentXOffset += charWidth + characterSpacingFactor;

                // Add to list for possible adjustments
                charObjects.Add((charObject, charWidth));
                
                CharacterController charController = charObject.AddComponent<CharacterController>();
                charController.CorrectCharacter = character.expected; // Set the correct character
                charController.isCorrect = character.result == StringComparer.ComparisonResult.Correct;
                textComponent.ForceMeshUpdate();

                // Add and configure BoxCollider2D
                BoxCollider2D charCollider = charObject.AddComponent<BoxCollider2D>();
                charCollider.isTrigger = true;
                
                

                Bounds textBounds = textComponent.textBounds;
                charCollider.size = new Vector2(textBounds.size.x, textBounds.size.y); // Match width and height
                charCollider.offset = new Vector2(textBounds.center.x, textBounds.center.y); // Center the collider
            }
        }
    }
    
    
    public void UpdateCharacterPositions()
    {
        float currentXOffset = 0;
        float currentYOffset = 0;
        float spaceWidth = 0.0f;

        List<List<Transform>> words = new();
        List<Transform> currentWord = new();

        // Group characters into words or spaces
        foreach (Transform child in transform)
        {
            var textComponent = child.GetComponent<TextMeshPro>();
            if (textComponent == null) continue;

            char character = string.IsNullOrEmpty(textComponent.text) ? '\0' : textComponent.text[0];

            if (character == ' ')
            {
                if (currentWord.Count > 0)
                {
                    words.Add(new List<Transform>(currentWord)); // Add current word
                    currentWord.Clear();
                }

                // Add the space as its own group
                words.Add(new List<Transform> { child });
            }
            else
            {
                currentWord.Add(child);
            }
        }

        if (currentWord.Count > 0)
        {
            words.Add(new List<Transform>(currentWord)); // Add the final word
        }

        // Reposition characters word by word
        foreach (var word in words)
        {
            float wordWidth = 0;
            List<(Transform child, float charWidth)> charObjects = new();

            // Measure total width of the word or space
            foreach (var charTransform in word)
            {
                var textComponent = charTransform.GetComponent<TextMeshPro>();
                char character = string.IsNullOrEmpty(textComponent.text) ? '\0' : textComponent.text[0];

                Vector2 charSize = textComponent.GetPreferredValues(character.ToString());
                float charWidth = character == ' ' ? 
                    (spaceWidth == 0 ? textComponent.fontSize * 0.025f : spaceWidth) : 
                    charSize.x;

                if (character == ' ') spaceWidth = charWidth; // Cache space width
                wordWidth += charWidth + characterSpacingFactor;

                charObjects.Add((charTransform, charWidth));
            }

            // Check if the word fits in the current line
            if (currentXOffset + wordWidth > maxWidth && word.Count > 1) // Avoid breaking a single character
            {
                currentXOffset = 0;
                currentYOffset -= lineHeight;
            }

            // Position each character in the word
            foreach (var (charTransform, charWidth) in charObjects)
            {
                charTransform.localPosition = new Vector3(currentXOffset, currentYOffset, 0);

                currentXOffset += charWidth + characterSpacingFactor;

                // Update collider
                var textComponent = charTransform.GetComponent<TextMeshPro>();
                var charCollider = charTransform.GetComponent<BoxCollider2D>();

                textComponent.ForceMeshUpdate();
                Bounds textBounds = textComponent.textBounds;
                charCollider.size = new Vector2(textBounds.size.x, textBounds.size.y);
                charCollider.offset = new Vector2(textBounds.center.x, textBounds.center.y);
            }
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