using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterData[] characters; // Array of all characters
    private int currentIndex = 0;

    [Header("Spawn")]
    public Transform spawnPoint;
    private GameObject currentCharacterInstance;

    [Header("UI References")]
    public TextMeshProUGUI characterNameText;

    public Image attackBar;
    public Image defenceBar; 
    public Image mobilityBar; 

    [Header("Buttons")]
    public Button leftButton;
    public Button rightButton;
    public Button selectButton;

    [Header("Animation Settings")]
    public bool animateBars = true;
    public float barAnimationSpeed = 0.3f;

    void Start()
    {
        leftButton.onClick.AddListener(PreviousCharacter);
        rightButton.onClick.AddListener(NextCharacter);
        selectButton.onClick.AddListener(SelectCharacter);

        DisplayCharacter(currentIndex);
    }

    public void NextCharacter()
    {
        Debug.Log("RIGHT BUTTON CLICKED!");
        currentIndex++;
        if (currentIndex >= characters.Length)
            currentIndex = 0; // Loop back to first

        DisplayCharacter(currentIndex);
    }

    public void PreviousCharacter()
    {
        Debug.Log("LEFT BUTTON CLICKED!");
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = characters.Length - 1; // Loop to last

        DisplayCharacter(currentIndex);
    }

    void DisplayCharacter(int index)
    {
        // Destroy previous character
        if (currentCharacterInstance != null)
            Destroy(currentCharacterInstance);

        // Get character data
        CharacterData character = characters[index];

        // Spawn new character
        if (character.characterPrefab != null)
        {
            currentCharacterInstance = Instantiate(
                character.characterPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            // Optional: Play idle animation
            Animator animator = currentCharacterInstance.GetComponent<Animator>();
            if (animator != null)
            {
                // Some animators use "Idle" trigger, some auto-play idle
                // Try triggering it, but don't worry if it doesn't exist
                if (animator.parameters.Length > 0)
                {
                    foreach (var param in animator.parameters)
                    {
                        if (param.name == "Idle" && param.type == AnimatorControllerParameterType.Trigger)
                        {
                            animator.SetTrigger("Idle");
                            break;
                        }
                    }
                }
            }
        }

        // Update UI
        characterNameText.text = character.characterName;

        // Update stat bars based on your 3-bar design
        if (animateBars)
        {
            // Smooth animation
            if (attackBar != null)
                StartCoroutine(AnimateBar(attackBar, character.attack / 100f));
            if (defenceBar != null)
                StartCoroutine(AnimateBar(defenceBar, character.defence / 100f));
            if (mobilityBar != null)
                StartCoroutine(AnimateBar(mobilityBar, character.mobility / 100f));
        }
        else
        {
            // Instant update
            if (attackBar != null)
                attackBar.fillAmount = character.attack / 100f;
            if (defenceBar != null)
                defenceBar.fillAmount = character.defence / 100f;
            if (mobilityBar != null)
                mobilityBar.fillAmount = character.mobility / 100f;
        }
    }

    IEnumerator AnimateBar(Image bar, float targetFill)
    {
        float startFill = bar.fillAmount;
        float elapsed = 0f;

        while (elapsed < barAnimationSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / barAnimationSpeed;
            bar.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            yield return null;
        }

        bar.fillAmount = targetFill;
    }

    public void SelectCharacter()
    {
        // Save selected character index and data
        PlayerPrefs.SetInt("SelectedCharacter", currentIndex);
        PlayerPrefs.SetString("SelectedCharacterName", characters[currentIndex].characterName);

        // Save individual stats for gameplay use later
        PlayerPrefs.SetFloat("SelectedCharacterAttack", characters[currentIndex].attack);
        PlayerPrefs.SetFloat("SelectedCharacterDefence", characters[currentIndex].defence);
        PlayerPrefs.SetFloat("SelectedCharacterMobility", characters[currentIndex].mobility);

        PlayerPrefs.Save();

        Debug.Log("Selected: " + characters[currentIndex].characterName);

        // Load next scene (e.g., Level1 or WeaponSelection)
        SceneManager.LoadScene("MainLobby");
    }
}