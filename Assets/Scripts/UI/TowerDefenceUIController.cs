using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the user interface elements for the tower defense game, handling tower selection, resource display, and next round information.
/// </summary>
public class TowerDefenceUIController : MonoBehaviour
{
    /// <summary>
    /// Reference to the ObjectPlacer script, used to set the tower to be placed.
    /// </summary>
    [Header("Object Placer Data")]
    [SerializeField] private ObjectPlacer objectPlacer;

    /// <summary>
    /// List of dropdown menus used to select the element and type of tower to place.
    /// </summary>
    [Header("Element Dropdown Data")]
    [SerializeField] private List<TMP_Dropdown> elementDropdowns;
    /// <summary>
    /// The index of the dropdown option that serves as the header or default selection for the element dropdowns.
    /// </summary>
    [SerializeField] private int elementHeaderIndex = 0;

    /// <summary>
    /// Dropdown menu to display information about the next enemy wave.
    /// </summary>
    [Header("Next Round Data")]
    [SerializeField] private TMP_Dropdown nextRoundDropdown;
    /// <summary>
    /// The text to display as the default option in the next round dropdown.
    /// </summary>
    [SerializeField] private string nextRoundText;

    /// <summary>
    /// Button to switch the game state to the first-person shooter mode.
    /// </summary>
    [Header("Other UI Data")]
    [SerializeField] private Button playButton;
    /// <summary>
    /// Text element to display the current amount of player resources.
    /// </summary>
    [SerializeField] private TMP_Text resourcesText;

    // Reference to the GameController instance.
    private GameController gameController;
    // Dictionary to store the header text for each elemental dropdown.
    private readonly Dictionary<TMP_Dropdown, string> dropdownHeaders = new();

    /// <summary>
    /// Gets the GameController instance and initializes the elemental dropdown menus on awake.
    /// </summary>
    private void Awake()
    {
        gameController = GameController.Instance;
        InitialiseElementalDropdowns();
    }

