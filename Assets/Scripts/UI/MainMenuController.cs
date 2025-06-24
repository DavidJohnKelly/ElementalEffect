using UnityEditor;
using UnityEngine;

/// <summary>
/// Controls the functionality of the main menu, including starting the game, quitting the application, and managing background music.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    /// <summary>
    /// The name of the scene to load when the "Play" button is pressed.
    /// </summary>
    [SerializeField] private string initialSceneName;
    /// <summary>
    /// The audio clip to play as background music in the main menu.
    /// </summary>
    [SerializeField] private AudioClip audioFile;

    // The AudioSource component used to play the background music.
    private AudioSource audioSource;

    /// <summary>
    /// Initializes the main menu by making the cursor visible and unlocked, and starts playing the background music if an AudioSource and audio file are present.
    /// </summary>
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && audioFile != null)
        {
            audioSource.clip = audioFile;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Loads the scene specified in the initialSceneName variable.
    /// </summary>
    public void Play()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(initialSceneName);
    }

    /// <summary>
    /// Quits the application. In the Unity Editor, it exits Play Mode; in a standalone build, it closes the application.
    /// </summary>
    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}