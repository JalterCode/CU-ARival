using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.AI;


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
    public TMP_FontAsset buttonFont;


    // Added for sort button
    public Button sortButton;               
    public Sprite starSprite;
    public Sprite yellowStarSprite;

    public Transform playerCamera;
    void Start()
    {

        Debug.Log("Find room script");
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
        
        if (sortButton != null)
        {
            sortButton.onClick.AddListener(SortRoomsByFloor);
        }
        else
        {
            Debug.LogWarning("Sort button not assigned in findRoomScript!");
        }
    }

    public void GenerateButtons(GameObject startingLocation) { 


        List<Transform> roomParents = GetFloorLocations();
        List<string> roomNames = new List<string>();
        List<GameObject> roomObjects = new List<GameObject>();
        Dictionary<string, float> roomDistances = new Dictionary<string, float>();

        foreach (Transform parent in roomParents)
        {
            foreach (Transform room in parent)
            {   
                if(double.TryParse(room.name, out _)) {
                    roomNames.Add(room.name); 
                }
                
            }
        }

        foreach (Transform btn in panel)
        {
            Button button = btn.GetComponent<Button>();
            roomButtons[btn.name] = button;
        }

        //get distances

        foreach (string roomName in roomNames)
    {
        GameObject roomObject = GameObject.Find(roomName);
        if (roomObject != null)
        {
            Vector3 endPos = roomObject.transform.position;

            float navMeshDistance = CalculateNavMeshPathDistance(startingLocation.transform.position, endPos);

            roomDistances.Add(roomName, navMeshDistance);
        }
    }


        // float Yoffset = -116f;
        foreach (string roomName in roomNames)
        {
            if (!roomButtons.ContainsKey(roomName)) // If button does not exist, create it
            {                
                Button newButton = Instantiate(buttonPrefab, panel);
                newButton.name = roomName;
                newButton.GetComponentInChildren<TMP_Text>().text = roomName;
                newButton.onClick.AddListener(() => OnNavigateButtonClicked(newButton));


                //Stars
                Image starImage = new GameObject("StarImage").AddComponent<Image>();
                starImage.transform.SetParent(newButton.transform);
                starImage.sprite = starSprite;
                starImage.rectTransform.sizeDelta = new Vector2(90, 90);
                starImage.rectTransform.anchoredPosition = new Vector2(-45, 0);
                Button starButton = starImage.gameObject.AddComponent<Button>();
                starButton.onClick.AddListener(() => OnStarClicked(starImage));
                roomButtons[roomName] = newButton;

                //Distance
                float objectDistance = roomDistances[roomName];
                GameObject distanceTextObj = new GameObject("DistanceText");
                distanceTextObj.transform.SetParent(newButton.transform);
                TextMeshProUGUI distanceText = distanceTextObj.AddComponent<TextMeshProUGUI>();
                distanceText.text = $"{objectDistance:F2} metres away";
                distanceText.rectTransform.sizeDelta = new Vector2(600, 30);
                distanceText.rectTransform.anchoredPosition = new Vector2(180, 0);
                distanceText.alignment = TextAlignmentOptions.Center;
                distanceText.fontSize = 50;
                distanceText.color = Color.grey;
                distanceText.font = buttonFont; 

                if (startingLocation.name.Substring(0, 1) != roomName.Substring(0, 1)) distanceText.text = "";


            }
        }
                SortRoomsByFloor();

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

    public TMP_Text GetlocationText() { return locationText;}

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

    private float CalculateNavMeshPathDistance(Vector3 startPoint, Vector3 endPoint)
{
    NavMeshPath path = new NavMeshPath();

    float distance = 0.0f;
    if (NavMesh.CalculatePath(startPoint, endPoint, NavMesh.AllAreas, path))
    {
        
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            distance = 0.0f;

            if (path.corners.Length > 1)
            {
                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    float segmentDistance = Vector3.Distance(path.corners[i], path.corners[i + 1]);
                    distance += segmentDistance;
                }
            }
        }
    }
    return distance;
}

}