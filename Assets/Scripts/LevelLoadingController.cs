using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class LevelLoadingController : MonoBehaviour
{
    public Image loadingBar;
    public float minWaitTime = 1f;

    private string levelToLoad;

    // Add fake milestones
    private float[] milestones = { 0.25f, 0.55f, 0.82f, 1f };

    void Start()
    {
        levelToLoad = PlayerPrefs.GetString("NextLevelToLoad", "Level1");
        StartCoroutine(LoadLevelAsync());
    }

    IEnumerator LoadLevelAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(levelToLoad);
        op.allowSceneActivation = false;

        float displayedProgress = 0f;
        loadingBar.fillAmount = 0f;

        float timer = 0f;

        // ---- Fake progress animation ----
        foreach (float target in milestones)
        {
            while (displayedProgress < target)
            {
                displayedProgress += Time.deltaTime * 0.6f;   // bar animation speed
                loadingBar.fillAmount = displayedProgress;

                yield return null;
            }

            loadingBar.fillAmount = target;

            // short break to make it feel organic
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }

        // ---- Wait for real Unity async loading ----
        while (op.progress < 0.9f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Respect your min wait time
        while (timer < minWaitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Allow entering the level
        op.allowSceneActivation = true;
    }
}
