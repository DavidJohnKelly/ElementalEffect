using UnityEngine;
using UnityEngine.UI;

// Credit to: https://www.sharpcoderblog.com/blog/unity-3d-create-main-menu-with-ui-canvas
// helped me with this script and the main menu impleentation

/// <summary>
/// Scales a UI Image component to always fill the screen while maintaining its aspect ratio.
/// This script was inspired by a solution found on sharpcoderblog.com.
/// </summary>
[ExecuteInEditMode]
public class SC_BackgroundScaler : MonoBehaviour
{
    // The Image component that needs to be scaled.
    Image backgroundImage;
    // The RectTransform of the Image component.
    RectTransform rt;
    // The aspect ratio of the background image.
    float ratio;

    /// <summary>
    /// Gets the Image component and its RectTransform, and calculates the aspect ratio of the background sprite.
    /// </summary>
    private void Start()
    {
        backgroundImage = GetComponent<Image>();
        rt = backgroundImage.rectTransform;
        ratio = backgroundImage.sprite.bounds.size.x / backgroundImage.sprite.bounds.size.y;
    }

    /// <summary>
    /// Updates the size of the RectTransform in each frame to ensure the background image fills the screen,
    /// maintaining its aspect ratio. This also works in Edit Mode due to the ExecuteInEditMode attribute.
    /// </summary>
    private void Update()
    {
        if (!rt) return;

        if (Screen.height * ratio >= Screen.width)
        {
            rt.sizeDelta = new Vector2(Screen.height * ratio, Screen.height);
        }
        else
        {
            rt.sizeDelta = new Vector2(Screen.width, Screen.width / ratio);
        }
    }
}