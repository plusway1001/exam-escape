using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager_brendan : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    [HideInInspector] public static int itemComplete = 0;
    [HideInInspector] public bool gameOver = false;

    //Array to store the door's referencess as the prefab contains multiple gameObjects that builds up the doorway
    [HideInInspector] public GameObject[] door;

    public GameObject instructionsPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Find the object with the relevant component and assign to the reference
        player = FindFirstObjectByType<Player_brendan>().gameObject;
        door = GameObject.FindGameObjectsWithTag("Finish");

        //Resets counter to 0, to prevent completion progress from being brought over when restarting scene during gameplay
        itemComplete = 0;

        //Opens instructions panel at the start of the gaame
        instructionsPanel.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        //Checks if the puzzle is completed
        if (itemComplete == 5)
        {
            gameOver = true;
            
            //Destroy the door object to reveal the exit to the player
            foreach (GameObject e in door)
            {
                Destroy(e);
            }
        }

        //Checks for the relevant keys being pressed to open/close the instructions panel when required
        if (Keyboard.current.qKey.wasPressedThisFrame && !instructionsPanel.activeSelf)
        {
            instructionsPanel.SetActive(true);
        }

        else if (Keyboard.current.qKey.wasPressedThisFrame && instructionsPanel.activeSelf)
        {
            instructionsPanel.SetActive(false);
        }
    }
}
