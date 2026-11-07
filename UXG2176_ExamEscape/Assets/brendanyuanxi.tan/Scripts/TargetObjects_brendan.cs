using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class TargetObjects_brendan : MonoBehaviour
{
    [HideInInspector] public bool canBeSelected = true;
    [HideInInspector] public bool isPlacedCorrectly;

    public GameObject targetPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlacedCorrectly)
        {
            canBeSelected = false;
            

        }
    }

}
