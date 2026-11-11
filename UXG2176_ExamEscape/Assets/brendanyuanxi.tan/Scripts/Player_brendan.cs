using TMPro;
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
    public RawImage crosshairImg;

    public GameObject holdItem;
    public GameObject heldObject;
    [HideInInspector] public bool isHolding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Disable the interact UI prompt from the start
        interactUI.SetActive(false);
        placementUI.SetActive(false);

        crosshairImg = crosshair.GetComponent<RawImage>();
    }

    private void Update()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + ryacastYOffset, transform.position.z);
        Vector3 direction = transform.GetChild(0).forward;

        //Draws a line in the Scene view
        Debug.DrawRay(origin, direction * maxDistance, Color.red);

        //wasPressedThisFrame fires once per key press, instead of every frame while F is held
        bool interactPressed = Keyboard.current.fKey.wasPressedThisFrame;

        //If the Raycast hits somthing
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            //If the player is facing an interactable object, that can be selected and the player is not holding any other item
            if (!isHolding
                && hitObject.CompareTag("Interactable")
                && hitObject.TryGetComponent(out TargetObjects_brendan item)
                && item.canBeSelected)
            {
                //Turns on Interact UI
                ShowPrompt(true, false, Color.red);

                if (interactPressed)
                {
                    PickUp(hitObject);
                }
            }

            //If the player is facing the cabinet cubicle and are holding an item
            else if (isHolding && hitObject.CompareTag("Target") && IsSlotFree(hitObject))
            {
                //Turns on Placement UI
                ShowPrompt(false, true, Color.green);

                if (interactPressed)
                {
                    PlaceItem(hitObject);
                }
            }

            //Hit something that is neither: reset the prompts
            else
            {
                ShowPrompt(false, false, Color.black);
            }
        }

        //If raycast doesn't hit anything
        else
        {
            //Disables and reset the interact UI prompt
            ShowPrompt(false, false, Color.black);
        }
    }

    void PickUp(GameObject obj)
    {
        heldObject = obj;

        //Sets the interacting object to be parent of the player
        obj.transform.SetParent(holdItem.transform, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        SetHeldState(obj, true);
        isHolding = true;
    }

    bool IsSlotFree(GameObject target)
    {
        int slotCount = target.transform.GetChild(0).childCount;
        
        if (slotCount > 0)
        {
            return false;
        }

        else
        {
            return true;
        }
    }

    void PlaceItem(GameObject target)
    {
        //Return the GameObject back to the office enviromnent, at the target position of the cabinet cubicle
        Transform slot = target.transform.GetChild(0);

        heldObject.transform.SetParent(slot, false);
        heldObject.transform.localPosition = new Vector3(0, 0.5f, 0);
        heldObject.transform.localRotation = Quaternion.identity * Quaternion.Euler(0, 90, 0);

        SetHeldState(heldObject, false);

        //Checks if the item is placed correctly
        if (heldObject.TryGetComponent(out TargetObjects_brendan item) && item.targetPoint == target)
        {
            item.isPlacedCorrectly = true;
            item.canBeSelected = false;

            LevelManager_brendan.itemComplete++;

            //Debug.Log(LevelManager_brendan.itemComplete);
        }

        heldObject = null;
        isHolding = false;
    }

    //While held, the item's colliders are off so that they can't block the raycast
    void SetHeldState(GameObject obj, bool held)
    {
        foreach (Collider col in obj.GetComponentsInChildren<Collider>())
        {
            col.enabled = !held;
        }

        if (obj.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = held;
        }
    }

    void ShowPrompt(bool interact, bool placement, Color crosshairColor)
    {
        interactUI.SetActive(interact);
        placementUI.SetActive(placement);
        crosshairImg.color = crosshairColor;
    }
}