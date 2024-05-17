using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SetupManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameInputPrompt;
    [SerializeField] TextMeshProUGUI playerNames;

    // Start is called before the first frame update
    void Awake()
    {
        nameInputPrompt.text = "Player 1, please enter your name.";
                        
    }

    public void P1Assign()

    {if (MainManager.playerName[0] == "Player 1")

        {
            MainManager.playerName[0] = playerNames.text;

            Debug.Log("Player 1 name is " + MainManager.playerName[0]);

            playerNames.gameObject.SetActive(false);

            nameInputPrompt.text = "Player 2, please enter your name.";

            playerNames.gameObject.SetActive(true);
        }

        else

        {
            MainManager.playerName[1] = playerNames.text;
            Debug.Log("Player 2 name is " + MainManager.playerName[1]);
            playerNames.gameObject.SetActive(false);

            StartNewGame();
        }
                
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartNewGame()

    {
        SceneManager.LoadScene(2);
    }
}