    /// <summary>
    /// Adds a listener to the play button to trigger the switch to the FPS game mode on start.
    /// </summary>
    private void Start()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayButtonClicked);
        }
        else
        {
            Debug.LogError("Button not found in the Canvas!");
        }
    }

    /// <summary>
    /// Updates the displayed resource amount and initializes the next round dropdown when the UI is enabled.
    /// </summary>
    private void OnEnable()
    {
        UpdateResourceAmount(gameController.CurrentMoney);

        List<(EnemyType, int)> nextRoundData = gameController.GetCurrentEnemyList();
        InitialiseNextRoundDropdown(nextRoundData);
    }

    /// <summary>
    /// Initializes the dropdown menu that displays information about the next enemy wave.
    /// </summary>
    /// <param name="nextRoundData">A list of tuples containing the enemy type and count for the next round.</param>
    private void InitialiseNextRoundDropdown(List<(EnemyType, int)> nextRoundData)
    {
        if (nextRoundDropdown != null)
        {
            nextRoundDropdown.onValueChanged.RemoveAllListeners();
            nextRoundDropdown.onValueChanged.AddListener((index) =>
            {
                nextRoundDropdown.Hide();
                nextRoundDropdown.captionText.text = nextRoundText;
                nextRoundDropdown.value = 0;
            });

            nextRoundDropdown.ClearOptions();

            List<string> enemyOptions = new()
            {
                nextRoundText
            };
            foreach (var (enemyType, count) in nextRoundData)
            {
                string enemyTypeString = EnemyData.GetTypeString(enemyType);
                enemyOptions.Add(enemyTypeString + " x " + count);
            }

            nextRoundDropdown.AddOptions(enemyOptions);
            nextRoundDropdown.RefreshShownValue();
        }
    }

    /// <summary>
    /// Initializes the dropdown menus for selecting the elemental type of the tower to place.
    /// </summary>
    private void InitialiseElementalDropdowns()
    {
        foreach (TMP_Dropdown dropdown in elementDropdowns)
        {
            string headerText = dropdown.options[elementHeaderIndex].text;
            dropdownHeaders[dropdown] = headerText;
            dropdown.captionText.text = headerText;

            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener((index) => OnElementalDropdownValueChanged(dropdown, index));

            TowerElement element = TowerData.GetTowerElement(dropdown.gameObject.name);
            SetDropdownColour(dropdown, element);

            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (i == elementHeaderIndex) continue;

                TowerType type = TowerData.GetTowerType(i);
                TowerConfig config = TowerFactory.Instance.GetTowerConfig(type, element, TowerLevel.One);
                if (config != null)
                {
                    dropdown.options[i].text = TowerData.GetTypeString(type) + " - " + config.Cost;
                }
            }

            dropdown.RefreshShownValue();
        }
    }

    /// <summary>
    /// Checks if a given color is considered dark based on its perceived brightness.
    /// </summary>
    /// <param name="colour">The color to check.</param>
    /// <returns>True if the color is dark, false otherwise.</returns>
    private bool IsDarkColour(Color colour)
    {
        // Values from approximate human perception
        float brightness = colour.r * 0.299f + colour.g * 0.587f + colour.b * 0.114f;
        return brightness < 0.5f;
    }

    /// <summary>
    /// Sets the background and text color of a dropdown menu based on the selected tower element.
    /// </summary>
    /// <param name="dropdown">The dropdown menu to modify.</param>
    /// <param name="element">The tower element associated with the dropdown.</param>
    private void SetDropdownColour(TMP_Dropdown dropdown, TowerElement element)
    {
        Color colour = ColourFactory.Instance.GetColour(element);
        Color textColor = IsDarkColour(colour) ? Color.white : Color.black;

        Image backgroundImage = dropdown.template.GetComponentInChildren<Image>();
        if (backgroundImage != null)
        {
            backgroundImage.color = colour;
        }
        foreach (Transform option in dropdown.template.GetComponentsInChildren<Transform>())
        {
            Image optionBackgroundImage = option.GetComponent<Image>();
            if (optionBackgroundImage != null && option.gameObject.name == "Item Background")
            {
                optionBackgroundImage.color = colour;
            }

            TMP_Text optionText = option.GetComponentInChildren<TMP_Text>();
            if (optionText != null)
            {
                optionText.color = textColor;
            }
        }

        if (dropdown.targetGraphic != null && dropdown.targetGraphic is Image targetImage)
        {
            targetImage.color = colour;
        }
        if (dropdown.captionText != null)
        {
            dropdown.captionText.color = textColor;
        }
    }

    /// <summary>
    /// Handles the event when the value of an elemental dropdown menu is changed.
    /// Sets the tower to be placed based on the selected element and type.
    /// </summary>
    /// <param name="dropdown">The dropdown menu that triggered the event.</param>
    /// <param name="selectedIndex">The index of the selected option in the dropdown.</param>
    private void OnElementalDropdownValueChanged(TMP_Dropdown dropdown, int selectedIndex)
    {
        if (selectedIndex == elementHeaderIndex) return;

        dropdown.onValueChanged.RemoveAllListeners();

        TowerElement element = TowerData.GetTowerElement(dropdown.gameObject.name);
        TowerType type = TowerData.GetTowerType(selectedIndex);
        SetTowerInPlacer(type, element);

        dropdown.value = elementHeaderIndex;
        dropdown.captionText.text = dropdownHeaders[dropdown];

        dropdown.onValueChanged.AddListener((index) => OnElementalDropdownValueChanged(dropdown, index));
    }

    /// <summary>
    /// Sets the tower to be placed by the ObjectPlacer script.
    /// </summary>
    /// <param name="type">The type of the tower to place.</param>
    /// <param name="element">The element of the tower to place.</param>
    private void SetTowerInPlacer(TowerType type, TowerElement element)
    {
        PlaceableObject tower = TowerFactory.Instance.GetTower(type, element, TowerLevel.One);
        Debug.Log("Tower: " + tower.name);
        objectPlacer.ObjectToPlace = tower;
    }

    /// <summary>
    /// Called when the play button is clicked, switches the game state to the first-person shooter mode.
    /// </summary>
    private void PlayButtonClicked()
    {
        gameController.SwitchToFPS();
    }

    /// <summary>
    /// Updates the text displaying the current amount of player resources.
    /// </summary>
    /// <param name="newResources">The new amount of resources to display.</param>
    public void UpdateResourceAmount(float newResources)
    {
        resourcesText.text = "Resources: " + newResources;
    }
}