using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class RacingManager : NetworkBehaviour
{
    [SerializeField] private GameObject racingPanel;
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private Button getResultsButton;

    [SerializeField] private Button acceptResultsButton;
    [SerializeField] private Button overrideResultsButton;

    [SerializeField] private Sprite[] leaderboardPlayerSprites;
    [SerializeField] private GameObject[] leaderboardPlayerIcons;
    [SerializeField] private TextMeshProUGUI[] rankingPlayerNames;
    [SerializeField] private TextMeshProUGUI[] rankingCarNames;

    private List<string> playerRankingList = new List<string>();
    private List<string> carRankingList = new List<string>();

    public NetworkVariable<GameManagerOnline.PlayerType> leaderboardWinner = new NetworkVariable<GameManagerOnline.PlayerType>(GameManagerOnline.PlayerType.None);
    public NetworkVariable<FixedString32Bytes> carWinner = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<FixedString32Bytes> carDefeated = new NetworkVariable<FixedString32Bytes>();

    [SerializeField] private GameManagerOnline.PlayerType winner;
    [SerializeField] private int winnerIndex;

    private string localCarWinner;
    private string localCarDefeated;

    private int localPlayerNameChecksum;



    // Start is called before the first frame update
    void Start()
    {
        CarSelectionHandler.OnRacingStart += CarSelectionHandler_OnRacingStart;

        leaderboardWinner.OnValueChanged += PopulateLeaderboard;
        carWinner.OnValueChanged += SetLocalCarWinner;
        carDefeated.OnValueChanged += SetLocalCarDefeated;

        CleanPlayerName();
        CalculateLocalPlayerNameChecksum();
    }

    public void SetLocalCarDefeated(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        Debug.Log("Event Invoked , with new value " + newValue);
        localCarWinner = newValue.ToSafeString();

        SetLeaderboardInformationRpc();
    }

    public void SetLocalCarWinner(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        Debug.Log("Event Invoked , with new value " + newValue);
        localCarDefeated = newValue.ToSafeString();
    }

    private void CalculateLocalPlayerNameChecksum()
    {
        byte[] bytes = Encoding.ASCII.GetBytes(MainManager.multiplayerName);

        for (int i = 0; i < bytes.Length - 1; i++)
        {
            localPlayerNameChecksum += bytes[i];
        }

        Debug.Log("Local Player Name Checksum" + localPlayerNameChecksum);
    }

    private int CalculateRVGLNameChecksum(string name)
    {
        int checksum = 0;
        byte[] bytes = Encoding.ASCII.GetBytes(name);

        foreach (var item in bytes)
        {
            checksum += item;
        }

        Debug.Log("RVGL name checksum " + checksum);
        return checksum;

    }

    private void CarSelectionHandler_OnRacingStart()
    {
        ShowRacingPanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShowRacingPanel()
    {
        racingPanel.gameObject.SetActive(true);

        if(GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Cross)
        {
            getResultsButton.gameObject.SetActive(true);
        }           

    }

    public void HideRacingPanel()
    {
        racingPanel.gameObject.SetActive(false);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void HideResultsPanelRpc()
    {
        resultsPanel.gameObject.SetActive(false);
        racingPanel.gameObject.SetActive(false);

        ResetLeaderboardWinnerRpc();
    }

    

    [Rpc(SendTo.Server)]
    public void ResetLeaderboardWinnerRpc()
    {
        leaderboardWinner.Value = GameManagerOnline.PlayerType.None;
    }

    
    public void GetResultLists()
    {
        playerRankingList.Clear();
        carRankingList.Clear();

        playerRankingList.AddRange(CSVFileReader.Instance.GetPlayerResultsList());
        carRankingList.AddRange(CSVFileReader.Instance.GetCarResultsList());
        SetCarRankingRpc();

        DetermineWinner();          
        
    }

    void CleanPlayerName()
    {
            MainManager.multiplayerName = MainManager.multiplayerName.TrimEnd(new char[] { '\r', ' ' });
            MainManager.multiplayerName = MainManager.multiplayerName.TrimStart(new char[] { '\r', ' ' });
            MainManager.multiplayerName = MainManager.multiplayerName.ToUpper();
        Debug.Log("Cleaned Multiplayer Name " + MainManager.multiplayerName);
                
    }


    public void DetermineWinner()
    {
        if(MainManager.gameUsingLogs == false) { return; }

        Debug.Log("Winner from game " + playerRankingList[0]);
        Debug.Log("Server name " + GameManagerOnline.Instance.serverPlayerName.Value);

        Debug.Log("Defeated from game " + playerRankingList[1]);
        Debug.Log("Client name " + GameManagerOnline.Instance.clientPlayerName.Value);

        Debug.Log(String.Compare(playerRankingList[0].Trim(), MainManager.multiplayerName.Trim(), System.Globalization.CultureInfo.CurrentCulture, System.Globalization.CompareOptions.StringSort));
        Debug.Log(MainManager.multiplayerName.Equals(playerRankingList[0].Trim(), StringComparison.CurrentCultureIgnoreCase));
        
        byte[] bytes = Encoding.ASCII.GetBytes(MainManager.multiplayerName);
        byte[] bytes1 = Encoding.ASCII.GetBytes(playerRankingList[0]);

        foreach (var item in bytes)
        {
            Debug.Log(item);
        }

        foreach (var item in bytes1)
        {
            Debug.Log("----------");
            Debug.Log(item);
        }

        if (CalculateRVGLNameChecksum(playerRankingList[0]) == localPlayerNameChecksum)
        {
            winner = GameManagerOnline.PlayerType.Cross;
            winnerIndex = 1;
            Debug.Log("Winner determined - Index " + winnerIndex);
        }

        else
        {
            winner = GameManagerOnline.PlayerType.Circle;
            winnerIndex = 2;
            Debug.Log("Winner determined - Index " + winnerIndex);
        }

        Debug.Log(carRankingList[0]);
        Debug.Log(carRankingList[1]);

        
    }

    public void ValidateResult()
    {
        GameManagerOnline.Instance.SetRaceWinner(winnerIndex);

        CSVFileReader.Instance.MoveCursorPosAfterSuccessfulRead();
    }

    [Rpc(SendTo.Server)]
    public void SetCarRankingRpc()
    {
        if(MainManager.gameUsingLogs == false)
            { return; }

        carWinner.Value = carRankingList[0];
        carDefeated.Value = carRankingList[1];
    }

    [Rpc(SendTo.Server)]
    private void SetLeaderboardInformationRpc()
    {        
        leaderboardWinner.Value = winner;
    }




     
    public void PopulateLeaderboard(GameManagerOnline.PlayerType previousValue, GameManagerOnline.PlayerType currentValue)
    {
        if (currentValue == GameManagerOnline.PlayerType.None)
            return;

        racingPanel.gameObject.SetActive(false);
        resultsPanel.gameObject.SetActive(true);

        if(GameManagerOnline.Instance.localPlayerType == GameManagerOnline.PlayerType.Cross)
        {
            acceptResultsButton.gameObject.SetActive(true);
            overrideResultsButton.gameObject.SetActive(true);
        }

        rankingCarNames[0].text = localCarWinner;
        rankingCarNames[1].text = localCarDefeated;

        Debug.Log(localCarWinner);
        Debug.Log(localCarDefeated);
               

        if (currentValue == GameManagerOnline.PlayerType.Cross)
        {
            leaderboardPlayerIcons[0].GetComponent<Image>().sprite = leaderboardPlayerSprites[0];
            leaderboardPlayerIcons[1].GetComponent<Image>().sprite = leaderboardPlayerSprites[1];

            rankingPlayerNames[0].text = GameManagerOnline.Instance.serverPlayerName.Value.ToString();
            rankingPlayerNames[1].text = GameManagerOnline.Instance.clientPlayerName.Value.ToString();
        }
        else
        {
            leaderboardPlayerIcons[0].GetComponent<Image>().sprite = leaderboardPlayerSprites[1];
            leaderboardPlayerIcons[1].GetComponent<Image>().sprite = leaderboardPlayerSprites[0];

            rankingPlayerNames[0].text = GameManagerOnline.Instance.clientPlayerName.Value.ToString();
            rankingPlayerNames[1].text = GameManagerOnline.Instance.serverPlayerName.Value.ToString();
        }

    }

}
