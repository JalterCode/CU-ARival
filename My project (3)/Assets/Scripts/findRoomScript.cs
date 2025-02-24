using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class findRoomScript : MonoBehaviour
{
    public Button Button5201;
    public Button Button5107;
    public Button Button5111;
    public Button Button5105;
    public Button Button5101;
    public Button Button6107;
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
        Button6107.onClick.AddListener(() => OnNavigateButtonClicked(Button6107));
        
        
        
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
        // if(selectedRoom.StartsWith("6")) {
        //     selectedRoom = "stairs5";
        // }
        string currentRoom = NavigationManager.instance.GetImageName();
        if (currentRoom.Substring(0, 1) != selectedRoom.Substring(0, 1)) {
            selectedRoom = "stairs" + currentRoom.Substring(0, 1);
        }

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
