using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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


    [SerializeField] GameObject carPanel;
    [SerializeField] Button carMenuToggleButton;
    [SerializeField] Button panelClose;


    int firstNumber;
    int secondNumber;

    List<string> trackList = new List<string>

    {
        "School's Out 1",
        "Museum 1","Toy World Aquatica: Redux","Ghost Town 1","Rooftops 1","Rooftops","Castle 1","HMS Invincible Redux","Aspenside",
        "Ranch","Airport","Fairground 1","Port Limano 2","StadVolt","Toytanic 2","Casino RV","Supermarket 1","Biohazard Factory","Toys In The Hood 2",
        "Toy World Mayhem","Smashride Circuit","RV Temple","Meltdown","Petro Volt","Botanical Garden ","Mysterious Toy-Volt Factory 1","Snowland 1","Home 2",
        "Subway 2","School's Out 2","Moon Dawn","Radioactive Garden","Toy World 1","Holiday Camp California Edition","ToySoldierz",
        "Santorini","Kadish Sprint","The Great Silence","Spa-Volt 1","Lunar","Skating Toys Redux","Museum EX","Library","Sakura","Hospital 2","Museum 3",
        "Home 1","Rooftop Chase Redux","Hospital 1","Game Room 2","Venice","Quake!","Metro-Volt","urbanX","Toytanic 1","Snowy River","Toy World 3",
        "Game Room 1","Botanical Garden EX","Helios","Route-77","Castle 2","Urban Sprint 1","Wonderful Skylands 1","Fairground 2",
        "Supermarket 2","White Rose Chapel","Grisville","Spaceship","Images Of Giza: Redux","Toy World 2","Toys in The Hood 1","The Bunker","Spa-Volt 2",
        "Medieval Redux","Port Limano 1","SBX Alpine","Jailhouse Rock","Ghost Town 2","Museum 2","Desolate District 1","Downtown 1","Downtown 2","Port Limano EX","Elementary 1","Elementary 2","Genghis Kastle","Swan Street",
        "Spring Visit","Aquarium 1","Frostpeak","Galaxy World 1","Galaxy World 2"
    };

    List<T> GetUniqueRandomElements<T>(List<T> inputList, int count)

    {
        List<T> inputListClone = new List<T>(inputList);
        Shuffle(inputListClone);
        return inputListClone.GetRange(0, count);

    }

    void Shuffle<T>(List<T> inputList)

    {
        for (int i = 0; i < inputList.Count - 1; i++)

        {
            T temp = inputList[i];
            int rand = Random.Range(i, inputList.Count);
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


    void SetStar()
    {
        int starField = Random.Range(0, 9);
        gameManagerScript.fields[starField].image.sprite = starSymbol;
        gameManagerScript.fieldText[starField].text = "?";

    }


    public void SetColors()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                int rand = Random.Range(0, 9);
                gameManagerScript.fields[rand].GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0);
                firstNumber = rand;
            }
            if (i == 1)

            {
                int rand2 = Random.Range(0, 9);
                while (rand2 == firstNumber)
                { rand2 = Random.Range(0, 9); }
                gameManagerScript.fields[rand2].GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0);
                secondNumber = rand2;
            }

            if (i == 2)

            {
                int rand3 = Random.Range(0, 9);
                while (rand3 == firstNumber || rand3 == secondNumber)

                { rand3 = Random.Range(0, 9); }
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



