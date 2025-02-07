using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class findRoomScript : MonoBehaviour
{
    public Button Button5201;
    public Button Button5107;
    public Button Button5111;
    public Button Button5105;
    public Button Button5101;

    //Making this shi better
    public SortedDictionary<string, Button> roomButtons = new SortedDictionary<string, Button>();
    public RectTransform panel;
    public Button buttonPrefab;
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

        // GenerateButtons();

        Button5201.onClick.AddListener(() => OnNavigateButtonClicked(Button5201));
        Button5107.onClick.AddListener(() => OnNavigateButtonClicked(Button5107));
        Button5111.onClick.AddListener(() => OnNavigateButtonClicked(Button5111));
        Button5105.onClick.AddListener(() => OnNavigateButtonClicked(Button5105));
        Button5101.onClick.AddListener(() => OnNavigateButtonClicked(Button5101));
        
        
        
    }


    /*Note to self: potentially pass selected floor number to function and get all rooms thay start with floor number*/
    public void GenerateButtons() { 
        List<Transform> roomParents = GetFloorLocations();
        List<string> roomNames = new List<string>();
        foreach (Transform parent in roomParents)
        {
            foreach (Transform room in parent)
            {
                roomNames.Add(room.name+"Button"); 
            }
        }

        foreach (Transform btn in panel)
        {
            Button button = btn.GetComponent<Button>();
            roomButtons[btn.name] = button;
        }


        float Yoffset = -116f;
        foreach (string roomName in roomNames)
        {
            if (!roomButtons.ContainsKey(roomName)) // If button does not exist, create it
            {
                Button newButton = Instantiate(buttonPrefab, panel);
                newButton.name = roomName;
                newButton.GetComponentInChildren<TMP_Text>().text = roomName.Replace("Button", "");

                Button lastButton = roomButtons.Values.Last();
                RectTransform lastRect = lastButton.GetComponent<RectTransform>();
                float newY = lastRect.anchoredPosition.y + Yoffset;

                RectTransform rectTransform = newButton.GetComponent<RectTransform>();
                Vector3 originalPosition = rectTransform.anchoredPosition;
                newButton.transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);

                
                newButton.onClick.AddListener(() => OnNavigateButtonClicked(newButton));

                roomButtons[roomName] = newButton;
            }
        }
    }

    public List<Transform> GetFloorLocations() {
        List<Transform> locations = new List<Transform>();
        foreach (Transform obj in FindObjectsOfType<Transform>())
        {
            if (obj.name.EndsWith("Locations"))
            {
                locations.Add(obj);
            }
        }
        return locations;
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
