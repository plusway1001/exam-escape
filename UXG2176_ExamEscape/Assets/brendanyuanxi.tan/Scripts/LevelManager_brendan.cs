using Unity.Android.Gradle;
using UnityEngine;

public class LevelManager_brendan : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    [HideInInspector] public static int itemComplete = 0;
    [HideInInspector] public bool gameOver = false;

    [HideInInspector] public GameObject[] door;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player_brendan>().gameObject;
        door = GameObject.FindGameObjectsWithTag("Finish");

        //Debug.Log(player);
    }

    // Update is called once per frame
    void Update()
    {
        if (itemComplete == 5)
        {
            gameOver = true;
            
            foreach (GameObject e in door)
            {
                Destroy(e);
            }
        }
    }
}
