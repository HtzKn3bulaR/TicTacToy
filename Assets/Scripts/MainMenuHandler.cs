using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private Button help;
    [SerializeField] private Button startLocal;
    [SerializeField] private Button startRemote;

    [SerializeField] private GameObject helpPanel;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLocalGame()
    {
        SceneManager.LoadScene(1);
    }

    public void StartRemoteGame()
    {
        MainManager.Instance.gameIsRemote = true;
        SceneManager.LoadScene(5);
    }

    public void OpenHelp()
    {
        helpPanel.SetActive(true);
    }

    public void HideHelp()
    {
        helpPanel.SetActive(false); 
    }


}
