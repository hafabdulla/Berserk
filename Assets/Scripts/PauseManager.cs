using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameObject pausePanel;
    public GameObject objectivesPanel;

    // ----------------------- NEW: Quit Panel -----------------------
    private GameObject quitGamePanel;
    public Button quitResumeButton;   // RESUME on Quit Panel
    public Button quitConfirmButton;  // CONFIRM on Quit Panel

    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
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

    [Header("Level Objectives")]
    public int currentLevel; // Set automatically based on scene
    public int enemiesKilledCount = 0;
    public int requiredKillsForLevel2 = 5;
    public int requiredKillsForLevel1 = 2;
    private bool targetDestroyed = false;

    // ----------------------- EXISTING SCORE (Lifetime Total) -----------------------
    private int totalCoins = 0;
    private int totalKills = 0;
    private float totalDamageDealt = 0f;
    private bool levelCompleted = false;

    // ----------------------- NEW: Additional Score Tracking -----------------------
    [Header("Score Display (Optional)")]
    public TextMeshProUGUI currentLevelScoreText; // Assign in inspector - displays current level score in real-time

    // Current Level Score - resets each level
    private int currentLevelScore = 0;
    private int currentLevelKills = 0;

    // Current Session Score - sum of all levels in this play session
    private int sessionScore = 0;
    // -------------------------------------------------------------------------------

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            LoadTotalCoins();
            LoadSessionScore();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Determine current level from scene name or build index
        DetermineCurrentLevel();
    }

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

        // Update score display initially
        UpdateCurrentLevelScoreDisplay();

        Debug.Log($"GameManager initialized for Level {currentLevel}");
    }

    void DetermineCurrentLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // Try to extract level number from scene name
        if (sceneName.Contains("Level1") || sceneName.Contains("level1"))
            currentLevel = 1;
        else if (sceneName.Contains("Level2") || sceneName.Contains("level2"))
            currentLevel = 2;
        else if (sceneName.Contains("Level3") || sceneName.Contains("level3"))
            currentLevel = 3;
        else
        {
            currentLevel = SceneManager.GetActiveScene().buildIndex + 5;
        }
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

    /// 
    /// when the target is destroyed (Level 1)
    /// 
    public bool IsTargetDestroyed()
    {
        return targetDestroyed;
    }

    public int GetEnemyKillCount()
    {
        return enemiesKilledCount;
    }

    public void OnTargetDestroyed()
    {
        if (currentLevel == 1 && !levelCompleted)
        {
            Debug.Log("Target destroyed!");
            targetDestroyed = true;
            CheckLevel1Completion();
        }
    }

    /// 
    /// when an enemy is killed (Level 2)
    /// 
    public void OnEnemyKilled(string EnemyType)
    {
        enemiesKilledCount++;
        Debug.Log($"Enemy killed! Total count: {enemiesKilledCount}");

        if (currentLevel == 1 && !levelCompleted)
        {
            Debug.Log($"Level 1 - Enemies killed: {enemiesKilledCount}/{requiredKillsForLevel1}");
            CheckLevel1Completion();
        }
        else if (currentLevel == 2 && !levelCompleted)
        {
            Debug.Log($"Level 2 - Enemies killed: {enemiesKilledCount}/{requiredKillsForLevel2}");

            if (enemiesKilledCount >= requiredKillsForLevel2)
            {
                Debug.Log("All enemies killed! Level 2 complete!");
                levelCompleted = true;
                ShowLevelComplete();
            }
        }

        // Coin Collection - Calculate coins earned
        int coinsEarned = 0;

        if (EnemyType == "Zombie")
        {
            coinsEarned = 10; // 10 coins for killing a zombie
        }
        else if (EnemyType == "CyberMonster")
        {
            coinsEarned = 20; // 20 coins for killing a cyber monster
        }

        // Update existing lifetime total (your original logic)
        totalCoins += coinsEarned;
        totalKills++;
        SaveTotalCoins();

        // NEW: Update current level score
        currentLevelScore += coinsEarned;
        currentLevelKills++;
        UpdateCurrentLevelScoreDisplay();

        Debug.Log($"Enemy killed: {EnemyType}. Coins earned: {coinsEarned}. Level Score: {currentLevelScore}. Total coins: {totalCoins}");
    }

    public void AddDamageDealt(float damage)
    {
        totalDamageDealt += damage;
    }

    public void OnLevelCompleted()
    {
        levelCompleted = true;

        // Add coins based on total damage dealt
        int damageCoins = Mathf.FloorToInt(totalDamageDealt / 10f); // 1 coin for every 10 damage
        totalCoins += damageCoins;

        // NEW: Add damage bonus to current level score
        currentLevelScore += damageCoins;

        SaveTotalCoins();
        UpdateCurrentLevelScoreDisplay();

        Debug.Log($"Level completed! Damage coins earned: {damageCoins}. Total coins: {totalCoins}");
    }

    // ----------------------- EXISTING GETTERS (unchanged) -----------------------
    public int GetTotalCoins()
    {
        return totalCoins;
    }

    public int GetTotalKills()
    {
        return totalKills;
    }

    public float GetTotalDamageDealt()
    {
        return totalDamageDealt;
    }

    // ----------------------- NEW: Additional Getters -----------------------
    public int GetCurrentLevelScore()
    {
        return currentLevelScore;
    }

    public int GetCurrentLevelKills()
    {
        return currentLevelKills;
    }

    public int GetSessionScore()
    {
        return sessionScore;
    }

    // ----------------------- EXISTING SAVE/LOAD (unchanged) -----------------------
    private void SaveTotalCoins()
    {
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();
    }

    private void LoadTotalCoins()
    {
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0); // Default to 0 if no coins are saved
    }

    // ----------------------- NEW: Session Score Save/Load -----------------------
    private void LoadSessionScore()
    {
        sessionScore = PlayerPrefs.GetInt("SessionScore", 0);
    }

    private void SaveSessionScore()
    {
        PlayerPrefs.SetInt("SessionScore", sessionScore);
        PlayerPrefs.Save();
    }

    private void UpdateCurrentLevelScoreDisplay()
    {
        if (currentLevelScoreText != null)
        {
            currentLevelScoreText.text = "Level Score: " + currentLevelScore.ToString();
        }
    }

    // Called when level ends - adds current level score to session
    private void FinalizeCurrentLevelScore()
    {
        sessionScore += currentLevelScore;
        SaveSessionScore();
        Debug.Log($"Level finalized! Level Score: {currentLevelScore}, Session Score: {sessionScore}");
    }

    // Call this when starting a new game session (from MainLobby)
    public static void ResetSessionScore()
    {
        PlayerPrefs.SetInt("SessionScore", 0);
        PlayerPrefs.Save();
    }
    // -------------------------------------------------------------------------------

    private void CheckLevel1Completion()
    {
        if (targetDestroyed && enemiesKilledCount >= requiredKillsForLevel1)
        {
            Debug.Log("Level 1 Complete! Target destroyed AND 2 enemies killed!");
            levelCompleted = true;
            ShowLevelComplete();
        }
        else
        {
            string targetStatus = targetDestroyed ? "Target destroyed" : "Target not destroyed";
            string enemyStatus = $"{enemiesKilledCount}/{requiredKillsForLevel1} enemies killed";
            Debug.Log($"Level 1 Progress: {targetStatus} | {enemyStatus}");
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
        // NEW: Finalize session score before quitting
        FinalizeCurrentLevelScore();

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
        // NEW: Finalize session score on game over
        FinalizeCurrentLevelScore();

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
        // NEW: Finalize session score before loading next level
        FinalizeCurrentLevelScore();

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
        Time.timeScale = 1f;

        Color c = fadeImage.color;

        while (c.a < 1f)
        {
            c.a += Time.deltaTime * fadeSpeed;
            fadeImage.color = c;
            yield return null;
        }

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            SceneManager.LoadScene("MainLobby");
    }
}
