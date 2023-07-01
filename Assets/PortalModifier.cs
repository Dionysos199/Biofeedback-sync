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
        //StartCoroutine(LerpValue(StartBrightness, EndBrightness));
      
    }

    private void Update()
    {
        LerpValue();
    }
    //  void LerpValue(float StartBrightness, float EndBrightness) // changing from void function to Co Routine
    void LerpValue()                                                                // and call it in the start function instead of update
    {
        float d = Vector3.Distance(player.position, this.transform.position);
       if ( d  <= TriggerDistance)
        {
            newBrightness = d*d/50;//if d=14 d*d=196, 196/50=3.9 so brightness starts at 3.9 and decreases as d decreases
            _renderer.material.SetFloat("_Brightness",newBrightness);
            Debug.Log("TriggerDistance reached");
            //float time = 0;
           //while(time < duration) // instead of "if"
           // {
           //     float t = time / duration;
           //     newBrightness = Mathf.Lerp(StartBrightness, EndBrightness,t);
           

           //    // _renderer.sharedMaterial.SetFloat("_Brightness", newBrightness);
           //     _renderer.material.SetFloat("_Brightness", newBrightness*d/10);

           //     time += Time.deltaTime;// instead of calling it before setting the new float value
           //     yield return null;
           // }

            //_renderer.sharedMaterial.SetFloat("_Brightness", newBrightness);
            // _renderer.material.SetFloat("_Brightness", newBrightness);


            //old version instead of lerp function//

            //float delta = (StartBrightness - DecreaseValue) * Time.deltaTime;
            //delta *= Time.deltaTime;

            // StartBrightness -= delta;

        }

    }

   
}
