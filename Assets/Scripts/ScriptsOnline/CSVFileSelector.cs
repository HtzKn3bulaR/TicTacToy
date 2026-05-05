using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CSVFileSelector : MonoBehaviour
{
    [SerializeField] TMP_Dropdown fileDropdown;
    [SerializeField] TextMeshProUGUI rvglFolder;
    public List<string> csvFiles = new List<string>();
    public string fileName;
    public string selectedFilePath;
    private string applicationDataPath;
    private string defaultFolderName = "D:/Re-Volt";

    //private CSVFileReader fileReaderScript;



    public class ResultsSet
    {
        public static string player;
        public static string car;
        
    }

    void Start()
    {

        applicationDataPath = defaultFolderName + "/profiles/";

        GetAllCSVFiles();

    }


    void Update()
    {

    }

    public void SetNewMainFolder(string folder)
    {
        string newFolder = folder;
        defaultFolderName = newFolder;
        applicationDataPath = defaultFolderName + "/profiles/";
        GetAllCSVFiles();
    }


    public void UseLauncherFolders(bool option)
    {
        if (option)
        {
            applicationDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "/rvmm/logs";
            GetAllCSVFiles() ;

        }
        else
        {
            SetNewMainFolder(defaultFolderName);        
        }
    }


    public void GetAllCSVFiles()
    {
        csvFiles.Clear();

        try

        {
            csvFiles.Add("Select the Log File for your session");

            string[] files = Directory.GetFiles(applicationDataPath, "*csv");

            foreach (string file in files)
            {
                csvFiles.Add(Path.GetFileName(file));
            }

            if (files.Length == 0)
            {
                fileDropdown.interactable = false;
            }
        }

        catch (UnassignedReferenceException)
        {
            Debug.LogError("Access denied to some directories");
            return;

        }

        catch (Exception e)
        {
            fileDropdown.ClearOptions();
            Debug.LogError($"An error occured while accessing files: {e.Message}");
            return;
            


        }

        SetDropdownOptions();

    }


    private void SetDropdownOptions()
    {
        fileDropdown.ClearOptions();
        fileDropdown.AddOptions(csvFiles);

        if (fileDropdown.options.Count > 1)
        {
            fileDropdown.interactable = true;
        }
    }

    public void OnCSVSelected()
    {
        if (fileDropdown.value != 0)
        {
            fileName = fileDropdown.options[fileDropdown.value].text;
            selectedFilePath = applicationDataPath + fileName;
        }

        else
        {
            //fileName = "";
            //selectedFilePath = "";
        }

        if (selectedFilePath != null)
        {
            MainManager.gameUsingLogs = true;

            MainManager.selectedFilePath = selectedFilePath;

            Debug.Log("Load Session File:" + applicationDataPath + fileName);


        }



    }






}
