using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player_brendan : MonoBehaviour
{
    [HideInInspector] public float maxDistance = 2f;
    [HideInInspector] public float ryacastYOffset = 0.5f;

    public GameObject interactUI;
    public GameObject placementUI;
    public GameObject crosshair;

    public GameObject holdItem;
    [HideInInspector] public bool isHolding = false;

    [HideInInspector] public GameObject officePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Disable the interact UI prompt from the start
        interactUI.SetActive(false);
        placementUI.SetActive(false);

        officePrefab = GameObject.FindGameObjectWithTag("Office");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + ryacastYOffset, transform.position.z);
        Vector3 direction = transform.GetChild(0).forward;

        //Draws a line in the Scene view
        Debug.DrawRay(origin, direction * maxDistance, Color.red);

        //If the Raycast hits somthing
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            //Debug.Log(hit.collider);
            GameObject gameObject = hit.collider.gameObject;

            if (gameObject.CompareTag("Interactable") && gameObject.GetComponent<TargetObjects_brendan>().canBeSelected && !isHolding)
            {
                //Debug.Log("Interact with object");
                interactUI.SetActive(true);
                crosshair.GetComponent<RawImage>().color = Color.red;

                //Debug.Log(gameObject.name);

                if (Keyboard.current.fKey.isPressed)
                {
                    gameObject.transform.SetParent(holdItem.transform, false);
                    gameObject.transform.position = holdItem.transform.position;


                    isHolding = true;

                }
            }

            else if (gameObject.CompareTag("Target") && isHolding)
            {
                placementUI.SetActive(true);
                crosshair.GetComponent<RawImage>().color = Color.green;


                if (Keyboard.current.fKey.isPressed)
                {
                    GameObject targetPlace = holdItem.transform.GetChild(0).gameObject;

                    targetPlace.transform.SetParent(officePrefab.transform, false);
                    targetPlace.transform.position = gameObject.transform.GetChild(0).transform.position;

                    if (targetPlace.GetComponent<TargetObjects_brendan>().targetPoint == gameObject)
                    {
                        targetPlace.transform.GetComponent<TargetObjects_brendan>().isPlacedCorrectly = true;
                        LevelManager_brendan.itemComplete++;

                        Debug.Log(LevelManager_brendan.itemComplete);
                    }

                    isHolding = false;
                }
            }


        }

        //If raycast doesn't hit anything
        else
        {
            //Disable the interact UI prompt
            interactUI.SetActive(false);
            placementUI.SetActive(false);
            crosshair.GetComponent<RawImage>().color = Color.black;

        }
    }
}
