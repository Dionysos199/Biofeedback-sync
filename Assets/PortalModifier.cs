using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalModifier : MonoBehaviour
{
    private Renderer _renderer;

    public float newBrightness;
    public float StartBrightness = 3.89f;
    public float EndBrightness = 0.24f;
    public float TriggerDistance =10f;
    public float DecreaseValue = 1.5f;
    public float duration = 5f;

    public Transform player;
    public Transform SceneTransitionObject;
   

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        StartCoroutine(LerpValue(StartBrightness, EndBrightness));
    }

   
    IEnumerator LerpValue(float StartBrightness, float EndBrightness) // changing from void function to Co Routine
                                                                      // and call it in the start function instead of update
    {

       if ( Vector3.Distance(player.position, SceneTransitionObject.position) <= TriggerDistance)
        {
            //Debug.Log("TriggerDistance reached");
           float time = 0;
           while(time < duration) // instead of "if"
            {
                float t = time / duration;
                newBrightness = Mathf.Lerp(StartBrightness, EndBrightness,t);
           

               // _renderer.sharedMaterial.SetFloat("_Brightness", newBrightness);
                _renderer.material.SetFloat("_Brightness", newBrightness);

                time += Time.deltaTime;// instead of calling it before setting the new float value
                yield return null;
            }

            //_renderer.sharedMaterial.SetFloat("_Brightness", newBrightness);
             _renderer.material.SetFloat("_Brightness", newBrightness);


            //old version instead of lerp function//

            //float delta = (StartBrightness - DecreaseValue) * Time.deltaTime;
            //delta *= Time.deltaTime;

            // StartBrightness -= delta;

        }

    }

   
}
