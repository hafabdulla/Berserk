using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private GameObject pausePanel;
    public GameObject objectivesPanel;

    // ----------------------- NEW: Quit Panel -----------------------
    private GameObject quitGamePanel;
    public Button quitResumeButton;   // RESUME on Quit Panel
    public Button quitConfirmButton;  // CONFIRM on Quit Panel
    // ---------------------------------------------------------------

    [Header("Buttons")]
    public Button resumeButton;
    public Button objectivesButton;
    public Button quitButton;
    public Button restartButton;

    [Header("Camera Control Script")]
    public MonoBehaviour cameraController;

    private bool isPaused = false;
    private bool showingObjectives = false;

    void Start()
    {
        pausePanel = GameObject.FindGameObjectWithTag("Pause_Resume_Panel");
        quitGamePanel = GameObject.FindGameObjectWithTag("GameQuitPanel");

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (objectivesPanel != null)
            objectivesPanel.SetActive(false);

        if (quitGamePanel != null)
            quitGamePanel.SetActive(false);

        resumeButton?.onClick.AddListener(ResumeGame);
        quitButton?.onClick.AddListener(OpenQuitPanel);
        objectivesButton?.onClick.AddListener(ShowObjectives);
        restartButton?.onClick.AddListener(RestartLevel);

        // Quit panel buttons
        quitResumeButton?.onClick.AddListener(CloseQuitPanel);
        quitConfirmButton?.onClick.AddListener(ConfirmQuit);
    }

    void Update()
    {
        // toggle pause
        if (Input.GetKeyDown(KeyCode.P))
        {
            // If Quit Panel is open → close that first
            if (quitGamePanel != null && quitGamePanel.activeSelf)
            {
                CloseQuitPanel();
                return;
            }

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

        if (quitGamePanel != null)
            quitGamePanel.SetActive(false);

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

    // -------------------------------------------------------------------
    //                     QUIT PANEL LOGIC
    // -------------------------------------------------------------------

    private void OpenQuitPanel()
    {
        if (quitGamePanel != null)
            quitGamePanel.SetActive(true);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void CloseQuitPanel()
    {
        if (quitGamePanel != null)
            quitGamePanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    private void ConfirmQuit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainLobby");
    }

    // -------------------------------------------------------------------
    //                           RESTART
    // -------------------------------------------------------------------
    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
