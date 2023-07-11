using UnityEngine;

public class InstantiateAtOffset : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public Vector3 offset = new Vector3(0f, 0f, 1f);

    public void InstantiateObjectAtOffset()
    {
        Vector3 spawnPosition = transform.position + transform.forward * offset.z +
                                transform.up * offset.y + transform.right * offset.x;

        Instantiate(prefabToInstantiate, spawnPosition, Quaternion.identity);
    }
}
