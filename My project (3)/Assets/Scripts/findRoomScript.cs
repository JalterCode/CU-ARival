using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class findRoomScript : MonoBehaviour
{
    public Button Button5201;
    public Button Button5107;
    public Button Button5111;
    public Button Button5105;
    public Button Button5101;

    public List<Button> rooms;
    //public TMP_Dropdown dropDown;
    private static string destination;
    private Button clickedButton; 
    public TMP_Text locationText;

    void Start()
    {

        if (Button5201 == null)
        {
            Debug.LogError("Button5201 is not assigned!");
            return;
        }

        if (Button5107 == null)
        {
            Debug.LogError("Button5107 is not assigned!");
            return;
        }

        Button5201.onClick.AddListener(() => OnNavigateButtonClicked(Button5201));
        Button5107.onClick.AddListener(() => OnNavigateButtonClicked(Button5107));
        Button5111.onClick.AddListener(() => OnNavigateButtonClicked(Button5111));
        Button5105.onClick.AddListener(() => OnNavigateButtonClicked(Button5105));
        Button5101.onClick.AddListener(() => OnNavigateButtonClicked(Button5101));
        
        
        
    }

    public string GetRoom()
    {
        if (clickedButton == null)
        {
            Debug.LogWarning("No button has been clicked");
            return null;
        }

        TextMeshProUGUI textComponent = clickedButton.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            return textComponent.text; 
        }

        Debug.LogError("No TextMeshProUGUI component found lol");
        return null;
    }

    private void OnNavigateButtonClicked(Button button)
    {
        clickedButton = button; 
        string selectedRoom = GetRoom();
        Debug.Log("Selected Room: " + selectedRoom);

        // Pass the selected room to the NavigationManager
        SetDestination(selectedRoom.Replace(" ", "").Trim());
        locationText.text = "Navigating to room " + selectedRoom;
    
        NavigationManager.instance.UpdateNavigationTarget(selectedRoom);
    }

    public void SetDestination(string roomName)
    {
        destination = roomName;
    }

    public static string GetDestination()
    {
        return destination;
    }
}
