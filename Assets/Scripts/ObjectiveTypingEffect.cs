using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectiveTypingEffect : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI objectiveText;
    public GameObject objectivePanel;

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f; // Time between each character
    public float lineDelay = 0.3f; // Delay between lines
    public float startDelay = 0.5f; // Delay before typing starts
    public float displayDuration = 4f; // How long to show after typing completes

    [Header("Sound (Optional)")]
    public AudioSource typingAudioSource;
    public AudioClip typingSound;

    [Header("Terminal Style")]
    public bool showCursor = true;
    public string cursorCharacter = "_";
    public float cursorBlinkSpeed = 0.5f;
    public Color textColor = new Color(0f, 1f, 0.4f, 1f); // Terminal green

    private string[] level1Objectives = new string[]
    {
        "> MISSION BRIEFING",
        "",
        "1- Destroy the rogue AI server in Neuroline Control Nexus",
        "2- Eliminate 2 enemies",
        "",
        "> GOOD LUCK, SOLDIER"
    };

    private string[] level2Objectives = new string[]
    {
        "> MISSION BRIEFING",
        "",
        "1- Eliminate 7 hostiles in Titansteel Foundry",
        "",
        "> GOOD LUCK, SOLDIER"
    };

    private Coroutine typingCoroutine;
    private Coroutine cursorCoroutine;
    private bool isTyping = false;

    void Start()
    {
        if (objectiveText != null)
        {
            objectiveText.color = textColor;
        }

        StartObjectiveDisplay();
    }

    public void StartObjectiveDisplay()
    {
        // Determine which level we're in
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string[] objectives;

        if (sceneName.Contains("Level1") || sceneName.Contains("level1"))
        {
            objectives = level1Objectives;
        }
        else if (sceneName.Contains("Level2") || sceneName.Contains("level2"))
        {
            objectives = level2Objectives;
        }
        else
        {
            // Default to level 1 objectives or custom handling
            objectives = level1Objectives;
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeObjectives(objectives));
    }

    private IEnumerator TypeObjectives(string[] objectives)
    {
        isTyping = true;

        // Show the panel
        if (objectivePanel != null)
        {
            objectivePanel.SetActive(true);
        }

        // Clear the text
        if (objectiveText != null)
        {
            objectiveText.text = "";
        }

        // Wait before starting
        yield return new WaitForSeconds(startDelay);

        // Start cursor blinking
        if (showCursor)
        {
            cursorCoroutine = StartCoroutine(BlinkCursor());
        }

        string fullText = "";

        // Type each line
        foreach (string line in objectives)
        {
            // Type each character in the line
            foreach (char c in line)
            {
                fullText += c;
                
                if (objectiveText != null)
                {
                    objectiveText.text = fullText + (showCursor ? cursorCharacter : "");
                }

                // Play typing sound
                if (typingAudioSource != null && typingSound != null && c != ' ')
                {
                    typingAudioSource.PlayOneShot(typingSound, 0.3f);
                }

                yield return new WaitForSeconds(typingSpeed);
            }

            // Add newline after each objective line
            fullText += "\n";
            
            if (objectiveText != null)
            {
                objectiveText.text = fullText + (showCursor ? cursorCharacter : "");
            }

            // Delay between lines
            yield return new WaitForSeconds(lineDelay);
        }

        isTyping = false;

        // Stop cursor and show final text
        if (cursorCoroutine != null)
        {
            StopCoroutine(cursorCoroutine);
        }

        if (objectiveText != null)
        {
            objectiveText.text = fullText; // Remove cursor
        }

        // Wait before hiding
        yield return new WaitForSeconds(displayDuration);

        // Fade out the panel
        yield return StartCoroutine(FadeOutPanel());
    }

    private IEnumerator BlinkCursor()
    {
        bool showCursorChar = true;

        while (isTyping)
        {
            yield return new WaitForSeconds(cursorBlinkSpeed);

            if (!isTyping) break;

            showCursorChar = !showCursorChar;

            // Only update cursor visibility, don't touch the typed text
            // This is handled in the typing coroutine
        }
    }

    private IEnumerator FadeOutPanel()
    {
        if (objectivePanel == null) yield break;

        CanvasGroup canvasGroup = objectivePanel.GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = objectivePanel.AddComponent<CanvasGroup>();
        }

        float fadeTime = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = 1f - (elapsedTime / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        objectivePanel.SetActive(false);
        canvasGroup.alpha = 1f; // Reset for next time
    }

    // Call this to skip the typing animation
    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (cursorCoroutine != null)
        {
            StopCoroutine(cursorCoroutine);
        }

        isTyping = false;

        // Show full text immediately
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string[] objectives;

        if (sceneName.Contains("Level1") || sceneName.Contains("level1"))
        {
            objectives = level1Objectives;
        }
        else if (sceneName.Contains("Level2") || sceneName.Contains("level2"))
        {
            objectives = level2Objectives;
        }
        else
        {
            objectives = level1Objectives;
        }

        if (objectiveText != null)
        {
            objectiveText.text = string.Join("\n", objectives);
        }

        // Start fade out after a short delay
        StartCoroutine(DelayedFadeOut());
    }

    private IEnumerator DelayedFadeOut()
    {
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeOutPanel());
    }

    void Update()
    {
        // Allow player to skip by pressing Space or Enter
        if (isTyping && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            SkipTyping();
        }
    }
}
