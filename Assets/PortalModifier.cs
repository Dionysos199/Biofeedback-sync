using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalModifier : MonoBehaviour
{
    private Renderer _renderer;

    public float StartBrightness = 3.89f;
    public float EndBrightness = 0.24f;
    public float TriggerDistance =10f;
    public float DecreaseValue = 0.5f;

    public Transform player;
    public Transform SceneTransitionObject;
   

    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        PlayerDistance();
    }
    public void PlayerDistance()
    {

       if ( Vector3.Distance(player.position, SceneTransitionObject.position) <= TriggerDistance)
        {
            //_renderer.material.SetFloat("_Brightness", StartBrightness);
            _renderer.sharedMaterial.SetFloat("_Brightness", StartBrightness);

           Debug.Log("Brightness gets decreased");
           float delta = (StartBrightness - DecreaseValue) * Time.deltaTime;
           delta *= Time.deltaTime;

           // StartBrightness -= delta;

            //float t = 1 - (Vector3.Distance(player.position, SceneTransitionObject.position) / 10);
            //float brightness = Mathf.Lerp(StartBrightness, EndBrightness, t);
            //_renderer.material.SetFloat("Brightness", brightness);
        }
       
    }

   
}
