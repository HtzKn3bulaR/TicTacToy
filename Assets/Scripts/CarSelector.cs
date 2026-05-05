using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarSelector : MonoBehaviour
{
    public TMP_Dropdown classMenu;
    [SerializeField] Button helpClose;
    [SerializeField] Button helpOpen;
    public TextMeshProUGUI carClassText;
    public TextMeshProUGUI car1Text;
    public TextMeshProUGUI car2Text;
    public TextMeshProUGUI car3Text;
    public TextMeshProUGUI car4Text;
    public GameObject carClassMenu;
    public GameObject carTable;
    [SerializeField] GameObject helpScreen;
    private int classSelected;
    public Button goToNameSelectScreen;
    [SerializeField] Button carClassConfirm;
    List<string> activeList;

    [SerializeField] private TextAsset rookieNames;
    [SerializeField] private TextAsset amateurNames;
    [SerializeField] private TextAsset advancedNames;
    [SerializeField] private TextAsset semiProNames;
    [SerializeField] private TextAsset proNames;
    [SerializeField] private TextAsset superProNames;

    [SerializeField] string[] carClasses = { "Rookie", "Amateur", "Advanced", "Semi-Pro", "Pro", "Super-Pro" };

    

    List<T> GetUniqueRandomElements<T>(List<T> inputList, int count)

    {
        List<T> inputListClone = new List<T>(inputList);
        Shuffle(inputListClone);
        return inputListClone.GetRange(0, count);

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



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClassSelect()

    {
        carClassMenu.SetActive(false);
        carTable.SetActive(true);
        classSelected = (classMenu.value)-1;
        carClassText.text = carClasses[classSelected];
        MainManager.carClass = carClasses[classSelected];

    }

    public List<string> ReadCarList(TextAsset file)
    {
        List<string> temporaryCarList = new List<string>();

        string[] temporaryData = file.text.Split(new string[] { "\n" }, StringSplitOptions.None);

        int carCount = temporaryData.Length;

        string nameTrimmed;

        foreach (string s in temporaryData)
        {
            nameTrimmed = s.TrimEnd(new char[] { '\r', ' ' });
            nameTrimmed = nameTrimmed.TrimStart(new char[] { '\r', ' ' });
            temporaryCarList.Add(nameTrimmed);
        }

        return temporaryCarList;
    }

    public void CarSelect()


    {
        switch (classSelected)

        {
            case 0:
                activeList = ReadCarList(rookieNames);
                break;

            case 1:
                activeList = ReadCarList(amateurNames);
                break;

            case 2:
                activeList = ReadCarList(advancedNames);
                break;

            case 3:
                activeList = ReadCarList(semiProNames);
                break;

            case 4:
                activeList = ReadCarList(proNames);
                break;

            case 5:
                activeList = ReadCarList(superProNames);
                break;


        }

        var uniqueRandomList = GetUniqueRandomElements(activeList, 4);

        for (int i = 0; i < uniqueRandomList.Count; i++)

        {
            MainManager.cars[i] = uniqueRandomList[i];
                     
                    }

        car1Text.gameObject.SetActive(true);
        car1Text.text = MainManager.cars[0];

        car2Text.gameObject.SetActive(true);
        car2Text.text = MainManager.cars[1];

        goToNameSelectScreen.gameObject.SetActive(true);
        carClassConfirm.gameObject.SetActive(false);
    }

    public void CloseHelp()

    {
        helpScreen.gameObject.SetActive(false);
        helpOpen.gameObject.SetActive(true);

    }

    public void OpenHelp()
    {
        helpScreen.gameObject.SetActive(true);
        helpOpen.gameObject.SetActive(false);

    }

           
    public void BackToStart()

    {
        SceneManager.LoadScene(0);

    }
    


    public void GoToNextScreen()

    {
        if(MainManager.Instance.gameIsRemote)
        {
            SceneManager.LoadScene(5);
        }
        else
            SceneManager.LoadScene(2);

    }




}
