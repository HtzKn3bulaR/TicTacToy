using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CarSelectionHandler : NetworkBehaviour
{

    public static CarSelectionHandler instance;

    [SerializeField] private GameObject carSelectionPanel;

    [SerializeField] private Button[] selectionButtonsX;
    [SerializeField] private Button[] selectionButtonsO;

    [SerializeField] private Button closeButton;
    [SerializeField] private Button continueButton;

    [SerializeField] private Button jokerXButton;
    [SerializeField] private Button jokerOButton;

    [SerializeField] private TextMeshProUGUI carNameTextX;
    [SerializeField] private TextMeshProUGUI carNameTextO;

    [SerializeField] private TextMeshProUGUI carSelectionInfoText;

    private int fieldSelected = 99;

    public NetworkVariable<int> carIndexSelectedByActivePlayer = new NetworkVariable<int>(9);
    public NetworkVariable<int> carIndexSelectedBySecondPlayer = new NetworkVariable<int>(8);

    // Start is called before the first frame update
    void Start()
    {

        instance = this;
        GameManagerOnline.Instance.selectedField.OnValueChanged += OfferCarSelectionOptions;

        carIndexSelectedByActivePlayer.OnValueChanged += SetUnusedCarButtonsInactiveRpc;
        carIndexSelectedByActivePlayer.OnValueChanged += CarSelectionInactivePlayerRpc;
        carIndexSelectedBySecondPlayer.OnValueChanged += SetUnusedCarButtonsInactiveForSecondPlayerRpc;
        carIndexSelectedBySecondPlayer.OnValueChanged += ShowInactivePlayerSelectionRpc;

        GameManagerOnline.OnRoundConcluded += GameManagerOnline_OnRoundConcluded;
                
    }
           

    public void ShowFlashActive()
    {
        Debug.Log("Showing Flash Active");

        carSelectionPanel.gameObject.SetActive(true);

        carSelectionInfoText.text = "Use Joker Or Continue?";
        closeButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);

        if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Cross)
        {
            jokerXButton.gameObject.SetActive(true);
        }
        if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Circle)
        {
            jokerOButton.gameObject.SetActive(true);
        }
    }

    public void ShowFlashInactive()
    {
        Debug.Log("Showing Flash Inactive");
        carSelectionPanel.gameObject.SetActive(true);             
        carSelectionInfoText.text = "Opponent Has Joker";        
    }

    private void GameManagerOnline_OnRoundConcluded()
    {
        ResetCarIndicesRpc();
        ResetCarTableRpc();
     }

    public void SetAllCarSelectionButtonsInactive()
    {
        for (int i = 0; i < selectionButtonsX.Length; i++)
        {
            selectionButtonsX[i].gameObject.SetActive(false);
            selectionButtonsO[i].gameObject.SetActive(false);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ShowInactivePlayerSelectionRpc(int previousValue, int currentValue)
    {
        if (currentValue == 8)
            return;


        if (GameManagerOnline.Instance.activePlayer.Value == GameManagerOnline.Instance.localPlayerType)
        {
            if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Cross)
            {
                carNameTextO.gameObject.SetActive(true);
                carNameTextO.text = MainManager.cars[currentValue];
                selectionButtonsO[currentValue].gameObject.SetActive(true);
                selectionButtonsO[currentValue].interactable = false;
            }

            else
            {
                carNameTextX.gameObject.SetActive(true);
                carNameTextX.text = MainManager.cars[currentValue];
                selectionButtonsX[currentValue].gameObject.SetActive(true);
                selectionButtonsX[currentValue].interactable = false;
            }
        }

        if (GameManagerOnline.Instance.activePlayer.Value != GameManagerOnline.Instance.localPlayerType)
        {
            if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Cross)
            {
                carNameTextX.gameObject.SetActive(true);
                carNameTextX.text = MainManager.cars[currentValue];
                selectionButtonsX[currentValue].gameObject.SetActive(true);
                selectionButtonsX[currentValue].interactable = false;
            }

            else
            {
                carNameTextO.gameObject.SetActive(true);
                carNameTextO.text = MainManager.cars[currentValue];
                selectionButtonsO[currentValue].gameObject.SetActive(true);
                selectionButtonsO[currentValue].interactable = false;
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void CarSelectionInactivePlayerRpc(int previousValue, int newValue)
    {
        if(newValue == 9) 
            return;

       
            Debug.Log("Field Type Is " + GameManagerOnline.Instance.fields[GameManagerOnline.Instance.selectedField.Value].GetComponent<FieldTypeHandler>().ReturnFieldType());
        

        if(GameManagerOnline.Instance.activePlayer.Value != GameManagerOnline.Instance.localPlayerType)
        {

            carSelectionInfoText.text = "Choose Your Car";

            if (GameManagerOnline.Instance.fields[GameManagerOnline.Instance.selectedField.Value].GetComponent<FieldTypeHandler>().ReturnFieldType() == FieldTypeHandler.FieldType.White)
            {
                if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Cross)
                {

                    selectionButtonsO[newValue].gameObject.SetActive(true);
                    selectionButtonsO[newValue].interactable = false;
                    carNameTextO.gameObject.SetActive(true);
                    carNameTextO.text = MainManager.cars[newValue];

                    switch (GridGeneratorRemote.Instance.carsUnlocked.Value)
                    {
                        case 2:                            
                                selectionButtonsX[0].gameObject.SetActive(true);
                                selectionButtonsX[1].gameObject.SetActive(true);
                                selectionButtonsX[newValue].gameObject.SetActive(false);                            
                            break;

                        case 3:                            
                                selectionButtonsX[0].gameObject.SetActive(true);
                                selectionButtonsX[1].gameObject.SetActive(true);
                                selectionButtonsX[2].gameObject.SetActive(true);
                                selectionButtonsX[newValue].gameObject.SetActive(false);                            
                            break;

                        case 4:                           
                                selectionButtonsX[0].gameObject.SetActive(true);
                                selectionButtonsX[1].gameObject.SetActive(true);
                                selectionButtonsX[2].gameObject.SetActive(true);
                                selectionButtonsX[3].gameObject.SetActive(true);
                                selectionButtonsX[newValue].gameObject.SetActive(false);                            
                            break;
                    }
                }

                if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Circle)
                {
                    Debug.Log("Car Value is " + newValue);
                    selectionButtonsX[newValue].gameObject.SetActive(true);
                    selectionButtonsX[newValue].interactable = false;
                    carNameTextX.gameObject.SetActive(true);
                    carNameTextX.text = MainManager.cars[newValue];

                    switch (GridGeneratorRemote.Instance.carsUnlocked.Value)
                    {
                        case 2:
                            selectionButtonsO[0].gameObject.SetActive(true);
                            selectionButtonsO[1].gameObject.SetActive(true);
                            selectionButtonsO[newValue].gameObject.SetActive(false);
                            break;

                        case 3:
                            selectionButtonsO[0].gameObject.SetActive(true);
                            selectionButtonsO[1].gameObject.SetActive(true);
                            selectionButtonsO[2].gameObject.SetActive(true);
                            selectionButtonsO[newValue].gameObject.SetActive(false);
                            break;

                        case 4:
                            selectionButtonsO[0].gameObject.SetActive(true);
                            selectionButtonsO[1].gameObject.SetActive(true);
                            selectionButtonsO[2].gameObject.SetActive(true);
                            selectionButtonsO[3].gameObject.SetActive(true);
                            selectionButtonsO[newValue].gameObject.SetActive(false);
                            break;
                    }
                }
            }

            if (GameManagerOnline.Instance.fields[GameManagerOnline.Instance.selectedField.Value].GetComponent<FieldTypeHandler>().ReturnFieldType() == FieldTypeHandler.FieldType.Red)
            {
                if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Cross)
                {
                    selectionButtonsO[newValue].gameObject.SetActive(true);
                    selectionButtonsO[newValue].interactable = false;
                    carNameTextO.gameObject.SetActive(true);
                    carNameTextO.text = MainManager.cars[newValue];

                    switch (GridGeneratorRemote.Instance.carsUnlocked.Value)
                    {
                        case 2:
                            selectionButtonsX[0].gameObject.SetActive(true);
                            selectionButtonsX[1].gameObject.SetActive(true);
                            break;

                        case 3:
                            selectionButtonsX[0].gameObject.SetActive(true);
                            selectionButtonsX[1].gameObject.SetActive(true);
                            selectionButtonsX[2].gameObject.SetActive(true);
                            break;

                        case 4:
                            selectionButtonsX[0].gameObject.SetActive(true);
                            selectionButtonsX[1].gameObject.SetActive(true);
                            selectionButtonsX[2].gameObject.SetActive(true);
                            selectionButtonsX[3].gameObject.SetActive(true);
                            break;
                    }
                }

                if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Circle)
                {
                    selectionButtonsX[newValue].gameObject.SetActive(true);
                    selectionButtonsX[newValue].interactable = false;
                    carNameTextX.gameObject.SetActive(true);
                    carNameTextX.text = MainManager.cars[newValue];

                    switch (GridGeneratorRemote.Instance.carsUnlocked.Value)
                    {
                        case 2:
                            selectionButtonsO[0].gameObject.SetActive(true);
                            selectionButtonsO[1].gameObject.SetActive(true);
                            
                            break;

                        case 3:
                            selectionButtonsO[0].gameObject.SetActive(true);
                            selectionButtonsO[1].gameObject.SetActive(true);
                            selectionButtonsO[2].gameObject.SetActive(true);
                            
                            break;

                        case 4:
                            selectionButtonsO[0].gameObject.SetActive(true);
                            selectionButtonsO[1].gameObject.SetActive(true);
                            selectionButtonsO[2].gameObject.SetActive(true);
                            selectionButtonsO[3].gameObject.SetActive(true);
                            
                            break;
                    }
                }
            }
        }

        if (GameManagerOnline.Instance.activePlayer.Value == GameManagerOnline.Instance.localPlayerType)
        {
            carSelectionInfoText.text = "Waiting for Opponent";

            if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Cross)
            {
                carNameTextX.gameObject.SetActive(true);
                carNameTextX.text = MainManager.cars[newValue];

            }

            else
            {
                carNameTextO.gameObject.SetActive(true);
                carNameTextO.text = MainManager.cars[newValue];
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void CarSelectedForNextRace(int carIndex)
    {
        if(GameManagerOnline.Instance.activePlayer.Value == GameManagerOnline.Instance.localPlayerType)
        {
            Debug.Log("Car Index " + carIndex);
            SetRaceCarIndexOnServerRpc(carIndex, true);

            if(GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Cross)
            {
                selectionButtonsX[carIndex].interactable = false;
            }
            else
            {
                selectionButtonsO[carIndex].interactable = false;
            }

        }
        else
        {
            SetRaceCarIndexOnServerRpc(carIndex, false);

            if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Cross)
            {
                selectionButtonsX[carIndex].interactable = false;
            }
            else
            {
                selectionButtonsO[carIndex].interactable = false;
            }
        }
        
    }


    [Rpc(SendTo.Server)]
    public void SetRaceCarIndexOnServerRpc (int index, bool IsActive)
    {
        if (IsActive == true)
        {
            Debug.Log("Setting Car index to " +  index);
            carIndexSelectedByActivePlayer.Value = index;

        }

        else
        { 
            carIndexSelectedBySecondPlayer.Value = index;
        }               
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetUnusedCarButtonsInactiveForSecondPlayerRpc (int previousValue, int newValue)
    {
        carSelectionInfoText.text = "";

        if (newValue > 3) { return; }

        if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Cross)
        {
            if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.Instance.activePlayer.Value)
            {
                return;
            }

            else
            {
                for (int i = 0; i < selectionButtonsX.Length; i++)
                {
                    if (i != newValue)
                    {
                        selectionButtonsX[i].gameObject.SetActive(false);
                    }

                }
            }
        }

        if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Circle)
        {
            if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.Instance.activePlayer.Value)
            {
                return;
            }

            else
            {
                for (int i = 0; i < selectionButtonsX.Length; i++)
                {
                    if (i != newValue)
                    {
                        selectionButtonsO[i].gameObject.SetActive(false);
                    }

                }
            }
        }

    }




    [Rpc(SendTo.ClientsAndHost)]
    public void SetUnusedCarButtonsInactiveRpc(int previousValue, int newValue)
    {
        if (newValue > 3) { return; }

        if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Cross)
        {
            if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.Instance.activePlayer.Value)
            {
                for (int i = 0; i < selectionButtonsX.Length; i++)
                {
                    if (i != newValue)
                    {
                        selectionButtonsX[i].gameObject.SetActive(false);
                    }
                                        
                }
            }

            else
            {
                for (int i = 0; i < selectionButtonsX.Length; i++)
                {
                    if (i != newValue)
                    {
                        selectionButtonsX[i].gameObject.SetActive(false);
                    }
                                        
                }
            }
        }

        else
        {
            if (GameManagerOnline.Instance.localPlayerType == GameManagerOnline.Instance.activePlayer.Value)
            {
                for (int i = 0; i < selectionButtonsX.Length; i++)
                {
                    Debug.Log("index is " + i);
                    Debug.Log("Car Index Value selected by Active Player " + carIndexSelectedByActivePlayer.Value);


                    if (i != newValue)
                    {
                        selectionButtonsO[i].gameObject.SetActive(false);
                    }

                    if (i == newValue)
                    {
                        selectionButtonsO[i].interactable = false;
                    }
                }
            }

            else
            {
                return;
            }
        }

    }


    public void OfferCarSelectionOptions(int previousValue, int newValue)
    {
        fieldSelected = newValue;

        carSelectionPanel.SetActive(true);

        if (GameManagerOnline.Instance.activePlayer.Value == GameManagerOnline.Instance.localPlayerType)
        {
            carSelectionInfoText.text = "Choose Your Car";
        }

        else { carSelectionInfoText.text = "Waiting for Opponent"; }


        
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            switch (GridGeneratorRemote.Instance.carsUnlocked.Value)
            {
                case 2:
                    if (GameManagerOnline.Instance.activePlayer.Value == GameManagerOnline.PlayerType.Cross)
                    {
                        selectionButtonsX[0].gameObject.SetActive(true);
                        selectionButtonsX[1].gameObject.SetActive(true);
                    }
                    break;

                case 3:
                    if (GameManagerOnline.Instance.activePlayer.Value == GameManagerOnline.PlayerType.Cross)
                    {
                        selectionButtonsX[0].gameObject.SetActive(true);
                        selectionButtonsX[1].gameObject.SetActive(true);
                        selectionButtonsX[2].gameObject.SetActive(true);
                    }
                    break;

                case 4:
                    if (GameManagerOnline.Instance.activePlayer.Value == GameManagerOnline.PlayerType.Cross)
                    {
                        selectionButtonsX[0].gameObject.SetActive(true);
                        selectionButtonsX[1].gameObject.SetActive(true);
                        selectionButtonsX[2].gameObject.SetActive(true);
                        selectionButtonsX[3].gameObject.SetActive(true);
                    }
                    break;
            }
        }

        else
        {
            switch (GridGeneratorRemote.Instance.carsUnlocked.Value)
            {
                case 2:
                    if (GameManagerOnline.Instance.activePlayer.Value == GameManagerOnline.PlayerType.Circle)
                    {
                        selectionButtonsO[0].gameObject.SetActive(true);
                        selectionButtonsO[1].gameObject.SetActive(true);
                    }
                    break;

                case 3:
                    if (GameManagerOnline.Instance.activePlayer.Value == GameManagerOnline.PlayerType.Circle)
                    {
                        selectionButtonsO[0].gameObject.SetActive(true);
                        selectionButtonsO[1].gameObject.SetActive(true);
                        selectionButtonsO[2].gameObject.SetActive(true);
                    }
                    break;

                case 4:
                    if (GameManagerOnline.Instance.activePlayer.Value == GameManagerOnline.PlayerType.Circle)
                    {
                        selectionButtonsO[0].gameObject.SetActive(true);
                        selectionButtonsO[1].gameObject.SetActive(true);
                        selectionButtonsO[2].gameObject.SetActive(true);
                        selectionButtonsO[3].gameObject.SetActive(true);
                    }
                    break;
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void ResetCarIndicesRpc()
    {
        carIndexSelectedByActivePlayer.Value = 9;
        carIndexSelectedBySecondPlayer.Value = 8;
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void ResetCarTableRpc()
    {
        Debug.Log("Resetting Car Table");

        for (int i = 0; i < selectionButtonsO.Length; i++)
        {
            selectionButtonsX[i].gameObject.SetActive(false);
            selectionButtonsX[i].interactable = true;
            selectionButtonsO[i].gameObject.SetActive(false);
            selectionButtonsO[i].interactable = true;
        }

        carNameTextX.text = "";
        carNameTextO.text = "";

        closeButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(false);

        jokerOButton.gameObject.SetActive(false);
        jokerXButton.gameObject.SetActive(false);

        carSelectionPanel.SetActive(false);
    }



}
