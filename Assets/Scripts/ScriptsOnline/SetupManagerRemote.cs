using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;


public class SetupManagerRemote : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI nameInputPrompt;
    [SerializeField] TextMeshProUGUI playerNames;

    // Start is called before the first frame update
    void Awake()
    {
        
        nameInputPrompt.text = "Please enter your name.";

    }

    public void MultiplayerNameAssign()

    {        
            MainManager.multiplayerName = playerNames.text;

            Debug.Log("Player  name is " + MainManager.multiplayerName);

            playerNames.gameObject.SetActive(false);

            
            StartNewGame();
        

    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartNewGame()

    {

        switch (MainManager.Instance.gameIsRemote)

        {
            case true:
                SceneManager.LoadScene(4);

                break;

            case false:
                SceneManager.LoadScene(3);
                break;
        }


    }
}
