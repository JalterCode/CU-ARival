using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class TimeInputValidator : MonoBehaviour
{
    public TMP_InputField timeInputField;  
    public TMP_Text errorMessage;  

    private void Start()
    {
        errorMessage.text = "";  // Clear error message initially
        timeInputField.onValueChanged.AddListener(ValidateTime);
    }

    private void ValidateTime(string input)
    {
        if (IsValidTime(input))
        {
            errorMessage.text = "";  // No error
        }
        else
        {
            errorMessage.text = "Invalid time! Use HH:mm (24-hour format)";
            errorMessage.color = Color.red;
        }
    }

    private bool IsValidTime(string input)
    {
        // Regex pattern for HH:mm format (24-hour clock)
        string pattern = @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$";
        return Regex.IsMatch(input, pattern);
    }
}
