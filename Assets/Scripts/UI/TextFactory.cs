using UnityEngine;
using TMPro;

/// <summary>
/// A factory class responsible for creating dynamic text objects in the game.
/// This class follows the Singleton pattern for easy access.
/// </summary>
public class TextFactory : MonoBehaviour
{
    /// <summary>
    /// The color to use for the outline of the text.
    /// </summary>
    [SerializeField] private Color outlineColour = Color.white;
    /// <summary>
    /// The prefab for the text object to be instantiated. Should contain a TMP_Text component.
    /// </summary>
    public GameObject textPrefab;

    /// <summary>
    /// The singleton instance of the TextFactory.
    /// </summary>
    public static TextFactory Instance;

    /// <summary>
    /// Sets up the Singleton instance of the TextFactory.
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of TextFactory detected.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Creates a new text object with the specified text, color, font size, and position.
    /// </summary>
    /// <param name="text">The string to display in the text object.</param>
    /// <param name="colour">The color of the text.</param>
    /// <param name="fontSize">The font size of the text.</param>
    /// <param name="position">The world position where the text object should be created.</param>
    public void CreateText(string text, Color colour, float fontSize, Vector3 position)
    {
        if (textPrefab == null)
        {
            Debug.LogError("Error in TextFactory! Text prefab is not assigned!");
            return;
        }

        GameObject textObj = Instantiate(textPrefab, position, Quaternion.identity);

        TMP_Text tmpText = textObj.GetComponent<TMP_Text>();
        tmpText.text = text;
        tmpText.fontSize = fontSize;
        tmpText.color = colour;
    }

    /// <summary>
    /// Creates a new text object with the specified number, color, font size, and position.
    /// Optionally rounds the number to an integer before displaying.
    /// </summary>
    /// <param name="number">The number to display in the text object.</param>
    /// <param name="colour">The color of the text.</param>
    /// <param name="fontSize">The font size of the text.</param>
    /// <param name="position">The world position where the text object should be created.</param>
    /// <param name="roundToInt">Whether to round the number to the nearest integer before displaying (default: true).</param>
    public void CreateText(float number, Color colour, float fontSize, Vector3 position, bool roundToInt = true)
    {
        if (textPrefab == null)
        {
            Debug.LogError("Error in TextFactory! Text prefab is not assigned!");
            return;
        }

        string text = roundToInt ? number.ToString("F0") : number.ToString();

        GameObject textObj = Instantiate(textPrefab, position, Quaternion.identity);

        TMP_Text tmpText = textObj.GetComponent<TMP_Text>();
        tmpText.text = text;
        tmpText.fontSize = fontSize;
        tmpText.color = colour;
    }
}