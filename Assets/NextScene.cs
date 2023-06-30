using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public FadeScreen fadeScreen;
    public InvertObjectNormals _invertObjectnormals;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PlayerCollided");
            //SceneController.instance.NextLevel();
           fadeScreen.fadeOnStart = true;
           _invertObjectnormals.enabled = true;


            SceneController.instance.GoToSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
