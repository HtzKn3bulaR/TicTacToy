using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class FolderSelectionHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI placeholderText;
    [SerializeField] private TextMeshProUGUI folderUserInput;


    public void ResetPlaceholderText()
    {
        placeholderText.text = string.Empty;
    }


    public void CheckChangedFolder()
    {
        if (folderUserInput.text == string.Empty)
        {

            placeholderText.text = "D:/Re-Volt";
        }
    }

}
