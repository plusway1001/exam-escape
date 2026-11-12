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
        //Disables the interact UI prompts from the start
        interactUI.SetActive(false);
        placementUI.SetActive(false);

        crosshairImg = crosshair.GetComponent<RawImage>();
    }

    private void Update()
    {

        //Var used to draw the raycast line
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + ryacastYOffset, transform.position.z);
        Vector3 direction = transform.GetChild(0).forward;

        //Draws a line in the Scene view
        Debug.DrawRay(origin, direction * maxDistance, Color.red);

        bool interactPressed = Keyboard.current.eKey.wasPressedThisFrame;

        //If the Raycast hits somthing
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            //Get the gameObject that the raycast hit
            GameObject hitObject = hit.collider.gameObject;

            //If the player is not holding an item, is facing an interactable object that can be selected and has the TargetObjects script component attached
            if (!isHolding
                && hitObject.CompareTag("Interactable")
                && hitObject.TryGetComponent(out TargetObjects_brendan item)
                && item.canBeSelected)
            {
                //Triggers the visual feedback update function
                ShowPrompt(true, false, Color.red);

                //If the interact key (eKey) is pressed
                if (interactPressed)
                {
                    //Add the targeted object to the player's hand
                    PickUp(hitObject);
                }
            }

            //But if the player is facing the cabinet cubicle and are holding an item
            else if (isHolding && hitObject.CompareTag("Target") && IsSlotFree(hitObject))
            {
                //Triggers the visual feedback update function
                ShowPrompt(false, true, Color.green);

                //If the interact key (eKey) is pressed
                if (interactPressed)
                {
                    //Callls the function to place the held item into the targeted cubiclle
                    PlaceItem(hitObject);
                }
            }

            //But if the player faces neither a targetable object, or a targetable cubicle
            else
            {
                //Resets the visual prompt to its default
                ShowPrompt(false, false, Color.black);
            }
        }

        //If raycast doesn't hit anything
        else
        {
            //Resets the visual prompt to its default
            ShowPrompt(false, false, Color.black);
        }
    }

    //Function used to pickup the targeted interactable object
    void PickUp(GameObject targetedObj)
    {
        //Assigns the targeted object reference to the heldObject var
        heldObject = targetedObj;

        //Sets the targeted object to be a parent of the player via the holdItem object attached to the player
        targetedObj.transform.SetParent(holdItem.transform, false);
        targetedObj.transform.localPosition = Vector3.zero;
        targetedObj.transform.localRotation = Quaternion.identity;

        //Disables the collider and rigidbody of the held item, to prevent interferences with the player's raycast
        SetHeldState(targetedObj, true);
        isHolding = true;
        AudioManager.Instance.PlaySfx(AudioManager.Instance.sounds.pickup);
    }

    //Function used to check if the cubicle the player is facing has an object already inside
    bool IsSlotFree(GameObject target)
    {
        //Get and assigns the number of children the targeted cubicle has
        int slotCount = target.transform.GetChild(0).childCount;

        //If the cubicle gameObject has a child object assigned to it, slotCount > 0, the cubicle is occupied and is not free
        if (slotCount > 0)
        {
            return false;
        }

        //If the cubicle gameObject does not have  a child object assigned to it, slotCount < 0, the cubicle is occupied and is free
        else
        {
            return true;
        }
    }

    //Function used to place the held item into the targeted cubicle
    void PlaceItem(GameObject target)
    {
        //Get and assign the transform of the targeted cubicle for reference
        Transform slot = target.transform.GetChild(0);

        //Return the GameObject back to the office enviromnent, at the target position of the cabinet cubicle
        heldObject.transform.SetParent(slot, false);
        heldObject.transform.localPosition = new Vector3(0, 0.5f, 0);
        heldObject.transform.localRotation = Quaternion.identity * Quaternion.Euler(0, 90, 0);

        //Enables the placed object's collider and rigidbody after placing it down in the game's enviromnent
        SetHeldState(heldObject, false);

        //Checks if the item is placed correctly by comparing the object's currentlly cubicle matches with the cubicle it's supposed to be in
        if (heldObject.TryGetComponent(out TargetObjects_brendan item) && item.targetPoint == target)
        {

            //Disable the object's ability to be selected and picked up by the player again since its already in the correct posiiton
            item.isPlacedCorrectly = true;
            item.canBeSelected = false;

            //Increases the count of correctly place object by 1
            LevelManager_brendan.itemComplete++;

        }

        AudioManager.Instance.PlaySfxAt(AudioManager.Instance.sounds.place, heldObject.transform.position);

        //Resets the var used to store the hled objects since the object is now placed and no longer in the player's hand
        heldObject = null;
        isHolding = false;
    }

    //Function used to disable the collider and rigidbody of the item held in the player's hand to prevent it from interferring with the player's raycast, or have the object be affected by external forces and physics while being held onto by the player
    void SetHeldState(GameObject obj, bool held)
    {
        //Searches for the collider and rigidybody foreach item held by the player, in the case == 1
        foreach (Collider col in obj.GetComponentsInChildren<Collider>())
        {
            col.enabled = !held;
        }

        if (obj.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = held;
        }
    }

    //Trigger to detect if player is attempting to leave the scene via the door
    private void OnTriggerEnter(Collider other)
    {
        Gunnode.GameManager.CompleteLevel();
    }

    //Function to update the crosshair for visual feedback, and enables/disables the required UI prompt
    void ShowPrompt(bool interact, bool placement, Color crosshairColor)
    {
        interactUI.SetActive(interact);
        placementUI.SetActive(placement);
        crosshairImg.color = crosshairColor;
    }
}