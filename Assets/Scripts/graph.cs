using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class graph : MonoBehaviour
{

    private RectTransform graphContainer;
    [SerializeField] private Sprite circleSprite;
    private void Awake()
    {
        graphContainer= transform.Find("graphContainer").GetComponent<RectTransform>();

    }
    private void CreateCircle(Vector2 anchoredPosition )
    {
        GameObject gameObject = new GameObject("cicle",typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;

    }
}
