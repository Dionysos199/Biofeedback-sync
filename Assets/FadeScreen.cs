using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;

public class FadeScreen : MonoBehaviour
{
    public bool fadeOnStart = false;
    public float fadeDuration = 2;
    public Color fadeColor;
    private Renderer _renderer;

  
  
    void Start()
    {
     
        _renderer = GetComponent<Renderer>();
       
        if (fadeOnStart)
        {
            FadeIn();
          
        }
    }

    public void FadeIn()
    {
        Fade(1, 0);
    }
    public void FadeOut()
    {
        Fade(0,1);
    }



    public void Fade(float alphaIn, float alphaOut)
    {
        StartCoroutine(FadeRoutine(alphaIn, alphaOut)); 
    }
        
    public IEnumerator FadeRoutine(float alphaIn, float alphaOut)
    {
        float timer = 0;
        while( timer<= fadeDuration )
        {
            Color newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer / fadeDuration); // If the third paramter is equal to 0 its equal to alphaIn
                                                                              // and if its equal to 1 its equal to alphaout
                                                                              // so at some point the timer will be equal to fadeDuration
                                                                              // according to the while loop so it will be 1/1 in the Lerp function
                                                                              //and the outcome is 1


            _renderer.material.SetColor("_Color", newColor); // We access the name of the color in the shader via properties and overhand our new created color


            timer += Time.deltaTime; // every frame of the game it will increase the time value
                                     // and once the timer is bigger than the fade duration it will exit this loop 

            yield return null; // so we wait for one frame before we go on
        }
        // To make sure we have alphaOut Output in the end
        Color newColor2 = fadeColor;
        newColor2.a = alphaOut;
        
        _renderer.material.SetColor("_Color", newColor2); 
    }
}
