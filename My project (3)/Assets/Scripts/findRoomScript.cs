using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;


public class findRoomScript : MonoBehaviour
{
    // public Button Button5201;
    // public Button Button5107;
    // public Button Button5111;
    // public Button Button5105;
    // public Button Button5101;
    // public Button Button6107;
    //public TMP_Dropdown dropDown;

    private SortedDictionary<string, Button> roomButtons = new SortedDictionary<string, Button>();
    [SerializeField] private RectTransform panel;
    public Button buttonPrefab;
    
    private static string destination;
    private static string realDestination;
    private Button clickedButton; 
    public TMP_Text locationText;
    public static string endPoint;

    // Added for sort button
    [SerializeField] private TMP_Dropdown sortDropdown;
    public Sprite starSprite;
    public Sprite yellowStarSprite;
    void Start()
    {
        // if (Button5201 == null)
        // {
        //     Debug.LogError("Button5201 is not assigned!");
        //     return;
        // }

        // if (Button5107 == null)
        // {
        //     Debug.LogError("Button5107 is not assigned!");
        //     return;
        // }

        // Button5201.onClick.AddListener(() => OnNavigateButtonClicked(Button5201));
        // Button5107.onClick.AddListener(() => OnNavigateButtonClicked(Button5107));
        // Button5111.onClick.AddListener(() => OnNavigateButtonClicked(Button5111));
        // Button5105.onClick.AddListener(() => OnNavigateButtonClicked(Button5105));
        // Button5101.onClick.AddListener(() => OnNavigateButtonClicked(Button5101));
        // Button6107.onClick.AddListener(() => OnNavigateButtonClicked(Button6107));
        
        GenerateButtons();
        SortRoomsByFloor();
        if (sortDropdown != null)
        {
            // Populate dropdown options
            sortDropdown.ClearOptions();
            List<string> options = new List<string> { "Sort by Distance", "Sort by Floor", "Sort by Schedule" };
            sortDropdown.AddOptions(options);

            // Add listener for dropdown value change
            sortDropdown.onValueChanged.AddListener(delegate {OnSortOptionSelected(sortDropdown.value); });
        }
        else
        {
            Debug.LogWarning("SortDropdown not assigned in findRoomScript");
        }
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
            Button button = btn.GetComponent<Button>();
            roomButtons[btn.name] = button;
        }


        // float Yoffset = -116f;
        foreach (string roomName in roomNames)
        {
            if (!roomButtons.ContainsKey(roomName)) // If button does not exist, create it
            {                Button newButton = Instantiate(buttonPrefab, panel);
                newButton.name = roomName;
                newButton.GetComponentInChildren<TMP_Text>().text = roomName.Replace("Button", "");
                newButton.onClick.AddListener(() => OnNavigateButtonClicked(newButton));

                Image starImage = new GameObject("StarImage").AddComponent<Image>();
                starImage.transform.SetParent(newButton.transform);
                starImage.sprite = starSprite;
                starImage.rectTransform.sizeDelta = new Vector2(150, 150);
                starImage.rectTransform.anchoredPosition = new Vector2(100, 0);
                Button starButton = starImage.gameObject.AddComponent<Button>();
                starButton.onClick.AddListener(() => OnStarClicked(starImage));

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

    private void OnNavigateButtonClicked(Button button)
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

        //panel getter for sort button
    public RectTransform GetPanel() { return panel; }
   
 //sort the rooms by floor
    public void SortRoomsByFloor()
    {
        List<RoomData> roomDataList = new List<RoomData>();
        foreach (var kvp in roomButtons)
        {
            RoomData roomData = new RoomData
            {
                roomNumber = kvp.Key.Replace("Button", ""),
                button = kvp.Value
            };
            roomDataList.Add(roomData);
        }

        if (roomDataList.Count == 0 || NavigationManager.instance == null)
        {
            Debug.LogWarning("Cannot sort: room list empty or NavigationManager instance not found!");
            return;
        }   

        string userFloor = GetUserFloor();

        roomDataList = roomDataList.OrderBy(room =>
        {
            bool isSameFloor = room.roomNumber.StartsWith(userFloor);
            return isSameFloor ? 0 : 1;
        }).ToList();

        for (int i = 0; i < roomDataList.Count; i++)
        {
            if (roomDataList[i].button != null)
            {
                roomDataList[i].button.transform.SetSiblingIndex(i);
            }
        }

        Debug.Log($"Sorted rooms for floor {userFloor}");

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // get the user's floor
    private string GetUserFloor()
    {
        string imageName = NavigationManager.instance.GetImageName();
        Debug.Log("Image name from NavigationManager: " + imageName);
        if (!string.IsNullOrEmpty(imageName))
        {
            return imageName.Substring(0, 1);
        }

        float yPos = NavigationManager.instance.startingPoint.position.y;
        if (yPos < 10f) return "3";
        if (yPos < 20f) return "4";
        if (yPos < 30f) return "5";
        return "3";
    }

    // Added RoomData class for sorting
    [System.Serializable]
    private class RoomData
    {
        public string roomNumber;
        public Button button;
    }
    private void OnStarClicked(Image starImage)
    {
        if (starImage.sprite == starSprite)
        {
            starImage.sprite = yellowStarSprite;
        }
        else
        {
            starImage.sprite = starSprite;
        }
    }

    // Sorting Methods for Dropdown
    private void OnSortOptionSelected(int index)
    {
        switch (index)
        {
            case 0: // Sort by Distance
                SortRoomsByDistance();
                break;
            case 1: // Sort by Floor
                SortRoomsByFloor();
                break;
            case 2: // Sort by Schedule (placeholder)
                Debug.Log("Sort by Schedule not implemented yet.");
                break;
            default:
                Debug.LogWarning("Invalid sort option selected.");
                break;
        }
    }
    public void SortRoomsByDistance()
    {
        if (NavigationManager.instance == null)
        {
            Debug.LogWarning("NavigationManager instance not found!");
            return;
        }

        Vector3 userPosition = NavigationManager.instance.startingPoint.position;
        List<RoomData> roomDataList = new List<RoomData>();

        foreach (var kvp in roomButtons)
        {
            RoomData roomData = new RoomData
            {
                roomNumber = kvp.Key.Replace("Button", ""),
                button = kvp.Value
            };
            roomDataList.Add(roomData);
        }

        if (roomDataList.Count == 0)
        {
            Debug.LogWarning("No rooms to sort!");
            return;
        }

        roomDataList.Sort((a, b) =>
        {
            GameObject roomA = GameObject.Find(a.roomNumber);
            GameObject roomB = GameObject.Find(b.roomNumber);

            if (roomA == null || roomB == null)
            {
                Debug.LogError($"Room not found: {a.roomNumber} or {b.roomNumber}");
                return 0;
            }

            float distanceA = Vector3.Distance(userPosition, roomA.transform.position);
            float distanceB = Vector3.Distance(userPosition, roomB.transform.position);
            return distanceA.CompareTo(distanceB); // Closest first
        });

        for (int i = 0; i < roomDataList.Count; i++)
        {
            if (roomDataList[i].button != null)
            {
                roomDataList[i].button.transform.SetSiblingIndex(i);
            }
        }

        Debug.Log("Rooms sorted by distance from user location.");

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

}
