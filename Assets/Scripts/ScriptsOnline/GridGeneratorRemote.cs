using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GridGeneratorRemote : NetworkBehaviour
{
    private GameManagerOnline gameManagerScript;
    [SerializeField] private Sprite starSymbol;
    public TextMeshProUGUI[] fieldText;

    [SerializeField] private TextMeshProUGUI waitingPanelMessage;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    [SerializeField] GameObject waitingToStartPanel;

    [SerializeField] TextMeshProUGUI carClass;
    [SerializeField] TextMeshProUGUI carA;
    [SerializeField] TextMeshProUGUI carB;
    [SerializeField] TextMeshProUGUI carC;
    [SerializeField] TextMeshProUGUI carD;

    [SerializeField] Button jokerX;
    [SerializeField] Button jokerO;

    public TextMeshProUGUI jokerP1;
    public TextMeshProUGUI jokerP2;


    [SerializeField] GameObject carPanel;
    [SerializeField] Button carMenuToggleButton;
    [SerializeField] Button panelClose;

    private NetworkVariable<int> carsUnlocked = new NetworkVariable<int>(2);
    
    private NetworkVariable<int> starField = new NetworkVariable<int>();

    private NetworkVariable<int> rand = new NetworkVariable<int>();
    private NetworkVariable<int> rand2 = new NetworkVariable<int>();
    private NetworkVariable<int> rand3 = new NetworkVariable<int>();

    NetworkList<FixedString32Bytes> tracksThisGame; 

    List<FixedString32Bytes> trackList = new List<FixedString32Bytes>

    {
        "School's Out 1",
        "Museum 1","Toy World Aquatica: Redux","Ghost Town 1","Rooftops 1","Rooftops","Castle 1","HMS Invincible Redux","Aspenside",
        "Ranch","Airport","Fairground 1","Port Limano 2","StadVolt","Toytanic 2","Casino RV","Supermarket 1","Biohazard Factory","Toys In The Hood 2",
        "Toy World Mayhem","Smashride Circuit","RV Temple","Meltdown","Petro Volt","Botanical Garden ","Mysterious Toy-Volt Factory 1","Snowland 1","Home 2",
        "Subway 2","School's Out 2","Moon Dawn","Radioactive Garden","Toy World 1","Holiday Camp California Ed","ToySoldierz",
        "Santorini","Kadish Sprint","The Great Silence","Spa-Volt 1","Lunar","Skating Toys Redux","Museum EX","Library","Sakura","Hospital 2","Museum 3",
        "Home 1","Rooftop Chase Redux","Hospital 1","Game Room 2","Venice","Quake!","Metro-Volt","urbanX","Toytanic 1","Snowy River","Toy World 3",
        "Game Room 1","Botanical Garden EX","Helios","Route-77","Castle 2","Urban Sprint 1","Wonderful Skylands 1","Fairground 2",
        "Supermarket 2","White Rose Chapel","Grisville","Spaceship","Images Of Giza: Redux","Toy World 2","Toys in The Hood 1","The Bunker","Spa-Volt 2",
        "Medieval Redux","Port Limano 1","SBX Alpine","Jailhouse Rock","Ghost Town 2","Museum 2","Desolate District 1","Downtown 1","Downtown 2","Port Limano EX","Elementary 1","Elementary 2","Genghis Kastle","Swan Street",
        "Spring Visit","Aquarium 1","Frostpeak","Galaxy World 1","Galaxy World 2","Office 1","Radio Kootwijk","s4","Crystal caves"
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
            int rand = UnityEngine.Random.Range(i, inputList.Count);
            inputList[i] = inputList[rand];
            inputList[rand] = temp;
        }
    }

    private void Awake()
    {
       tracksThisGame = new NetworkList<FixedString32Bytes>();
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;

        carsUnlocked.OnValueChanged += OnCarsUnlockedChangedRpc;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void OnCarsUnlockedChangedRpc(int previousValue, int newValue)
    {
        CarSelectorRemote.Instance.PopulateCarTable(newValue);
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                GenerateGrid();

                waitingPanelMessage.text = "Client connected. Click Ready to start";

                readyButton.gameObject.SetActive(true);                
                InviteClientRpc();
            }
        }
    }

    [ClientRpc]
    public void InviteClientRpc()
    {
        readyButton.gameObject.SetActive(true);
        
    }

    public void ClientPressedReadyButton()
    {
        if (NetworkManager.Singleton.LocalClientId == 1)
        {
            GenerateGrid();
        }
    }


    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        waitingPanelMessage.text = "Waiting for Client connection...";
        startHostButton.gameObject.SetActive(false);
        startClientButton.gameObject.SetActive(false);
        
        
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        waitingPanelMessage.text = "Connected to Host. Click Ready to start.";
        startHostButton.gameObject.SetActive(false);
        startClientButton.gameObject.SetActive(false);
        
    }

    public void HideWaitingPanel()
    {
        waitingToStartPanel.gameObject.SetActive(false);
    }


    public void GenerateGrid()
    {
        
        if (NetworkManager.Singleton.LocalClientId == 0)
        {GenerateRandomElements(); }
        
        SetTracks();

        SetStar();

        SetColors();

        SetupCarTable();

        readyButton.gameObject.SetActive(false);
        
    }
            
    public void GenerateRandomElements()
    {
        
            var uniqueRandomList = GetUniqueRandomElements(trackList, 9);

            for (int i = 0; i < uniqueRandomList.Count; i++)
            {
                tracksThisGame.Add(uniqueRandomList[i].Value);
            }

            starField.Value = UnityEngine.Random.Range(0, 9);
            Debug.Log("Star Field is " + starField.Value);


            rand.Value = UnityEngine.Random.Range(0, 9);

            rand2.Value = UnityEngine.Random.Range(0, 9);
            while (rand2.Value == rand.Value)
            { rand2.Value = UnityEngine.Random.Range(0, 9); }

            rand3.Value = UnityEngine.Random.Range(0, 9);
            while (rand3.Value == rand.Value || rand3.Value == rand2.Value)
            { rand3.Value = UnityEngine.Random.Range(0, 9); }
        

    }

    
    public void SetTracks()
    {
       gameManagerScript = GameObject.Find("GameManagerOnline").GetComponent<GameManagerOnline>();


        for (int i = 0; i < tracksThisGame.Count; i++)
        {
            Debug.Log("Tracks in List " + tracksThisGame.Count);
            Debug.Log("Index " + i);
            gameManagerScript.fieldText[i].text = tracksThisGame[i].Value;
        }
    }

   
    void SetStar()
    {
        Debug.Log("Star Field is " + starField.Value);
        gameManagerScript.fields[starField.Value].image.sprite = starSymbol;
        gameManagerScript.fieldText[starField.Value].text = "?";
    }

    
    public void SetColors()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                gameManagerScript.fields[rand.Value].GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0);
                
            }
            if (i == 1)

            {
                
                gameManagerScript.fields[rand2.Value].GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0);
                
            }

            if (i == 2)

            {
                
                gameManagerScript.fields[rand3.Value].GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0);
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
            if (NetworkManager.Singleton.LocalClientId == 0)
            {
                jokerX.gameObject.SetActive(true);
            }
        }

        if (MainManager.p2HasJoker)
        {
            if (NetworkManager.Singleton.LocalClientId == 1)
            {
                jokerO.gameObject.SetActive(true);
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void UnlockCarRpc()
    {
        carsUnlocked.Value++;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void DeactivateJokerSymbolRpc(int index)
    {
        switch (index)
        {
            case 0:
                jokerP1.gameObject.SetActive(false);
                MainManager.p1HasJoker = false;
                MainManager.xJokerWasUsed = true;
                break;
            case 1:
                jokerP2.gameObject.SetActive(false);
                MainManager.p2HasJoker = false;
                MainManager.oJokerWasUsed = true;
                break;
        }
    }


    public void UnlockNextCarByX()
    {
        if (carsUnlocked.Value < 3)

        {
            MainManager.carCIsActive = true;
            carC.text = MainManager.cars[2];
            carC.gameObject.SetActive(true);
            MainManager.p1HasJoker = false;
            MainManager.xJokerWasUsed = true;
            jokerX.gameObject.SetActive(false);

            DeactivateJokerSymbolRpc(0);
            UnlockCarRpc();
            

            return;
        }

        if (carsUnlocked.Value == 3)

        {
            MainManager.carDIsActive = true;
            carD.text = MainManager.cars[3];
            carD.gameObject.SetActive(true);
            MainManager.p1HasJoker = false;
            MainManager.xJokerWasUsed = true;
            jokerX.gameObject.SetActive(false);

            DeactivateJokerSymbolRpc(0);
            UnlockCarRpc();

            return;

        }

    }

    public void UnlockNextCarByO()
    {
        if (carsUnlocked.Value < 3)

        {
            MainManager.carCIsActive = true;
            carC.text = MainManager.cars[2];
            carC.gameObject.SetActive(true);
            MainManager.p2HasJoker = false;
            MainManager.oJokerWasUsed = true;
            jokerO.gameObject.SetActive(false);

            DeactivateJokerSymbolRpc(1);
            UnlockCarRpc();

            return;
        }

        if (carsUnlocked.Value == 3)

        {
            MainManager.carDIsActive = true;
            carD.text = MainManager.cars[3];
            carD.gameObject.SetActive(true);
            MainManager.p2HasJoker = false;
            MainManager.oJokerWasUsed = true;
            jokerO.gameObject.SetActive(false);

            DeactivateJokerSymbolRpc(1);
            UnlockCarRpc();

            return;

        }


    }


}



