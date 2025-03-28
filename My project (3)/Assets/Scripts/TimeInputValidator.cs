using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class TimeInputValidator : MonoBehaviour
{
    public TextMeshProUGUI className;
    public TextMeshProUGUI startTime;  
    public TextMeshProUGUI endTime; 
    public TMP_Text errorMessage;  // Drag & drop the error message Text in Inspector
    public Button AddClass;

    void Start()
    {
        errorMessage.text = "";  // Clear error message initially
        AddClass.interactable = false;
        
    }

    void Update()
    {
        string start = startTime.text.Trim().Replace("\u200B", "");
        string end = endTime.text.Trim().Replace("\u200B", "");

        ValidateTime(start, end);
        
    }


    private void ValidateTime(string start, string end)
    {
        if (IsValidTime(start) && IsValidTime(end))
        {
            errorMessage.text = "";  // No error
            errorMessage.gameObject.SetActive(false);
        }
        else
        {
            errorMessage.text = $"Invalid time! Use HH:mm (24-hour format)";
            errorMessage.color = Color.red;
            errorMessage.gameObject.SetActive(true);
        }

        string classN = className.text.Trim().Replace("\u200B", "");
        if ((classN.Length != 0) && IsValidTime(start) && IsValidTime(end)) {
            AddClass.interactable = true;
            Debug.Log("GAY");
        } else {
            AddClass.interactable = false;
        }
    }

    private bool IsValidTime(string input)
    {
        // Regex pattern for HH:mm format (24-hour clock)
        string pattern = @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$";
        return Regex.IsMatch(input, pattern);
    }

    public void EnableButton() {
        string classN = this.className.text.Trim().Replace("\u200B", "");
        string startT = this.startTime.text.Trim().Replace("\u200B", "");
        string endT = this.endTime.text.Trim().Replace("\u200B", "");
        
        if ((classN.Length != 0) && IsValidTime(startT) && IsValidTime(endT)) {
            AddClass.interactable = true;

        } else {
            AddClass.interactable = false;
        }
    }
}
