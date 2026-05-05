using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    private GameManager gameManagerScript;
    [SerializeField] private Sprite starSymbol;
    public TextMeshProUGUI[] fieldText;

    [SerializeField] TextMeshProUGUI carClass;
    [SerializeField] TextMeshProUGUI carA;
    [SerializeField] TextMeshProUGUI carB;
    [SerializeField] TextMeshProUGUI carC;
    [SerializeField] TextMeshProUGUI carD;

    [SerializeField] Button jokerX;
    [SerializeField] Button jokerO;

    [SerializeField] private TextAsset stockTracks;
    [SerializeField] private TextAsset standardTracks;


    [SerializeField] GameObject carPanel;
    [SerializeField] Button carMenuToggleButton;
    [SerializeField] Button panelClose;

    private List<string> temporaryList = new List<string>();
    private List<string> trackList = new List<string>();


    int firstNumber;
    int secondNumber;
        

    List<T> GetUniqueRandomElements<T>(List<T> inputList, int count)

    {
        List<T> inputListClone = new List<T>(inputList);
        Shuffle(inputListClone);
        return inputListClone.GetRange(0, count);

    }

    private void Awake()
    {
        trackList.AddRange(ReadTrackFile(stockTracks));
        temporaryList.Clear();
        trackList.AddRange(ReadTrackFile(standardTracks));
        temporaryList.Clear();
    }


    void Shuffle<T>(List<T> inputList)

    {
        for (int i = 0; i < inputList.Count; i++)

        {
            T temp = inputList[i];
            int rand = UnityEngine.Random.Range(i, inputList.Count);
            inputList[i] = inputList[rand];
            inputList[rand] = temp;
        }
    }

    void Start()
    {
                
        var uniqueRandomList = GetUniqueRandomElements(trackList, 9);


        
            gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        

        for (int i = 0; i < uniqueRandomList.Count; i++)

        {
            gameManagerScript.fieldText[i].text = uniqueRandomList[i];
        }

        SetStar();

        SetColors();

        SetupCarTable();
    }

    public List<string> ReadTrackFile(TextAsset trackFile)
    {
        string[] trackData = trackFile.text.Split(new string[] { "\n" }, StringSplitOptions.None);

        int tableSize = trackData.Length;
        Debug.Log("Table size " + tableSize);

        for (int i = 0; i < tableSize; i++)
        {
            string nameTrimmed;

            nameTrimmed = trackData[i].TrimEnd(new char[] { '\r', ' ' });
            nameTrimmed = nameTrimmed.TrimStart(new char[] { '\r', ' ' });                      

            temporaryList.Add(nameTrimmed);

        }

        return temporaryList;
    }


    void SetStar()
    {
        int starField = UnityEngine.Random.Range(0, 9);
        gameManagerScript.fields[starField].image.sprite = starSymbol;
        gameManagerScript.fieldText[starField].text = "?";

    }


    public void SetColors()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                int rand = UnityEngine.Random.Range(0, 9);
                gameManagerScript.fields[rand].GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0);
                firstNumber = rand;
            }
            if (i == 1)

            {
                int rand2 = UnityEngine.Random.Range(0, 9);
                while (rand2 == firstNumber)
                { rand2 = UnityEngine.Random.Range(0, 9); }
                gameManagerScript.fields[rand2].GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0);
                secondNumber = rand2;
            }

            if (i == 2)

            {
                int rand3 = UnityEngine.Random.Range(0, 9);
                while (rand3 == firstNumber || rand3 == secondNumber)

                { rand3 = UnityEngine.Random.Range(0, 9); }
                gameManagerScript.fields[rand3].GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0);
            }


        }


    }

    void SetupCarTable()

    {
        carClass.text = MainManager.carClass;
        carA.text = MainManager.cars[0];
        carA.gameObject.SetActive(true);
        carB.text = MainManager.cars[1];
        carB.gameObject.SetActive(true);

    }

    public void ShowCarPanel()

    {
        carPanel.SetActive(true);
        carMenuToggleButton.gameObject.SetActive(false);

        CheckJokerStatus();
    }

    public void HideCarPanel()

    {
        carPanel.SetActive(false);
        carMenuToggleButton.gameObject.SetActive(true);

    }

    void CheckJokerStatus()
    {
        if (MainManager.p1HasJoker)
        {
            jokerX.gameObject.SetActive(true);
        }

        if (MainManager.p2HasJoker)
        {
            jokerO.gameObject.SetActive(true);

        }

    }

    public void UnlockNextCarByX()
    {
        if (MainManager.carCIsActive == false)

        {
            MainManager.carCIsActive = true;
            carC.text = MainManager.cars[2];
            carC.gameObject.SetActive(true);
            MainManager.p1HasJoker = false;
            MainManager.xJokerWasUsed = true;
            gameManagerScript.jokerP1.gameObject.SetActive(false);
            jokerX.gameObject.SetActive(false);
            return;
        }

        if (MainManager.carCIsActive == true)

        {
            MainManager.carDIsActive = true;
            carD.text = MainManager.cars[3];
            carD.gameObject.SetActive(true);
            MainManager.p1HasJoker = false;
            MainManager.xJokerWasUsed = true;
            gameManagerScript.jokerP1.gameObject.SetActive(false);
            jokerX.gameObject.SetActive(false);
            return;

        }

    }

    public void UnlockNextCarByO()
    {
        if (MainManager.carCIsActive == false)

        {
            MainManager.carCIsActive = true;
            carC.text = MainManager.cars[2];
            carC.gameObject.SetActive(true);
            MainManager.p2HasJoker = false;
            MainManager.oJokerWasUsed = true;
            gameManagerScript.jokerP2.gameObject.SetActive(false);
            jokerO.gameObject.SetActive(false);
            return;
        }

        if (MainManager.carCIsActive == true)

        {
            MainManager.carDIsActive = true;
            carD.text = MainManager.cars[3];
            carD.gameObject.SetActive(true);
            MainManager.p2HasJoker = false;
            MainManager.oJokerWasUsed = true;
            gameManagerScript.jokerP2.gameObject.SetActive(false);
            jokerO.gameObject.SetActive(false);
            return;

        }


    }


}



