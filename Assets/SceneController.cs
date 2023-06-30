using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    public FadeScreen fadeScreen;

   
    // Start is called before the first frame update
    void Awake()
    {
        

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //public void GoToScene()
    //{
    //    StartCoroutine(GoToSceneRoutine());
    //}

    //IEnumerator GoToSceneRoutine()
    //{
    //    fadeScreen.FadeOut();
    //    yield return new WaitForSeconds(fadeScreen.fadeDuration);

    //    // Launch the new scene

    //    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex +1);
    //}

    public void GoToSceneAsync(int sceneIndex)
    {
        StartCoroutine(GoToSceneAsyncRoutine(sceneIndex));
    }

    IEnumerator GoToSceneAsyncRoutine(int sceneIndex)
    {
        

        if (fadeScreen == true)
        {
            fadeScreen.FadeOut();
          
        }



        // yield return new WaitForSeconds(fadeScreen.fadeDuration);

        // Launch the new scene

        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float timer = 0;
        while (timer <= fadeScreen.fadeDuration && !operation.isDone)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        operation.allowSceneActivation = true;
    }



    //public void NextLevel()
    //{
    //    Debug.Log("NextLevel called");
    //    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    //}
}
