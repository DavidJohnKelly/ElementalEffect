using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
/// <summary>
/// Controls the fade-in, display duration, and fade-out behavior of a TextMeshPro text object.
/// </summary>
public class TextFade : MonoBehaviour
{
    /// <summary>
    /// The total duration in seconds that the text object should be visible.
    /// </summary>
    [Header("Text Settings")]
    [SerializeField] private float textDuration = 1f;
    /// <summary>
    /// The duration in seconds for the text to fade in. Set to 0 for no fade-in.
    /// </summary>
    [SerializeField] private float fadeInTime = 0.2f;
    /// <summary>
    /// The duration in seconds for the text to fade out. Set to 0 for no fade-out.
    /// </summary>
    [SerializeField] private float fadeOutTime = 0.5f;

    // The current time elapsed since the text object was created.
    private float currentTime = 0;
    // The TextMeshPro component attached to this GameObject.
    private TMP_Text textObject;
    // The initial color of the text.
    private Color textColour;

    /// <summary>
    /// Initializes the text fade by getting the TMP_Text component and setting the initial alpha to 0 if fade-in is enabled.
    /// </summary>
    public void Start()
    {
        textObject = GetComponent<TMP_Text>();
        textColour = textObject.color;

        if (fadeInTime > 0)
        {
            Color initialColour = textColour;
            initialColour.a = 0;
            textObject.color = initialColour;
        }
    }

    /// <summary>
    /// Updates the alpha of the text based on the current time and the specified fade-in and fade-out times.
    /// Destroys the GameObject when the text duration has elapsed.
    /// </summary>
    private void Update()
    {
        currentTime += Time.deltaTime;

        if (textObject != null)
        {
            float alpha = 1f;

            if (currentTime < fadeInTime && fadeInTime > 0)
            {
                alpha = Mathf.Clamp01(currentTime / fadeInTime);
            }
            else if (currentTime > textDuration - fadeOutTime && fadeOutTime > 0)
            {
                alpha = Mathf.Clamp01((textDuration - currentTime) / fadeOutTime);
            }

            Color newColour = textColour;
            newColour.a = alpha * textColour.a;
            textObject.color = newColour;
        }

        if (currentTime >= textDuration)
        {
            Destroy(gameObject);
        }
    }
}