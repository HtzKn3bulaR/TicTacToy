using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public static string[] playerName = { "Player 1", "Player 2" };

    public static int roundsWonP1 = 0;
    public static int roundsWonP2 = 0;

    
    private void Awake()
    {
        if (Instance != null)

        { Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
