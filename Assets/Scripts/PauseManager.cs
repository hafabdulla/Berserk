using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private GameObject pausePanel;
    public GameObject objectivesPanel;

    // ----------------------- NEW: Quit Panel -----------------------
    private GameObject quitGamePanel;
    public Button quitResumeButton;   // RESUME on Quit Panel
    public Button quitConfirmButton;  // CONFIRM on Quit Panel

    public GameObject gameOverPanel;
    public  GameObject levelCompletePanel;
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

    [Header("Fade Image")]
    public Image fadeImage;
    public float fadeSpeed = 1f;
    void Start()
    {
        pausePanel = GameObject.FindGameObjectWithTag("Pause_Resume_Panel");
        quitGamePanel = GameObject.FindGameObjectWithTag("GameQuitPanel");
        levelCompletePanel = GameObject.FindGameObjectWithTag("LevelCompletePanel");

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (objectivesPanel != null)
            objectivesPanel.SetActive(false);

        if (quitGamePanel != null)
            quitGamePanel.SetActive(false);

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

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

    public void OpenQuitPanel()
    {
        if (quitGamePanel != null)
            quitGamePanel.SetActive(true);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void CloseQuitPanel()
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

    // ---------------- GAME OVER ----------------
    public void ShowGameOver()
    {
        Time.timeScale = 0f;

        if (cameraController != null)
            cameraController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }


    public void GameOver_Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver_MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainLobby");
    }


    // ---------------- LEVEL COMPLETE ----------------

    public void LoadNextLevel()
    {
        StartCoroutine(FadeAndLoadNext());
    }


    public void RestartLevel_FromWin()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ShowLevelComplete()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        Time.timeScale = 0f;

        if (cameraController != null)
            cameraController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator FadeAndLoadNext()
    {
        // make sure time runs normally
        Time.timeScale = 1f;

        Color c = fadeImage.color;

        // fade alpha from 0 → 1
        while (c.a < 1f)
        {
            c.a += Time.deltaTime * fadeSpeed;
            fadeImage.color = c;
            yield return null;
        }

        // after full fade → load next level
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            SceneManager.LoadScene("MainLobby");
    }

}
