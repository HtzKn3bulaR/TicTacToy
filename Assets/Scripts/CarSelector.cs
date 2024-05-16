using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarSelector : MonoBehaviour
{
    public TMP_Dropdown classMenu;
    public TextMeshProUGUI carClassText;
    public TextMeshProUGUI car1Text;
    public TextMeshProUGUI car2Text;
    public TextMeshProUGUI car3Text;
    public TextMeshProUGUI car4Text;
    public GameObject carClassMenu;
    public GameObject carTable;
    private int classSelected;

    [SerializeField] string[] carClasses = { "Rookie", "Amateur", "Advanced", "Semi-Pro", "Pro", "Super-Pro" };

    List<string> rookieList = new List<string>

    {
        "Toukka 4x4","Starfire GT","Lancer","El Gekko","Condor GRV","Junky","Rouge","Get Air","BigVolt","Road Star","Sunset Light","Show-Off","Nimbus","Harvester","Rebound 4x4","Albatross GT",
        "Updraft","Chubble","El Rapido","Vaanbus","Kanberra Kruiser","Blobster","Col. Moss","Angus 400","Nesbitt","Hot Spot","Micro","Phat Slug","Hurricane","LR 64","Super Wheat","Dust Mite","High-Rod",
        "Crazy Pat","Myrmech","Mr. Bedtime","Tesla","Funziona","Phat Trucker","Splat","Panorama","Ciagnik","Genghis Kar","Quaqa Turbo","Volken Turbo","HSF-1","Pipsqueak","Naranja Turbo","RC Phink","E-Razr"
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

    public void CarSelect()

    {
        var uniqueRandomList = GetUniqueRandomElements(rookieList, 4);

        for (int i = 0; i < uniqueRandomList.Count; i++)

        {
            MainManager.cars[i] = uniqueRandomList[i];
                     
                    }

        car1Text.gameObject.SetActive(true);
        car1Text.text = MainManager.cars[0];

        car2Text.gameObject.SetActive(true);
        car2Text.text = MainManager.cars[1];
    }




}
