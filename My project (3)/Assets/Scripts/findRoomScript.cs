using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;


public class findRoomScript : MonoBehaviour
{
    // public Button Button5201;
    // public Button Button5107;
    // public Button Button5111;
    // public Button Button5105;
    // public Button Button5101;
    // public Button Button6107;
    //public TMP_Dropdown dropDown;

    private SortedDictionary<string, ButtonADT> roomButtons = new SortedDictionary<string, ButtonADT>();
    [SerializeField] private RectTransform panel;
    public ButtonADT buttonPrefab;
    
    private static string destination;
    private static string realDestination;
    private ButtonADT clickedButton; 
    public TMP_Text locationText;
    public static string endPoint;
    public Sprite starSprite;

    void Start()
    {
        GenerateButtons();
    }

    public void GenerateButtons() { 
        List<Transform> roomParents = GetFloorLocations();
        List<string> roomNames = new List<string>();
        foreach (Transform parent in roomParents)
        {
            foreach (Transform room in parent)
            {   
                if(double.TryParse(room.name, out _)) {
                    roomNames.Add(room.name+"Button"); 
                }
                
            }
        }

        foreach (Transform btn in panel)
        {
            ButtonADT button = btn.GetComponent<ButtonADT>();
            roomButtons[btn.name] = button;
        }


        // float Yoffset = -116f;
        foreach (string roomName in roomNames)
        {
            if (!roomButtons.ContainsKey(roomName)) // If button does not exist, create it
            {
                ButtonADT newButton = Instantiate(buttonPrefab, panel);

                newButton.name = roomName;
                newButton.GetComponentInChildren<TMP_Text>().text = roomName.Replace("Button", "");
                newButton.onClick.AddListener(() => OnNavigateButtonClicked(newButton));

                Image starImage = new GameObject("StarImage").AddComponent<Image>();
                starImage.transform.SetParent(newButton.transform);
                starImage.sprite = starSprite;
                starImage.rectTransform.sizeDelta = new Vector2(150, 150);
                starImage.rectTransform.anchoredPosition = new Vector2(100, 0);

                roomButtons[roomName] = newButton;
            }
        }
    }

    public List<Transform> GetFloorLocations() {
        List<Transform> locations = new List<Transform>();
        foreach (Transform obj in FindObjectsOfType<Transform>())
        {
            if (obj.name.EndsWith("please"))
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

    private void OnNavigateButtonClicked(ButtonADT button)
    {
        clickedButton = button; 
        string selectedRoom = GetRoom();
        realDestination = selectedRoom;
        Debug.Log("Selected Room: " + selectedRoom);
        // if(selectedRoom.StartsWith("6")) {
        //     selectedRoom = "stairs5";
        // }
        string currentRoom = NavigationManager.instance.GetImageName();
        if (currentRoom.Substring(0, 1) != selectedRoom.Substring(0, 1)) {
            selectedRoom = "elevator" + currentRoom.Substring(0, 1);
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

    public static string RealDestination() {

        return realDestination;
    }


    public void SetlocationText(string NewLocationText)
{
    if (locationText != null)
    {
        locationText.text = NewLocationText;

        Debug.Log("locationText updated to:");
    }
    else
    {
        Debug.LogError("locationText is null in findRoomScript! Assign it in the Inspector.");
    }
}

    public TMP_Text GetlocationText(TMP_Text NewLocationText) { return locationText;}
}
