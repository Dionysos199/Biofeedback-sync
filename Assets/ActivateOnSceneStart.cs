using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivateOnSceneStart : MonoBehaviour
{
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the loaded scene matches the current scene
        if (scene == SceneManager.GetActiveScene())
        {
            // Activate this object
            gameObject.SetActive(true);
        }
    }
}

