using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingScreenController : MonoBehaviour
{
    public float delay = 3f;    //delay duration


    void Start()
    {
        Invoke("GoNext", delay);
    }

    void GoNext()
    {
        SceneManager.LoadScene("LoadingScene");
    }
}
