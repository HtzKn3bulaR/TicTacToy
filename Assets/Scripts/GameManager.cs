using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI nextTrack;
    public TextMeshProUGUI[] fieldText;
    [SerializeField] private TextMeshProUGUI p1Name;
    [SerializeField] private TextMeshProUGUI p2Name;
    public int activePlayer = 0;
    public Sprite[] playerSymbols;
    public Button[] fields;
    public GameObject preRacePanel;
    public GameObject postRacePanel;
    private int pendingField;
    public TMP_Dropdown winnerSelect;
    private int roundWinner;
    public int[] occupiedFields;
    private bool gameEnded = false;

    public TextMeshProUGUI jokerP1;
    public TextMeshProUGUI jokerP2;

    // Start is called before the first frame update
    void Start()
    {
        switch (activePlayer)

        {
            case 0:
                messageText.text = ($"{MainManager.playerName[0]}, select your field.");
                break;
        }

        FieldSetup();

        NamesSetup();

    }

    public void FieldSetup()

    {

        for (int i = 0; i < fields.Length; i++)

        {
            fields[i].interactable = true;
            fields[i].GetComponent<Image>().sprite = null;
        }

        for (int i = 0; i < occupiedFields.Length; i++)

        {
            occupiedFields[i] = -100;
        }

    }

    private void NamesSetup()

    {
        p1Name.text = MainManager.playerName[0];
        p2Name.text = MainManager.playerName[1];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ButtonClicked(int fieldNumber)
    {
        preRacePanel.SetActive(true);
        string trackSelected = (fieldText[fieldNumber].GetComponentInChildren<TMP_Text>().text);
        //fields[fieldNumber].interactable = false;
        //fields[fieldNumber].GetComponentInChildren<TMP_Text>().SetText("");
        nextTrack.text = trackSelected;
        pendingField = fieldNumber;
    }

    public void PostRaceProcedure()

    {
        preRacePanel.SetActive(false);
        fields[pendingField].interactable = false;
        fields[pendingField].GetComponentInChildren<TMP_Text>().SetText("");
        postRacePanel.SetActive(true);

    }

    public void SetRaceWinner()

    {
        roundWinner = winnerSelect.value;
        Debug.Log("Winner is " + roundWinner);

    }

    public void WinnerGetsPoint()

    {
        switch (roundWinner)
        {
            case 0:
                MainManager.roundsWonP1++;
                //Debug.Log("P1 has won " + MainManager.roundsWonP1);
                break;

            case 1:
                MainManager.roundsWonP2++;
                //Debug.Log("P2 has won " + MainManager.roundsWonP2);
                break;
        }
    }


    public void ConcludeRound()

    {
        postRacePanel.SetActive(false);

        fields[pendingField].image.sprite = playerSymbols[roundWinner];

        occupiedFields[pendingField] = roundWinner + 1;

        WinnerGetsPoint();

        CheckScore();

        WinnerCheck();

        if (gameEnded != true)

        {
            if (activePlayer == 0)

            { activePlayer = 1; }

            else

            { activePlayer = 0; }

            switch (activePlayer)

            {
                case 0:
                    messageText.text = ($"{MainManager.playerName[0]}, select your field.");
                    break;


                case 1:
                    messageText.text = ($"{MainManager.playerName[1]}, select your field.");
                    break;
            }
        }

    }

    public void CheckScore()

    {
        if (MainManager.roundsWonP1 >= 2)
        {
            jokerP1.gameObject.SetActive(true);
        }

        if (MainManager.roundsWonP2 >= 2)
        {
            jokerP2.gameObject.SetActive(true);
        }
    }

    public void WinnerCheck()

    {
        int s1 = occupiedFields[0] + occupiedFields[1] + occupiedFields[2];
        int s2 = occupiedFields[3] + occupiedFields[4] + occupiedFields[5];
        int s3 = occupiedFields[6] + occupiedFields[7] + occupiedFields[8];
        int s4 = occupiedFields[0] + occupiedFields[3] + occupiedFields[6];
        int s5 = occupiedFields[1] + occupiedFields[4] + occupiedFields[7];
        int s6 = occupiedFields[2] + occupiedFields[5] + occupiedFields[8];
        int s7 = occupiedFields[0] + occupiedFields[4] + occupiedFields[8];
        int s8 = occupiedFields[6] + occupiedFields[4] + occupiedFields[2];

        var solutions = new int[] { s1, s2, s3, s4, s5, s6, s7, s8 };

        for (int i = 0; i < solutions.Length; i++)
        {
            if (solutions[i] == 3)
            {
                Debug.Log("Player 1 is the winner!");
                P1Finish();
                gameEnded = true;
            }

            else if (solutions[i] == 6)

            {
                Debug.Log("Player 2 is the winner!");
                P2Finish();
                gameEnded = true;
            }

        }
    }


    void P1Finish()


    {
        messageText.text = ($"Tic Tac Toy! {MainManager.playerName[0]} wins the match.");

        for (int i = 0; i < fields.Length; i++)

        {
            fields[i].interactable = false;
        }


    }

    void P2Finish()

    {
        messageText.text = ($"Tic Tac Toy! {MainManager.playerName[1]} wins the match.");

        for (int i = 0; i < fields.Length; i++)

        {
            fields[i].interactable = false;
        }


    }


}
