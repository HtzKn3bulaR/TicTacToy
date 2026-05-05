using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManagerOnline : NetworkBehaviour
{
    public static GameManagerOnline Instance;

    public TextMeshProUGUI messageText;
    public TextMeshProUGUI nextTrack;
    public TextMeshProUGUI[] fieldText;
    [SerializeField] private TextMeshProUGUI p1Name;
    [SerializeField] private TextMeshProUGUI p2Name;

    [SerializeField] GameObject blockPanel;

    public NetworkVariable<FixedString32Bytes> serverPlayerName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> clientPlayerName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> matchWinnerName = new NetworkVariable<FixedString32Bytes>("None");
        
    public NetworkVariable<PlayerType> activePlayer = new NetworkVariable<PlayerType>(PlayerType.None);
    public NetworkVariable<PlayerType> roundWinner = new NetworkVariable<PlayerType>(PlayerType.None);

    public NetworkVariable<int> selectedField = new NetworkVariable<int>(99);

    public List<int> fieldsNotPlayed = new List<int>(); 
                
    public Sprite[] playerSymbols;
    public Button[] fields;
    public GameObject preRacePanel;
    public GameObject postRacePanel;
    private int pendingField;
    public TMP_Dropdown winnerSelect;
    public int[] occupiedFields; 
    private NetworkVariable<bool> gameEnded = new NetworkVariable<bool> (false);

    public AudioClip fieldWin;
    public AudioClip matchStart;

    private GameObject audioManager;
    private GameObject mainManager;

    private AudioSource gameplayAudio;

    [SerializeField] Button quit;
    [SerializeField] Button rematch;

    public TextMeshProUGUI jokerP1;
    public TextMeshProUGUI jokerP2;

    public event EventHandler<OnPlayerConnectedEventArgs> OnPlayerConnected;
       
    public static event Action OnRoundConcluded;
    public static event Action OnFieldSelected;
    public static event Action OnFieldTopLeftCornerSelected;
   
        

    public class OnPlayerConnectedEventArgs : EventArgs
    {
        public PlayerType playerType;
    }

    
    public enum PlayerType
    {
        None,
        Cross,
        Circle,
    }

    public PlayerType localPlayerType;

    private void Awake()
    {

        Instance = this;
        Debug.Log("Game Is Remote " + MainManager.Instance.gameIsRemote);
                    
    }


    void Start()
    {

        blockPanel.gameObject.SetActive(true);
        
        occupiedFields = new int[9];

        OnPlayerConnected += GameManagerOnline_OnPlayerConnected;
        activePlayer.OnValueChanged += OnActivePlayerChanged;
        
        OnFieldSelected += GameManagerOnline_OnFieldSelected;
        selectedField.OnValueChanged += MakeSelectedFieldNonInteractableRpc;
        OnFieldTopLeftCornerSelected += GameManagerOnline_OnFieldTopLeftCornerSelected;
        roundWinner.OnValueChanged += SetWinnerSymbol;
        gameEnded.OnValueChanged += ShowEndText;
                        
        FieldSetup();
               
        StopThemeAudio();

                       
    }

    public void ShowPostRacePanel()
    {
        postRacePanel.gameObject.SetActive(true);
    }

    private void GameManagerOnline_OnFieldTopLeftCornerSelected()
    {
        MakeSelectedFieldNonInteractableRpc(9, 0);
    }

    private void ShowEndText(bool previousValue, bool newValue)
    {
        messageText.text = ($"Tic Tac Toy! {matchWinnerName.Value} wins the match.");

        if(matchWinnerName.Value == "None")
        {
            messageText.text = ($"Stalemate. Click below for Rematch or Quit.");
        }
    }

    private void GameManagerOnline_OnFieldSelected()
    {
        Debug.Log("Pending field is " + pendingField);

        SetSelectedFieldRpc(pendingField);                
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void MakeSelectedFieldNonInteractableRpc(int previousValue, int newValue)
    {
        Debug.Log("Field Number is set to " + newValue);
        fields[newValue].interactable = false;

        if(localPlayerType != activePlayer.Value)
        {
            messageText.text = ($"Opponent has chosen {fields[newValue].GetComponentInChildren<TMP_Text>().text}");
        }
    }

    [Rpc(SendTo.Server)]
    public void SetSelectedFieldRpc(int fieldIndex)
    {
        selectedField.Value = fieldIndex;
        Debug.Log("Server set value to field number " + selectedField.Value);

        if (fieldIndex == 0)
        {
            OnFieldTopLeftCornerSelected?.Invoke();
        }
    }

    
    private void OnActivePlayerChanged(PlayerType previousValue, PlayerType newValue)
    {
        if (localPlayerType == previousValue)
        {
            blockPanel.SetActive(true);
        }

        if (localPlayerType == newValue)
        {
            blockPanel.SetActive(false);
        }

        if(!gameEnded.Value)
        { UpdateInfoMessage(newValue);}

    }

    private void GameManagerOnline_OnPlayerConnected(object sender, OnPlayerConnectedEventArgs e)
    {
        NamesSetupRpc(MainManager.multiplayerName, e.playerType);

        if (IsActivePlayer(localPlayerType))
        {
            messageText.text = ($"{MainManager.multiplayerName}, it's your turn. Select a field.");
            blockPanel.SetActive(false);
        }
        else
        {
            messageText.text = ($"It's your opponents turn.");
            blockPanel.SetActive(true);
        }

    }

    public override void OnNetworkSpawn()
    {
        //Debug.Log("Local ID is " + NetworkManager.Singleton.LocalClientId);
        if(NetworkManager.Singleton.LocalClientId == 0 )
        {
            localPlayerType = PlayerType.Cross;
            OnPlayerConnected?.Invoke(this, new OnPlayerConnectedEventArgs { playerType = localPlayerType });
            serverPlayerName.Value = MainManager.multiplayerName;

            //CarSelectorRemote.Instance.CarSelectionHost();
            
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            
        }
        else
        {
            localPlayerType = PlayerType.Circle;
            p1Name.text = serverPlayerName.Value.ToSafeString();
            p2Name.text = MainManager.multiplayerName;
            OnPlayerConnected?.Invoke(this, new OnPlayerConnectedEventArgs { playerType = localPlayerType });

        }

    }

    
    private bool IsActivePlayer(PlayerType playerType)
    {
        if(activePlayer.Value == playerType)
            return true;
        else return false;
    }


    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
       
        if(NetworkManager.Singleton.ConnectedClientsList.Count == 2 )
        {
            int startingPlayer = UnityEngine.Random.Range(0, 2);
            switch (startingPlayer)
            {
                case 0:
                    activePlayer.Value = PlayerType.Cross; 
                    if(localPlayerType == PlayerType.Cross)
                    { messageText.text = ($"{MainManager.multiplayerName}, it's your turn. Select a field.");
                        blockPanel.SetActive(false);
                    }
                    else
                    { messageText.text = ($"It's your opponents turn.");
                        blockPanel.SetActive(true);
                    }
                    break;

                case 1:
                    activePlayer.Value = PlayerType.Circle;
                    if (localPlayerType == PlayerType.Circle)
                    { messageText.text = ($"{MainManager.multiplayerName}, it's your turn. Select a field."); }
                    else
                    { messageText.text = ($"It's your opponents turn."); }
                    break;
            }


        }
    }

    void StopThemeAudio()

    {
        audioManager = GameObject.Find("AudioManager");
        Destroy(audioManager);

        gameplayAudio = GetComponent<AudioSource>();
        gameplayAudio.PlayOneShot(matchStart);

    }

    public void FieldSetup()

    {

        for (int i = 0; i < fields.Length; i++)

        {
            fields[i].interactable = true;
            fields[i].GetComponent<Image>().sprite = null;
        }

        for (int i = 0; i < fields.Length; i++)

        {
            occupiedFields[i] = -100;
        }

    }

    [Rpc(SendTo.Server)]
    public void NamesSetupRpc(string multiplayerName, PlayerType type)

    {if (type == PlayerType.Cross)
        {
            serverPlayerName.Value = multiplayerName;
            Debug.Log("Server Player Network Name is " + serverPlayerName.Value);
            p1Name.text = multiplayerName;
            Debug.Log("Server player name is " + MainManager.multiplayerName);            
        }
        else
        {
            clientPlayerName.Value = multiplayerName;
            p2Name.text = multiplayerName;
            Debug.Log("Client player name is " + MainManager.multiplayerName);            
        }
    }        


    // Update is called once per frame
    void Update()
    {

    }


    public void ButtonClicked(int fieldNumber)
    {
        preRacePanel.SetActive(true);
        string trackSelected = (fieldText[fieldNumber].GetComponentInChildren<TMP_Text>().text);
        nextTrack.text = trackSelected;
        pendingField = fieldNumber;
    }

    
    public void PostRaceProcedure()

    {        
        preRacePanel.SetActive(false);
        SetFieldInactive(fields[pendingField]);

        if(pendingField == GridGeneratorRemote.Instance.starField.Value)
        {
            GridGeneratorRemote.Instance.RevealStarField();
        }

        ShowHostWinnerPanelRpc(pendingField);
        blockPanel.SetActive(true);

    }

    [Rpc(SendTo.Server)]
    public void ShowHostWinnerPanelRpc(int field)
    {
        pendingField = field;

        //postRacePanel.SetActive(true);

        OnFieldSelected?.Invoke();
    }


    private void SetFieldInactive(Button field)
    {
        field.interactable = false;
        field.GetComponentInChildren<TMP_Text>().SetText("");
        
    }


    public void SetRaceWinnerLocal()
    {
        int winnerIndex = winnerSelect.value;
        Debug.Log("Race winner index reported " + winnerIndex);
        SetRaceWinner(winnerIndex);
    }


    
    public void SetRaceWinner(int winnerIndex)
    {
        switch(winnerIndex)
        {
                case 1:
                Debug.Log("Winner Index submitted " + winnerIndex);
                roundWinner.Value = PlayerType.Cross;
                Debug.Log("Winner is " + roundWinner.Value);
                break;
            case 2:
                Debug.Log("Winner Index submitted " + winnerIndex);
                roundWinner.Value = PlayerType.Circle;
                Debug.Log("Winner is " + roundWinner.Value);
                break;
        }
        
    }

    
    public void WinnerGetsPoint()

    {
        if (roundWinner.Value == PlayerType.Cross)
        { MainManager.roundsWonP1++;
            //Debug.Log("P1 has won " + MainManager.roundsWonP1);
        }

        if (roundWinner.Value == PlayerType.Circle)
        {
            MainManager.roundsWonP2++;
            //Debug.Log("P2 has won " + MainManager.roundsWonP2);
        }
                                   
    }


    public void ConcludeRound()

    {                                        
        PassTurnRpc();             

        OnRoundConcluded?.Invoke();
        Debug.Log("Conclude Round Event Invoked");
    }

    
    [Rpc(SendTo.ClientsAndHost)]
    public void SetJokerRpc(PlayerType player)
    {
        if (player == PlayerType.Cross)
        {
            jokerP1.gameObject.SetActive(true);
            MainManager.p1HasJoker = true;
            
        }

        if (player == PlayerType.Circle)
        {
            jokerP2.gameObject.SetActive(true);
            MainManager.p2HasJoker = true;
            
        }

        if (localPlayerType == player)
        {
            Debug.Log("Show Flash Active");
            CarSelectionHandler.instance.ShowFlashActive();
            blockPanel.SetActive(true);
        }

        else
        {
            Debug.Log("Show Flash Inactive");
            CarSelectionHandler.instance.ShowFlashInactive();
            blockPanel.SetActive(true);
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ContinueAfterJokerDecisionRpc()
    {
        

        if(activePlayer.Value == localPlayerType)
        {
            blockPanel.SetActive(false);
        }
    }

    [Rpc(SendTo.Server)]
    public void PassTurnRpc()
    {
        if (!gameEnded.Value)

        {
            if (activePlayer.Value == localPlayerType)

            {
                if (localPlayerType == PlayerType.Cross)
                {
                    activePlayer.Value = PlayerType.Circle;
                }
                else
                {
                    activePlayer.Value = PlayerType.Cross;
                }
            }

            else

            { activePlayer.Value = localPlayerType; }
            
        }
    }

    public void UpdateInfoMessage(PlayerType nextActivePlayer)
    {
        if (gameEnded.Value == false)
        {

            if (nextActivePlayer == localPlayerType)

            {
                messageText.text = ($"{MainManager.multiplayerName}, it's your turn! Select your field.");
            }
            else
            {
                messageText.text = ($"It's your opponents turn.");
            }
        }

        else
        { messageText.text = ($"Tic Tac Toy! {matchWinnerName.Value} wins the match."); }
    }


    
    public void SetWinnerSymbol(PlayerType previousWinner, PlayerType currentWinner)
    {
        Debug.Log("Current winner " + currentWinner);
        Debug.Log("Previous winner " + previousWinner);

        Debug.Log("Event Set Winner Symbol started");

        if (currentWinner == PlayerType.Cross)
        {
            Debug.Log("Setting Symbol on field " + fields[selectedField.Value]);
            fields[selectedField.Value].interactable = false;
            Debug.Log("Field is interactable " + fields[selectedField.Value].interactable);
            fields[selectedField.Value].image.sprite = playerSymbols[0];
            FieldAcquiredSound();
            fields[selectedField.Value].GetComponentInChildren<TMP_Text>().SetText("");
            occupiedFields[pendingField] = 0 + 1;

            if(localPlayerType == PlayerType.Circle)
            {
                ResetWinnerRpc();
            }
        }

        if (currentWinner == PlayerType.Circle)
        {
            Debug.Log("Setting Symbol on field " + fields[selectedField.Value]);
            fields[selectedField.Value].interactable = false;
            Debug.Log("Field is interactable " + fields[selectedField.Value].interactable);
            fields[selectedField.Value].image.sprite = playerSymbols[1];
            FieldAcquiredSound();
            fields[selectedField.Value].GetComponentInChildren<TMP_Text>().SetText("");
            occupiedFields[pendingField] = 1 + 1;

            if (localPlayerType == PlayerType.Circle)
            {
                ResetWinnerRpc();
            }

        }
        
        if (roundWinner.Value == PlayerType.None)
        { Debug.Log("Event Winner Reset Invoked"); }
        
    }

    [Rpc(SendTo.Server)]
    private void ResetWinnerRpc()
    {
        roundWinner.Value = PlayerType.None;
        Debug.Log("round Winner Reset to " +  roundWinner.Value);
    }

    
    public void CheckScore()

    {
        postRacePanel.SetActive(false);

        WinnerGetsPoint();

        WinnerCheck();

        if (GridGeneratorRemote.Instance.carsUnlocked.Value == 4)
        {
            ConcludeRound();
            return;
        }

        else if (MainManager.roundsWonP1 >= 2 && MainManager.xJokerWasUsed == false)
        {
            SetJokerRpc(PlayerType.Cross);
            
        }

        else if (MainManager.roundsWonP1 >= 4 && MainManager.xJokerWasUsed == true)

        {
            SetJokerRpc(PlayerType.Cross);
            
        }

        else if (MainManager.roundsWonP2 >= 2 && MainManager.oJokerWasUsed == false)
        {

            SetJokerRpc(PlayerType.Circle);
            
        }

        else if (MainManager.roundsWonP2 >= 4 && MainManager.oJokerWasUsed == true)

        {
            SetJokerRpc(PlayerType.Circle);
           
        }

        else
            { ConcludeRound(); }
    }

    void PlayTheme()

    {
        gameplayAudio = GetComponent<AudioSource>();
        gameplayAudio.Play();


    }

    void FieldAcquiredSound()

    {
        gameplayAudio = GetComponent<AudioSource>();
        gameplayAudio.PlayOneShot(fieldWin);

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
                matchWinnerName.Value = serverPlayerName.Value;
                gameEnded.Value = true;
                GetFieldsNotPlayed();
                P1Finish();
            }

            else if (solutions[i] == 6)

            {
                Debug.Log("Player 2 is the winner!");
                gameEnded.Value = true;
                matchWinnerName.Value = clientPlayerName.Value;
                GetFieldsNotPlayed();
                P2Finish();
                
            }


        }

        if (gameEnded.Value != true)

        { CheckForDraw(); }

    }

    private void GetFieldsNotPlayed()
    {
        for (int i = 0; i < fields.Length; i++)

        {
            if (occupiedFields[i] == -100)
            { fieldsNotPlayed.Add(i); }
        }

    }


    void CheckForDraw()

    {
        int sum = 0;

        for (int i = 0; i < occupiedFields.Length; i++)

        { sum += occupiedFields[i]; }

        if (sum > -50)

        {
            gameEnded.Value = true;

            GameEndingRoutineRpc("NN");
           

            PlayTheme();
            quit.gameObject.SetActive(true);
            rematch.gameObject.SetActive(true);

        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void DeactivateFieldRpc(int index)
    {
        fields[index].gameObject.SetActive(false);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void GameEndingRoutineRpc(FixedString32Bytes name)
    {
        if (name == "NN")
        {
            messageText.text = ($"Stalemate. Click below for Rematch or Quit.");
        }

        else
        {
            messageText.text = ($"Tic Tac Toy! {name} wins the match.");
        }

        quit.gameObject.SetActive(true);
        rematch.gameObject.SetActive(true);

        PlayTheme();
    }


    void P1Finish()
    {
        Debug.Log("Finish routine started ");

        GameEndingRoutineRpc(serverPlayerName.Value);

        for (int i = 0; i < fields.Length; i++)
        {
            Debug.Log("Field " + i + "contained in List " + fieldsNotPlayed.Contains(i));

            if (fieldsNotPlayed.Contains(i))
            { 
                DeactivateFieldRpc(i);
            }
        }
              
    }

    
    void P2Finish()             
    {
        Debug.Log("Finish routine started");

        GameEndingRoutineRpc(clientPlayerName.Value);

        for (int i = 0; i < fields.Length; i++)
        {
            Debug.Log("Field " + i + "contained in List " + fieldsNotPlayed.Contains(i));
            if (fieldsNotPlayed.Contains(i))
            {
                DeactivateFieldRpc(i);
            }
        }      

    }

    public void RestartGame()

    {
        MainManager.playerName[0] = "Player 1";
        MainManager.playerName[1] = "Player 2";

        MainManager.roundsWonP1 = 0;
        MainManager.roundsWonP2 = 0;

        MainManager.carClass = "Re-Volt";

        MainManager.carCIsActive = false;
        MainManager.carDIsActive = false;

        MainManager.p1HasJoker = false;
        MainManager.p2HasJoker = false;

        MainManager.xJokerWasUsed = false;
        MainManager.oJokerWasUsed = false;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()

    {
        Application.Quit();
    }


}
