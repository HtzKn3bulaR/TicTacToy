using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public bool gameIsRemote = false;

    public static string[] playerName = { "Player 1", "Player 2" };
    public static string multiplayerName;

    public static int roundsWonP1 = 0;
    public static int roundsWonP2 = 0;

    public static string carClass = "Re-Volt";
    public static string[] cars = { "Car1", "Car2", "Car3", "Car4" };

    public static bool carCIsActive = false;
    public static bool carDIsActive = false;

    public static bool p1HasJoker = false;
    public static bool p2HasJoker = false;

    public static bool xJokerWasUsed = false;
    public static bool oJokerWasUsed = false;

    public static string selectedFilePath;

    public static string joinCode;
    public static string clientJoinCode;

    public static bool gameUsingLogs = false;
    
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
