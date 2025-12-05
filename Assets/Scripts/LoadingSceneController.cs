using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    public float waitTime = 5f;
    public Image loadingBar;   // <-- added this so we can animate it

    // fake milestones
    private float[] milestones = { 0.25f, 0.55f, 0.82f, 1f };

    void Start()
    {
        if (loadingBar != null)
            loadingBar.fillAmount = 0f;

        StartCoroutine(GoToMainLobby());
    }

    IEnumerator GoToMainLobby()
    {
        float timer = 0f;
        float displayedProgress = 0f;

        // --- Animate loading bar while we wait ---
        foreach (float target in milestones)
        {
            while (displayedProgress < target)
            {
                displayedProgress += Time.deltaTime * 0.6f;
                if (loadingBar != null)
                    loadingBar.fillAmount = displayedProgress;

                yield return null;
            }

            if (loadingBar != null)
                loadingBar.fillAmount = target;

            // small vibe pause
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }

        // --- Still respect your original waitTime ---
        while (timer < waitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene("MainLobby");
    }
}
