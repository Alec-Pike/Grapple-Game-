using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RestartLevel()
    {
        Debug.Log("Retry button was pressed");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
