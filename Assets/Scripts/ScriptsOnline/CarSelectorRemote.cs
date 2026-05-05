using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Collections;
using System;

public class CarSelectorRemote : NetworkBehaviour
{
    public static CarSelectorRemote Instance;

    public TMP_Dropdown classMenu;
    public TextMeshProUGUI carClassText;
    public TextMeshProUGUI car1Text;
    public TextMeshProUGUI car2Text;
    public TextMeshProUGUI car3Text;
    public TextMeshProUGUI car4Text;
    public GameObject carClassMenu;
    public GameObject carTable;
    private int classSelected = 9;
    [SerializeField] Button carClassConfirm;
    List<string> activeList;

    [SerializeField] private TextAsset rookieNames;
    [SerializeField] private TextAsset amateurNames;
    [SerializeField] private TextAsset advancedNames;
    [SerializeField] private TextAsset semiProNames;
    [SerializeField] private TextAsset proNames;
    [SerializeField] private TextAsset superProNames;

    public GameObject parserConfigPanel;

    public TextMeshProUGUI warningText;

    public NetworkVariable<FixedString32Bytes> className = new NetworkVariable<FixedString32Bytes>();

    public NetworkVariable<FixedString32Bytes> nameCar1 = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> nameCar2 = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> nameCar3 = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> nameCar4 = new NetworkVariable<FixedString32Bytes>();

    public static EventHandler OnServerSetupCompleted;

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
        Instance = this;

        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;

    }

    public void ToggleParserConfigPanel(bool state)
    {
        if (state == false)
        {
            parserConfigPanel.gameObject.SetActive(false);

        }
        else { parserConfigPanel.gameObject.SetActive(true); }
    }


    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.LocalClientId == 1)
        {
            PopulateCarTable(2);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckCarClassIsValid()
    {
        if (classSelected == 9)
        {
            warningText.gameObject.SetActive(true);
            return;
        }

        else
        {
            ConcludeCarSelection();
        }
    }

    public void ClassSelect()

    {
        carTable.SetActive(true);
        classSelected = (classMenu.value) - 1;
        carClassText.text = carClasses[classSelected];
        MainManager.carClass = carClasses[classSelected];
        className.Value = carClasses[classSelected];

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
        
        nameCar1.Value = MainManager.cars[0];
        nameCar2.Value = MainManager.cars[1];
        nameCar3.Value = MainManager.cars[2];
        nameCar4.Value = MainManager.cars[3];
        
        PopulateCarTable(2);

        carClassConfirm.gameObject.SetActive(true);
    }

    public void PopulateCarTable(int carsUnlocked)
    {
        MainManager.carClass = className.Value.ToString();
        carClassText.text = className.Value.ToString();

        car1Text.gameObject.SetActive(true);
        MainManager.cars[0] = nameCar1.Value.ToString();
        car1Text.text = nameCar1.Value.ToString();

        car2Text.gameObject.SetActive(true);
        MainManager.cars[1] = nameCar2.Value.ToString();
        car2Text.text = nameCar2.Value.ToString();

        MainManager.cars[2] = nameCar3.Value.ToString();
        MainManager.cars[3] = nameCar4.Value.ToString();

        if (carsUnlocked > 2) 
        {
            car3Text.gameObject.SetActive(true);
            car3Text.text = nameCar3.Value.ToString();
        }

        if (carsUnlocked > 3)
        {
            car4Text.gameObject.SetActive(true);
            car4Text.text = nameCar4.Value.ToString();
        }

    }

    public void ConcludeCarSelection()
    {
        carTable.gameObject.SetActive(false);
        carClassMenu.gameObject.SetActive(false);

        OnServerSetupCompleted?.Invoke(this, EventArgs.Empty);
    }

    public void HideCarPanel()
    {
        carTable.gameObject.SetActive(false);
    }
                 

    public void CarSelectionHost()
    {
        carClassMenu.gameObject.SetActive(true);
        
    }
}
