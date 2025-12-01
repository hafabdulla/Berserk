using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainLobbyController : MonoBehaviour
{
    [Header("Player Info")]
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI rankText;
    public Image playerAvatar;

    [Header("Currency")]
    public TextMeshProUGUI goldAmountText;

    [Header("Character Display")]
    public GameObject[] allCharacterPrefabs; // Array of ALL character models
    private GameObject currentCharacterInstance;


    [Header("Buttons")]
    public Button levelsButton;
    public Button recruitButton;
    public Button inventoryButton;
    public Button storeButton;
    public Button startMissionButton;
    public Button quitButton;

    [Header("Warning Panel")]
    public GameObject warningPanel; // Panel that shows warnings
    public TextMeshProUGUI warningMessageText; // Text inside warning panel
    public Button warningOkButton;

    private bool hasSelectedCharacter = false;
    private bool hasSelectedGun = false;

    void Start()
    {
        // Check selections
        CheckSelections();

        // Setup button listeners
        levelsButton.onClick.AddListener(OnLevelsClicked);
        recruitButton.onClick.AddListener(OnRecruitClicked);
        inventoryButton.onClick.AddListener(OnInventoryClicked);
        storeButton.onClick.AddListener(OnStoreClicked);
        startMissionButton.onClick.AddListener(OnStartMissionClicked);


        if (warningOkButton != null)
            warningOkButton.onClick.AddListener(OnWarningOkClicked);

        // Load player data
        LoadPlayerInfo();
        LoadCharacterDisplay();
    }

    void CheckSelections()
    {
        hasSelectedCharacter = PlayerPrefs.HasKey("SelectedCharacter");
        hasSelectedGun = PlayerPrefs.HasKey("SelectedGun");

        Debug.Log($"Character selected: {hasSelectedCharacter}, Gun selected: {hasSelectedGun}");

        // Enable/disable Start Mission button based on selections
        if (startMissionButton != null)
        {
            bool canStart = hasSelectedCharacter && hasSelectedGun;
            startMissionButton.interactable = canStart;

            if (!canStart)
            {
                // Visual feedback that button is disabled
                ColorBlock colors = startMissionButton.colors;
                colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                startMissionButton.colors = colors;
            }
        }

        // Hide warning panel initially
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
    }

    void LoadCharacterDisplay()
    {
        // Destroy previous character if exists
        if (currentCharacterInstance != null)
        {
            Destroy(currentCharacterInstance);
        }

        GameObject characterToSpawn = null;

        if (hasSelectedCharacter)
        {
            // Load the selected character
            int selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

            if (selectedIndex >= 0 && selectedIndex < allCharacterPrefabs.Length)
            {
                characterToSpawn = allCharacterPrefabs[selectedIndex];
                Debug.Log("Loading selected character: " + selectedIndex);
            }
            else
            {
                Debug.LogWarning("Invalid character index, using default");
                characterToSpawn = allCharacterPrefabs[0];
            }
        }
        else
        {
            // No character selected - show default (first character)
            if (allCharacterPrefabs.Length > 0)
            {
                characterToSpawn = allCharacterPrefabs[0];
                Debug.Log("No character selected, displaying default character");
            }
        }

        // Spawn the character at specific position and scale
        if (characterToSpawn != null)
        {
            currentCharacterInstance = Instantiate(
                characterToSpawn,
                new Vector3(0, 0, -7),  // Position
                Quaternion.identity      // Rotation (facing forward)
            );

            // Set scale
            currentCharacterInstance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            // Optional: Add slow rotation for better viewing
            CharacterRotator rotator = currentCharacterInstance.AddComponent<CharacterRotator>();
            rotator.rotationSpeed = 20f;

            // Play idle animation if available
            Animator animator = currentCharacterInstance.GetComponent<Animator>();
            if (animator != null)
            {
                Debug.Log("Character animator found");
            }

            Debug.Log($"Character spawned at (0, 0, -7) with scale (1.5, 1.5, 1.5)");
        }
        else
        {
            Debug.LogError("No character prefab to spawn!");
        }
    }



    void LoadPlayerInfo()
    {
        // Load player info from PlayerPrefs
        string playerName = PlayerPrefs.GetString("PlayerName", "TANGO SsS00024");
        int playerRank = PlayerPrefs.GetInt("PlayerRank", 15);
        int goldAmount = PlayerPrefs.GetInt("GoldAmount", 152);

        // Update UI
        if (usernameText != null)
            usernameText.text = playerName;

        if (rankText != null)
            rankText.text = "Rank: " + playerRank;

        if (goldAmountText != null)
            goldAmountText.text = goldAmount.ToString();

    }

    void OnLevelsClicked()
    {
        Debug.Log("Levels clicked");
        // TODO: Open level selection screen
        // SceneManager.LoadScene("LevelSelection");

        // For now, show message
        Debug.Log("Level selection not implemented yet");
    }

    void OnRecruitClicked()
    {
        Debug.Log("Recruit clicked - Opening Character Selection");
        SceneManager.LoadScene("CharacterSelection");
    }

    void OnInventoryClicked()
    {
        Debug.Log("Inventory clicked");
        // TODO: Open inventory screen
        SceneManager.LoadScene("GunSelection");

    }

    void OnStoreClicked()
    {
        Debug.Log("Store/Gun Selection clicked");
        SceneManager.LoadScene("GunSelection");
    }

    void OnStartMissionClicked()
    {
        Debug.Log("Start Mission clicked");

        // Check if both character and gun are selected
        if (!hasSelectedCharacter && !hasSelectedGun)
        {
            ShowWarning("⚠️ Selection Required!\n\nPlease select a CHARACTER and a WEAPON before starting a mission.");
            return;
        }
        else if (!hasSelectedCharacter)
        {
            ShowWarning("⚠️ No Character Selected!\n\nPlease select a character before starting a mission.");
            return;
        }
        else if (!hasSelectedGun)
        {
            ShowWarning("⚠️ No Weapon Selected!\n\nPlease select a weapon before starting a mission.");
            return;
        }

        // All checks passed - start the game
        Debug.Log("Starting mission...");
        SceneManager.LoadScene("Level1");
    }

    void OnSettingsClicked()
    {
        Debug.Log("Settings clicked");
        // TODO: Open settings menu
        Debug.Log("Settings not implemented yet");
    }

    

    void ShowWarning(string message)
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(true);

            if (warningMessageText != null)
            {
                warningMessageText.text = message;
            }
        }
        else
        {
            // Fallback if no warning panel - just log and redirect
            Debug.LogWarning(message);

            // Automatically redirect to appropriate selection screen
            if (!hasSelectedCharacter)
            {
                SceneManager.LoadScene("CharacterSelection");
            }
            else if (!hasSelectedGun)
            {
                SceneManager.LoadScene("GunSelection");
            }
        }
    }

    void OnWarningOkClicked()
    {
        // Hide warning panel
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }

        // Redirect user to appropriate selection screen
        if (!hasSelectedCharacter)
        {
            Debug.Log("Redirecting to Character Selection");
            SceneManager.LoadScene("CharacterSelection");
        }
        else if (!hasSelectedGun)
        {
            Debug.Log("Redirecting to Gun Selection");
            SceneManager.LoadScene("GunSelection");
        }
    }
}