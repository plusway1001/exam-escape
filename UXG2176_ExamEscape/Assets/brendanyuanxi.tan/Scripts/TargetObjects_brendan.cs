using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class TargetObjects_brendan : MonoBehaviour
{
    //Script attached to the interactable objects 
    public bool canBeSelected = true;
    public bool isPlacedCorrectly;

    public GameObject targetPoint;

    // Update is called once per frame
    void Update()
    {
        if (isPlacedCorrectly)
        {
            //If the item this script is attached to is placed in the correct cubicle, prevent the item from being interactable again
            canBeSelected = false;
            

        }
    }

}
