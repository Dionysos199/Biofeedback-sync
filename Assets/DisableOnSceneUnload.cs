using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableOnSceneUnload : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene unloadedScene)
    {
        // Check if the unloaded scene matches the current scene
        if (unloadedScene == SceneManager.GetActiveScene())
        {
            // Disable this object
            gameObject.SetActive(false);
        }
    }
}
