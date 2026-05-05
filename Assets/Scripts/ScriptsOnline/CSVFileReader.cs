using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.Events;


public class CSVFileReader : MonoBehaviour
{

    public static CSVFileReader Instance;
        
    [SerializeField] TextMeshProUGUI trackPlayed;
       

    [SerializeField] GameObject SetNewLogFilePanel;
      

    private char[] trimchar;

    private int cursorPos = 4;
    private int trackCursor = 2;
    public string selectedFilePath;

    private int linesInCSV;
    public bool CSVfileIsNew = false;

    private List<string> resultLines = new List<string>();
    private string trackLine = "No Data";

    private List<string> playerNames = new List<string>();
    private List<string> carNames = new List<string>();
    
    private string trackInfo;
        
    private List<string> cleanedNames = new List<string>();

    private List<string> validLineMarkers = new List<string>() { "01", "02" };

    private CSVFileSelector fileSelectorScript;
       

    public int changeValue;

    private int playerNumber = 2;

    

    void Start()
    {
        Instance = this;

        if (selectedFilePath != null)
        {
            selectedFilePath = MainManager.selectedFilePath;
        }

        fileSelectorScript = GameObject.Find("CSVFileSelector").GetComponent<CSVFileSelector>();             

    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<string> GetPlayerResultsList()
    {
        return playerNames;
    }

    public List<string> GetCarResultsList()
    {
        return carNames;
    }


    public void ReadCSVFileCurrentRound()
    {
        if (MainManager.selectedFilePath != null)
        {

            string[] lines = File.ReadAllLines(MainManager.selectedFilePath);

            if (CSVfileIsNew)
            {
                cursorPos = 4;
                trackCursor = 2;
                CSVfileIsNew = false;
            }

            else
            {
                trackCursor = cursorPos - 2;
                Debug.Log("Track cursor set to " + trackCursor);
            }

            Debug.Log("Number of lines in file " + lines.Length);
            linesInCSV = lines.Length;

            resultLines.Clear();
            Debug.Log("Cursor position before reading" + cursorPos);
            Debug.Log("Track Cursor before reading" + trackCursor);

            if (cursorPos > lines.Length)
            {
                Debug.Log("No more session data found in file");
                playerNames.Clear();
                carNames.Clear();
                
                LeaderboardClear();

                SetNewLogFilePanel.gameObject.SetActive(true);
                fileSelectorScript.GetAllCSVFiles();

                CSVfileIsNew = true;

            }

            else if ((cursorPos + playerNumber) > lines.Length)
            {

                for (int i = cursorPos; i < (lines.Length - cursorPos); i++)
                {
                    resultLines.Add(lines[i]);
                }

                trackLine = lines[trackCursor];
                Debug.Log(lines[trackCursor]);

            }

            else
            {
                for (int i = cursorPos; i < (cursorPos + playerNumber); i++)
                {

                    resultLines.Add(lines[i]);

                }

                trackLine = lines[trackCursor];
                Debug.Log(lines[trackCursor]);

                ExtractResultsData();
            }

        }

        else
        {
            //No Log File Has Been Selected

            if (MainManager.gameUsingLogs)
            {
                SetNewLogFilePanel.gameObject.SetActive(true);
            }

            else 
            {
                GameManagerOnline.Instance.ShowPostRacePanel();
            }

        }

    }

    void ExtractResultsData()
    {

        playerNames.Clear();
        carNames.Clear();
        
        char[] chars = { '"', '#' };
        bool checkOK = true;

        string[] firstCheckLine = resultLines[0].Split(",");

        if (firstCheckLine[0].Trim(chars) == "01")
        {
            Debug.Log("Valid race result set found!");
            checkOK = true;
        }

        else
        {
            Debug.Log("Line Invalid! Looking Up next line");
            checkOK = false;
        }


        switch (checkOK)
        {
            case true:

                string[] trackData = trackLine.Split(",");
                trackInfo = trackData[1].Trim(chars);

                for (int i = 0; i < playerNumber; i++)

                {
                    string[] lineData = resultLines[i].Split(",");



                    Debug.Log(lineData[1].Trim(chars));

                    if (validLineMarkers.Contains(lineData[0].Trim(chars)))

                    {
                        playerNames.Add(lineData[1].Trim(chars));
                        carNames.Add(lineData[2].Trim(chars));
                        Debug.Log("Line " + i + "was read");

                    }
                    else
                    {
                        cursorPos--;

                        Debug.Log("Not all players have log file entries");
                    }

                }
                CleanCSVNames();
                break;

            case false:
                cursorPos++;
                ReadCSVFileCurrentRound();
                break;
        }

    }

    public void MoveCursorPosAfterSuccessfulRead()
    {

        if ((cursorPos + playerNumber) > linesInCSV)
        {
            cursorPos += (linesInCSV - cursorPos);
            cursorPos += 2;

            Debug.Log("Cursor position after reading " + cursorPos);
        }

        else
        {
            cursorPos += (playerNumber);
            cursorPos += 2;
            Debug.Log("Cursor position after reading " + cursorPos);
        }


    }


    void CleanCSVNames()
    {
        for (int i = 0; i < playerNames.Count; i++)
        {
            playerNames[i] = playerNames[i].TrimEnd(new char[] { '\r', ' ' });
            playerNames[i] = playerNames[i].TrimStart(new char[] { '\r', ' ' });
            playerNames[i] = playerNames[i].ToUpper();
            Debug.Log("Name cleaned by File Reader: " + playerNames[i]);

        }
    }
           
      
        

    public void SetAutoResultsValid()
    {
        //MainManager.autoResultsValid = true;

    }

    public void SetAutoResultsInvalid()
    {
        //MainManager.autoResultsValid = false;
    }




    public void LeaderboardClose()
    {
        
        LeaderboardClear();
    }

    private void LeaderboardClear()
    {
        for (int i = 0; i < 5; i++)
        {
            //LBplayers[i].text = "";
            //LBcars[i].text = "";          
        }              

        Debug.Log("Leaderboard cleared");

    }

    public void SetNewFilePanelClose()
    {
        SetNewLogFilePanel.gameObject.SetActive(false);

    }
           
    public void RaceInProgessPanelClose()
    {
        //raceInProgressPanel.gameObject.SetActive(false);
    }       

}
