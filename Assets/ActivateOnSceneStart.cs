using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivateOnSceneStart : MonoBehaviour
{
    private AudioListener _audioListener;
    
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        _audioListener = GetComponent<AudioListener>();

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
            _audioListener.enabled = true;

            // Activate this object
            gameObject.SetActive(true);
        }
    }
}

