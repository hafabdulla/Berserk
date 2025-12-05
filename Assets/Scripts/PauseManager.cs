using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private GameObject pausePanel;
    public GameObject objectivesPanel;

    [Header("Buttons")]
    public Button resumeButton;
    public Button objectivesButton;
    public Button quitButton;
    public Button restartButton;

    [Header("Camera Control Script")]
    public MonoBehaviour cameraController;
    // Drag your camera script here in the Inspector

    private bool isPaused = false;
    private bool showingObjectives = false;

    void Start()
    {
        pausePanel = GameObject.FindGameObjectWithTag("Pause_Resume_Panel");

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (objectivesPanel != null)
            objectivesPanel.SetActive(false);

        resumeButton?.onClick.AddListener(ResumeGame);
        quitButton?.onClick.AddListener(OnQuitPressed);
        objectivesButton?.onClick.AddListener(ShowObjectives);
        restartButton?.onClick.AddListener(RestartLevel);
    }

    void Update()
    {
        // toggle pause
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused && !showingObjectives)
                ResumeGame();
            else if (!isPaused)
                PauseGame();
        }

        // Close Objectives with Space
        if (showingObjectives && Input.GetKeyDown(KeyCode.Space))
        {
            HideObjectives();
        }
    }

    public void PauseGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
        if (cameraController != null)
            cameraController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
        if (cameraController != null)
            cameraController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }

    void ShowObjectives()
    {
        if (objectivesPanel != null)
        {
            objectivesPanel.SetActive(true);
            if (pausePanel != null)
                pausePanel.SetActive(false);

            showingObjectives = true;
        }
    }
    void HideObjectives()
    {
        if (objectivesPanel != null)
            objectivesPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(true);

        showingObjectives = false;
    }

    private void OnQuitPressed()
    {
        Debug.Log("QUIT pressed — show confirmation popup here.");
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
