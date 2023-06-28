using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyPortal : MonoBehaviour
{
    GameObject obj = GameObject.Find("Portal");

   
    // Start is called before the first frame update
    void Start()
    {
        

       
    }

    // Update is called once per frame
    void Update()
    {
        if (obj != null);

        // Access the desired component attached to the game object
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            // Access the material of the component
            Material material = renderer.material;

            
            // Access specific variables of the material
            Color color = material.color;
            Texture texture = material.mainTexture;


         
        }
    }
}
