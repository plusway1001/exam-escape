using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager_brendan : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    [HideInInspector] public static int itemComplete = 0;
    [HideInInspector] public bool gameOver = false;

    [HideInInspector] public GameObject[] door;

    public GameObject instructionsPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player_brendan>().gameObject;
        door = GameObject.FindGameObjectsWithTag("Finish");

        instructionsPanel.SetActive(true);
        //Debug.Log(player);
    }

    // Update is called once per frame
    void Update()
    {
        if (itemComplete == 5)
        {
            gameOver = true;
            Gunnode.GameManager.Lose();
            
            foreach (GameObject e in door)
            {
                Destroy(e);
            }
        }

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
