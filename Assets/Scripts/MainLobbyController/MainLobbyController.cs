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
    public Transform characterSpawnPoint;
    public GameObject[] allCharacterPrefabs; // Array of ALL character models
    private GameObject currentCharacterInstance;

    [Header("Buttons")]
    public Button levelsButton;
    public Button recruitButton;
    public Button inventoryButton;
    public Button storeButton;
    public Button startMissionButton;

    [Header("Warning Panel")]
    public GameObject characterWarningPanel; // Panel that says "Select a character first!"
    public Button warningOkButton;

    private bool hasSelectedCharacter = false;

    void Start()
    {
        // Check if character is selected
        CheckCharacterSelection();

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

    void CheckCharacterSelection()
    {
        hasSelectedCharacter = PlayerPrefs.HasKey("SelectedCharacter");

        if (!hasSelectedCharacter)
        {
            Debug.Log("No character selected - showing first-time message");

            // Option A: Immediately go to character selection
            // SceneManager.LoadScene("CharacterSelection");

            // Option B: Show warning and disable Start Mission button
            if (startMissionButton != null)
            {
                startMissionButton.interactable = false;
                // You can change button color to show it's disabled
                ColorBlock colors = startMissionButton.colors;
                colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                startMissionButton.colors = colors;
            }

            if (characterWarningPanel != null)
            {
                // Show warning panel briefly or keep it until user selects
                // characterWarningPanel.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Character already selected");

            if (startMissionButton != null)
            {
                startMissionButton.interactable = true;
            }

            if (characterWarningPanel != null)
            {
                characterWarningPanel.SetActive(false);
            }
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

        // Spawn the character
        if (characterToSpawn != null && characterSpawnPoint != null)
        {
            // Specific position and scale for lobby display
            Vector3 spawnPosition = new Vector3(0, 0, -7);
            Vector3 spawnScale = new Vector3(1.5f, 1.5f, 1.5f);

            currentCharacterInstance = Instantiate(
                characterToSpawn,
                spawnPosition,
                Quaternion.identity // Default rotation (facing forward)
            );

            // Apply scale
            currentCharacterInstance.transform.localScale = spawnScale;

            // Optional: Add slow rotation to character for better viewing
            CharacterRotator rotator = currentCharacterInstance.AddComponent<CharacterRotator>();
            rotator.rotationSpeed = 20f; // Slow rotation

            // Play idle animation
            Animator animator = currentCharacterInstance.GetComponent<Animator>();
            if (animator != null)
            {
                // Let default idle animation play
                Debug.Log("Character animator found, playing idle");
            }
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
        int coinAmount = PlayerPrefs.GetInt("CoinAmount", 0);

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
    }

    void OnStoreClicked()
    {
        Debug.Log("Store clicked");
        // TODO: Open store/gun selection
    }

    void OnStartMissionClicked()
    {
        Debug.Log("Start Mission clicked");

        // Double check character selection
        if (!PlayerPrefs.HasKey("SelectedCharacter"))
        {
            Debug.LogWarning("Cannot start mission - No character selected!");
            ShowCharacterWarning();
            return;
        }

        // All checks passed - start the game
        SceneManager.LoadScene("Level1");
    }

    void ShowCharacterWarning()
    {
        if (characterWarningPanel != null)
        {
            characterWarningPanel.SetActive(true);
        }
        else
        {
            // Fallback - just go to character selection
            Debug.Log("Please select a character first!");
            SceneManager.LoadScene("CharacterSelection");
        }
    }

    void OnWarningOkClicked()
    {
        // User acknowledged warning - take them to character selection
        SceneManager.LoadScene("CharacterSelection");
    }
}